using System;
using System.Globalization;
using System.Linq;
using SelectMiscreationFeat;

internal static class LocalizedSearchChecks
{
    internal static void Run()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            // ゲームの公式訳ではなく、各言語の表示文を模した検索用データ。
            foreach (var sample in new[]
            {
                (Culture: "en-US", Label: "Beast Gene (Strength+3, Fire Resistance)",
                    Name: "bEaSt", Ability: "sTrEnGtH", Feat: "fIrE rEsIsTaNcE", Other: "Bird Gene (Magic+5)"),
                (Culture: "zh-CN", Label: "野兽的基因(力量+3, 火焰抗性)",
                    Name: "野兽", Ability: "力量", Feat: "火焰抗性", Other: "鸟的基因(魔力+5)"),
                (Culture: "zh-TW", Label: "野獸的基因(力量+3, 火焰抗性)",
                    Name: "野獸", Ability: "力量", Feat: "火焰抗性", Other: "鳥的基因(魔力+5)")
            })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(sample.Culture);
                CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
                var labels = new[] { sample.Label, sample.Other, sample.Label + " +7" };

                // 遺伝子名・能力名・フィート名・数値を部分一致で検索する。
                foreach (var term in new[] { sample.Name, sample.Ability, sample.Feat, "+3" })
                {
                    var query = new GeneSearchQuery();
                    query.Queue(term, 0);
                    Check(!query.TryApply(term, false, 0.1f), sample.Culture + ": updated before delay");
                    Check(query.TryApply(term, false, 0.21f), sample.Culture + ": search not applied");
                    Check(labels.Where(query.Matches).SequenceEqual(new[] { labels[0], labels[2] }),
                        sample.Culture + ": wrong matches or order for " + term);

                    // ゼロ件から検索解除すると元の一覧へ戻る。
                    query.Queue("__missing__", 1);
                    Check(query.TryApply("__missing__", false, 1.21f) && !labels.Any(query.Matches),
                        sample.Culture + ": zero-match search failed");
                    query.Queue("", 2);
                    Check(query.TryApply("", false, 2.21f) && labels.All(query.Matches),
                        sample.Culture + ": clear failed");
                }
                Console.WriteLine("PASS: localized search " + sample.Culture);
            }

            // ピンイン入力中は待機し、確定した漢字で検索する。
            CheckChineseComposition("li", "力量", "野兽的基因(力量+3)");
            CheckChineseComposition("ye shou", "野兽", "野兽的基因(力量+3)");
            CheckChineseComposition("ye shou", "野獸", "野獸的基因(力量+3)");
            Console.WriteLine("PASS: Chinese IME commit delay and cancellation");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    private static void CheckChineseComposition(string composing, string committed, string label)
    {
        var query = new GeneSearchQuery();
        query.Queue(composing, 0);
        Check(!query.TryApply(composing, true, 1), "Chinese composition applied prematurely");
        Check(!query.TryApply(committed, false, 2), "Chinese commit skipped delay");
        Check(!query.TryApply(committed, false, 2.1f), "Chinese commit delay too short");
        Check(query.TryApply(committed, false, 2.21f) && query.Matches(label), "Chinese commit failed");
        Check(!query.Matches(composing), "Pinyin matched instead of committed characters");

        // 次の変換を取り消した場合は、確定済みの検索条件を維持する。
        query.Queue("x", 3);
        Check(!query.TryApply("x", true, 4), "Cancelled composition applied prematurely");
        Check(!query.TryApply(committed, false, 5), "Cancellation updated immediately");
        Check(!query.TryApply(committed, false, 5.21f) && query.Matches(label), "Cancellation changed query");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}
