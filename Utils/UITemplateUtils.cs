using ColossalFramework.UI;
using System.Collections.Generic;
using System.Reflection;

namespace Commons.Utils
{
    internal class UITemplateUtils
    {
        public static Dictionary<string, UIComponent> GetTemplateDict() => (Dictionary<string, UIComponent>) typeof(UITemplateManager).GetField("m_Templates", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(UITemplateManager.instance);
    }

}

