using UnityEngine;
using UnityEngine.UI;

namespace SelectMiscreationFeat;

internal static class GeneSearchInput
{
    internal static (InputField, Text) Create(LayerList layer)
    {
        // 外枠にはタイトル領域も含まれるため、リストの表示領域を基準にする。
        Canvas.ForceUpdateCanvases();
        var viewport = layer.scroll.viewport != null
            ? layer.scroll.viewport
            : (RectTransform)layer.scroll.content.parent;
        var bar = new GameObject("GeneSearch", typeof(RectTransform), typeof(LayoutElement));
        var rect = (RectTransform)bar.transform;
        rect.SetParent(viewport.parent, false);
        bar.GetComponent<LayoutElement>().ignoreLayout = true;
        rect.anchorMin = new Vector2(viewport.anchorMin.x, viewport.anchorMax.y);
        rect.anchorMax = viewport.anchorMax;
        rect.offsetMin = new Vector2(viewport.offsetMin.x, viewport.offsetMax.y - 36);
        rect.offsetMax = viewport.offsetMax;

        // 入力欄36pxと間隔8pxを確保し、候補が検索欄に重ならないようにする。
        viewport.offsetMax -= new Vector2(0, 44);

        // ゲームのフォントを使い、日本語を入力できる標準InputFieldを作る。
        var template = layer.list.moldItem.GetComponent<ItemGeneral>().button1.mainText;
        var fieldObject = new GameObject("Input", typeof(RectTransform), typeof(Image), typeof(InputField));
        var fieldRect = (RectTransform)fieldObject.transform;
        fieldRect.SetParent(rect, false);
        // 入力欄の左端を候補のショートカット文字付近に寄せる。
        Stretch(fieldRect, 28, 100);
        var background = fieldObject.GetComponent<Image>();
        background.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);
        var field = fieldObject.GetComponent<InputField>();
        field.targetGraphic = background;
        field.textComponent = CreateText(fieldRect, template, "Text", "", Color.white);
        field.placeholder = CreateText(fieldRect, template, "Placeholder", "Search...", Color.gray);
        field.lineType = InputField.LineType.SingleLine;
        field.navigation = new Navigation { mode = Navigation.Mode.None };

        // ゼロ件でも結果件数を表示して検索欄を残す。
        var count = CreateText(rect, template, "Count", "", template.color);
        var countRect = (RectTransform)count.transform;
        countRect.anchorMin = new Vector2(1, 0);
        countRect.anchorMax = Vector2.one;
        countRect.offsetMin = new Vector2(-96, 0);
        countRect.offsetMax = Vector2.zero;
        count.alignment = TextAnchor.MiddleRight;
        return (field, count);
    }

    private static Text CreateText(Transform parent, Text template, string name, string value, Color color)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        Stretch((RectTransform)obj.transform, 8, 8);
        var text = obj.GetComponent<Text>();
        text.font = template.font;
        text.fontSize = template.fontSize;
        text.fontStyle = template.fontStyle;
        text.alignment = TextAnchor.MiddleLeft;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.supportRichText = false;
        text.raycastTarget = false;
        text.color = color;
        text.text = value;
        return text;
    }

    private static void Stretch(RectTransform rect, float left, float right)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, 0);
        rect.offsetMax = new Vector2(-right, 0);
    }
}
