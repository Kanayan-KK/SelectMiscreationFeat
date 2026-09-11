using System;

namespace SelectMiscreationFeat;

internal sealed class GeneSearchQuery
{
    private string applied = "";
    private string pending = "";
    private float updateAt;
    private bool composing;

    internal void Queue(string text, float now)
    {
        pending = text;
        updateAt = now + 0.2f;
    }

    internal bool TryApply(string text, bool isComposing, float now)
    {
        // 変換途中の文字では絞り込まず、確定後から待つ。
        if (isComposing)
        {
            composing = true;
            return false;
        }
        if (composing)
        {
            composing = false;
            Queue(text, now);
        }
        if (pending == applied || now < updateAt) return false;
        applied = pending;
        return true;
    }

    internal bool Matches(string label) => label.IndexOf(applied, StringComparison.OrdinalIgnoreCase) >= 0;
}
