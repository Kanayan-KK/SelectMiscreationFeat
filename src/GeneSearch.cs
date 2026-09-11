using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SelectMiscreationFeat;

public sealed class GeneSearch : MonoBehaviour
{
    private LayerList layer = null!;
    private InputField input = null!;
    private Text count = null!;
    private List<Thing> candidates = null!;
    private List<Thing> visible = null!;
    private Dictionary<Thing, string> summaries = null!;
    private readonly GeneSearchQuery query = new GeneSearchQuery();
    private int focusedFrame = -10;

    public bool BlocksShortcuts => input != null &&
        (input.isFocused || Input.compositionString.Length > 0 || Time.frameCount <= focusedFrame + 1);

    public void Initialize(LayerList target, List<Thing> all, List<Thing> filtered,
        Dictionary<Thing, string> labels)
    {
        layer = target;
        candidates = all;
        visible = filtered;
        summaries = labels;

        // 検索結果が減っても入力欄とウィンドウの位置を固定する。
        var window = (RectTransform)layer.windows[0].transform;
        layer.SetSize(-1, Mathf.Max(window.sizeDelta.y, 240));
        (input, count) = GeneSearchInput.Create(layer);
        input.onValueChanged.AddListener(QueueSearch);
        UpdateCount();
    }

    private void QueueSearch(string text)
    {
        query.Queue(text, Time.unscaledTime);
    }

    private void Update()
    {
        if (input == null) return;
        if (input.isFocused) focusedFrame = Time.frameCount;

        if (!query.TryApply(input.text, input.isFocused && Input.compositionString.Length > 0, Time.unscaledTime)) return;

        // 候補本体を保ち、キャッシュ済みの表示文だけで絞り込む。
        visible.Clear();
        foreach (var candidate in candidates)
            if (query.Matches(summaries[candidate])) visible.Add(candidate);
        layer.list.page = 0;
        layer.list.List();
        layer.scroll.verticalNormalizedPosition = 1;
        UpdateCount();
    }

    private void UpdateCount()
    {
        count.text = $"{visible.Count} / {candidates.Count}";
    }
}

[HarmonyPatch(typeof(LayerList), nameof(LayerList.OnUpdateInput))]
internal static class GeneSearchInputPatch
{
    // このMODの検索欄だけを対象に文字キーによる即決定を止める。
    private static bool Prefix(LayerList __instance)
    {
        var search = __instance.GetComponent<GeneSearch>();
        return search == null || !search.BlocksShortcuts;
    }
}

[HarmonyPatch(typeof(Layer), nameof(Layer.OnBack))]
internal static class GeneSearchBackPatch
{
    // IMEの変換取り消しで遺伝子選択画面まで閉じないようにする。
    private static bool Prefix(Layer __instance)
    {
        var search = __instance is LayerList ? __instance.GetComponent<GeneSearch>() : null;
        return search == null || !search.BlocksShortcuts;
    }
}
