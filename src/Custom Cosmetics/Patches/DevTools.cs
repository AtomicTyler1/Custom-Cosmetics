using HarmonyLib;
using System;
using System.Linq;
using UnityEngine.EventSystems;

namespace Custom_Cosmetics.Patches
{
    [HarmonyPatch]
    public static class Devtools
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CostumeChangeUI), nameof(CostumeChangeUI.OnEntityCreated))]
        public static void OnSelect(CostumeChangeUI __instance)
        {
            if (Plugin.CosmeticDevTools.Value)
            {
                Plugin.devUI._isVisible = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.OnGameManagerLoad))]
        public static void Disable(GameManager __instance)
        {
            if (Plugin.CosmeticDevTools.Value)
            {
                Plugin.devUI._isVisible = false;
            }
        }
    }
}