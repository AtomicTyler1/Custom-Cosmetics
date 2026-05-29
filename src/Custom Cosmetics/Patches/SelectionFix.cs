using HarmonyLib;
using System.Linq;
using UnityEngine.EventSystems;

namespace Custom_Cosmetics.Patches
{
    [HarmonyPatch]
    public static class SelectionFix
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CostumeChangeUI), nameof(CostumeChangeUI.OnSelect))]
        public static void OnSelect(CostumeChangeUI __instance)
        {
            if (GameUtil.TryGetLocalPlayer(out var player))
            {
                PlayerCostumeManager manager = player.GetObject<PlayerCostumeManager>();
                if (manager != null)
                {
                    var indices = manager.unlockedCostumeIndicies.ToList();
                    for (int i = 0; i < manager.costumes.Length; i++)
                    {
                        if (!indices.Contains(i) && manager.costumes[i].name.StartsWith("custom-costume-"))
                            indices.Add(i);
                    }
                    manager.unlockedCostumeIndicies = indices.ToArray();
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CostumeChangeUI), nameof(CostumeChangeUI.Cycle))]
        public static void ClampCostumeIndex()
        {
            if (!GameUtil.TryGetLocalPlayer(out var player)) return;
            var manager = player.GetObject<PlayerCostumeManager>();
            if (manager == null) return;

            int max = manager.unlockedCostumeIndicies.Length - 1;
            if (manager.currentUnlockedCostumeIndex > max)
                manager.currentUnlockedCostumeIndex = max;
            if (manager.currentUnlockedCostumeIndex < 0)
                manager.currentUnlockedCostumeIndex = 0;
        }
    }
}