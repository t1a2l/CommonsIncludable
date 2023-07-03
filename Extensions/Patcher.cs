using HarmonyLib;
using Commons.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Commons.Extensions
{
	public class Patcher : MonoBehaviour
    {
        private const string HarmonyId = "com.redirectors.TLM";

        private static bool patched = false;

        public static BindingFlags allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.SetField;

        public interface IPatcher
        {
        }

        public static void PatchAll()
        {
            if (patched) return;

            Debug.Log("com.redirectors.TLM: Patching...");

            patched = true;
            GameObject m_topObj = GameObject.Find("Patches") ?? new GameObject("Patches");
            Type typeTarg = typeof(IPatcher);
            List<Type> instances = ReflectionUtils.GetInterfaceImplementations(typeTarg, typeTarg);
            try
            {
                foreach (Type t in instances)
                {
                    LogUtils.DoLog($"Patch: {t}");
                    m_topObj.AddComponent(t);

                }
            }
            finally
            {
                
            }
            // Apply your patches here!
            // Harmony.DEBUG = true;
            var harmony = new Harmony("com.redirectors.TLM");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAll()
        {
            if (!patched) return;

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);

            patched = false;

            UnityEngine.Debug.Log("com.redirectors.TLM: Reverted...");
        }
    }
}
