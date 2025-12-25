using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace SelectMiscreationFeat
{
    [HarmonyPatch]
    public class Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DramaOutcome), nameof(DramaOutcome.upgrade_miscreation))]
        public static bool UpgradeMiscreation(DramaOutcome __instance)
        {
            // できそこないフィート(id 1248)を持つキャラクターを見つける
            var chara = EMono.pc.party.members.Find((Chara c) => !c.IsPC && c.HasElement(1248));

            // キャラクターが見つからない場合は元のメソッドを実行
            if (chara == null)
                return true;

            // フィート値(アップグレード数)を取得
            var num = chara.Evalue(1248);
            if (num <= 0)
                return true;

            // フィートレベルをリセット（既存処理を踏襲）
            chara.SetFeat(1248, 0);

            // 選択プロセスを開始
            SelectGenes(chara, num, 0);

            // 元のメソッドをスキップ
            return false;
        }

        /// <summary>
        /// 遺伝子選択プロセス
        /// </summary>
        /// <param name="chara">キャラオブジェクト</param>
        /// <param name="remaining">残選択回数</param>
        /// <param name="step">選択済回数</param>
        private static void SelectGenes(Chara chara, int remaining, int step)
        {
            if (remaining <= 0)
            {
                // 乱数リセットして終了
                Rand.SetSeed();
                return;
            }

            // 候補を生成
            var candidates = new List<Thing>();
            var attempts = 0;

            // 選択肢数を設定値から取得（最大1000）
            var choiceCount = Mathf.Min(Plugin.Instance?.ChoiceCount?.Value ?? 20, 1000);

            while (candidates.Count < choiceCount && attempts < choiceCount * 10)
            {
                // シード値の決定
                var seed = chara.uid + step + (attempts * 1000);

                // EnableRandomがtrueの場合は完全にランダムなシード値を使用
                if (Plugin.Instance?.EnableRandom?.Value ?? false)
                    seed = EClass.rnd(10000000);

                attempts++;

                // 元のロジック: DNA.GenerateRandomGene(chara.LV + 30, chara.uid + i);
                var gene = DNA.GenerateRandomGene(chara.LV + 30, seed);

                // 有効性チェック
                if (gene.c_DNA.GetInvalidFeat(chara) == null && gene.c_DNA.GetInvalidAction(chara) == null)
                {
                    // 候補リストに追加
                    candidates.Add(gene);
                }
                else
                {
                    // 削除
                    gene.Destroy();
                }
            }

            // UIを表示
            EClass.ui.AddLayer<LayerList>()
                .SetList2(
                    candidates,
                    GetGeneSummary,
                    (t, _) =>
                    {
                        // 選択時
                        t.c_DNA.Apply(chara, reverse: false);
                        EMono.Sound.Play("good");

                        // クリーンアップのためにすべての候補を破棄
                        foreach (var c in candidates)
                            c.Destroy();

                        // 続行
                        SelectGenes(chara, remaining - 1, step + 1);
                    },
                    (t, item) =>
                    {
                        // テキストの自動改行を無効化
                        item.button1.mainText.horizontalOverflow = HorizontalWrapMode.Overflow;
                    }
                )
                .SetSize(1000)
                .SetHeader($"Select Gene for {chara.Name} ({remaining} remaining)");
        }

        /// <summary>
        /// 遺伝子効果テキストを作成するメソッド
        /// </summary>
        private static string GetGeneSummary(Thing t)
        {
            var dna = t.c_DNA;
            var summary = t.Name; // "Xの遺伝子"

            var stats = new List<string>();
            for (var i = 0; i < dna.vals.Count; i += 2)
            {
                var id = dna.vals[i];
                var val = dna.vals[i + 1];
                var e = Element.Create(id, val);

                // フィートでなければ＋を付ける
                var sign = (e.Value > 0 && e.source.category != "feat") ? "+" : "";

                // フォーマット: "筋力+3", "フィート名"
                var text = e.Name;
                if (e.source.category != "feat" && e.source.category != "ability")
                    text += $"{sign}{e.Value}";
                stats.Add(text);
            }

            if (stats.Count > 0)
                summary += "(" + string.Join(", ", stats) + ")";

            return summary;
        }
    }
}