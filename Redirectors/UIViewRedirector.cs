using ColossalFramework.UI;
using HarmonyLib;
using Commons.Extensions;
using Commons.Interfaces;
using Commons.Utils;
using System;
using static Commons.Extensions.Patcher;

namespace Commons.Redirectors
{
    public class UIViewRedirector : Patcher, IPatcher
    {
        [HarmonyPatch(typeof(UIView), "Start")]
        [HarmonyPostfix]
        public static void AfterStart()
        {
            System.Collections.Generic.List<Type> impls = ReflectionUtils.GetInterfaceImplementations(typeof(IViewStartActions), typeof(UIViewRedirector));
            foreach (Type impl in impls)
            {
                var inst =impl.GetConstructor(new Type[0])?.Invoke(new object[0]) as IViewStartActions;
                inst?.OnViewStart();
            }
        }
    }
}