using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Custom_Cosmetics.Patches;

[HarmonyPatch]
public class SaveManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SaveData), nameof(SaveData.IsCostumeUnlocked))]
    public static void ForceCustomCostumeUnlock(CostumeObject costume, ref bool __result)
    {
        if (costume != null && costume.name.StartsWith("custom-costume-"))
        {
            __result = true;
        }
    }
}
