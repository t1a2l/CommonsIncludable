using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Commons.UI.i18n;
using Commons.Utils;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Commons.Redirectors
{
    [HarmonyPatch(typeof(UIDynamicPanels))]
    public static class UIDynamicPanelsRedirector
    {
        [HarmonyPatch(typeof(UIDynamicPanels), "Init")]
        [HarmonyPrefix]
        public static void PreInit(UIView view, UIDynamicPanels __instance)
        {
            int? ct = __instance.m_DynamicPanels?.Where(x =>
            {
                try
                {
                    return x?.name == DialogControl.PANEL_ID && x.panelRoot?.gameObject != null && string.Compare(x.panelRoot?.stringUserData, DialogControl.VERSION) >= 0;
                }
                catch
                {
                    return false;
                }
            }).Count();
            if (ct == 0)
            {
                var oldPanel = __instance.m_DynamicPanels.Where(x => x?.name == DialogControl.PANEL_ID).FirstOrDefault();
                if (oldPanel != null)
                {
                    LogUtils.DoWarnLog($"Unregistering older  panel (v: {oldPanel.panelRoot?.stringUserData})");
                    oldPanel.panelRoot.enabled = false;
                    oldPanel.Destroy();
                    Object.Destroy(oldPanel.panelRoot);
                }
                var listDynPanel = __instance.m_DynamicPanels.Where(x => x?.name != DialogControl.PANEL_ID).ToList();
                listDynPanel.Insert(0, DialogControl.CreatePanelInfo(view));
                __instance.m_DynamicPanels = listDynPanel.ToArray();
                TLMLocaleManager.m_localeStringsDictionary(TLMLocaleManager.m_localeManagerLocale(LocaleManager.instance))[new Locale.Key() { m_Identifier = TLMLocaleManager.m_defaultTestKey }] = "OK";
                TLMLocaleManager.m_localeStringsDictionary(TLMLocaleManager.m_localeManagerLocale(LocaleManager.instance))[new Locale.Key() { m_Identifier = TLMLocaleManager.m_defaultModControllingKey }] = CommonProperties.ModName;
            }
        }

        public static void RemovePanel()
        {
            if (!(UIView.library is null))
            {
                UIView.library.m_DynamicPanels = UIView.library.m_DynamicPanels.Where(x => x?.name != DialogControl.PANEL_ID).ToArray();
            }
        }
    }
}