using System;
using SelectMiscreationFeat;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

// 連続入力では最後の入力から0.2秒待つ。
var query = new GeneSearchQuery();
query.Queue("筋", 0);
Check(!query.TryApply("筋", false, 0.1f), "Updated before delay");
query.Queue("筋力", 0.1f);
Check(!query.TryApply("筋力", false, 0.21f), "Did not debounce repeated input");
Check(query.TryApply("筋力", false, 0.31f), "Did not update after delay");
Check(query.Matches("獣の遺伝子(筋力+3)"), "Japanese partial match failed");
Check(!query.Matches("獣の遺伝子(魔力+3)"), "Unrelated label matched");
Check(!query.TryApply("筋力", false, 1), "Unchanged query updated again");

// 表示文全体を対象に英字の大小を無視し、数値も検索する。
query.Queue("gEnE", 1);
Check(query.TryApply("gEnE", false, 1.21f) && query.Matches("Strong Gene (+3)"), "Case-insensitive match failed");
query.Queue("+3", 2);
Check(query.TryApply("+3", false, 2.21f) && query.Matches("筋力+3"), "Numeric match failed");
query.Queue("存在しない", 3);
Check(query.TryApply("存在しない", false, 3.21f) && !query.Matches("筋力+3"), "Zero-match search failed");
query.Queue("", 4);
Check(query.TryApply("", false, 4.21f) && query.Matches("筋力+3") && query.Matches("Gene"), "Clearing did not restore all");

// IME変換が長引いても確定前に適用せず、確定後にも待つ。
query.Queue("ま", 5);
Check(!query.TryApply("ま", true, 6), "Updated during composition");
Check(!query.TryApply("魔力", false, 7), "Updated immediately on IME commit");
Check(!query.TryApply("魔力", false, 7.1f), "IME commit delay was too short");
Check(query.TryApply("魔力", false, 7.21f) && query.Matches("魔力+5"), "Committed text not applied");
Console.WriteLine("PASS: debounce, partial matching, case, numbers, zero results, clear, IME commit");
LocalizedSearchChecks.Run();
