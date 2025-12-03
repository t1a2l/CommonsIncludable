using UnityEngine;

namespace Commons.LiteUI.BaseElements
{
    public abstract class GUIRootWindowBase(string title, Rect rect, bool resizable = true, bool hasTitlebar = true, Vector2 minSize = default) : GUIWindow(title, rect, resizable, hasTitlebar, minSize)
    {
    }
}