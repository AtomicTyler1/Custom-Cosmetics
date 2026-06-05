using HarmonyLib;
using System.Linq;
using UnityEngine.EventSystems;

namespace Custom_Cosmetics.Patches
{
    [HarmonyPatch]
    public static class SelectionFix
    {
        private static void EnsureCustomCostumesUnlocked(PlayerCostumeManager manager)
        {
            if (manager == null) return;

            var indices = manager.unlockedCostumeIndicies.ToList();
            bool modified = false;

            for (int i = 0; i < manager.costumes.Length; i++)
            {
                if (manager.costumes[i].name.StartsWith("custom-costume-") && !indices.Contains(i))
                {
                    indices.Add(i);
                    modified = true;
                }
            }

            if (modified)
            {
                manager.unlockedCostumeIndicies = indices.ToArray();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CostumeChangeUI), nameof(CostumeChangeUI.OnSelect))]
        public static void OnSelectPrefix()
        {
            if (GameUtil.TryGetLocalPlayer(out var player))
            {
                EnsureCustomCostumesUnlocked(player.GetObject<PlayerCostumeManager>());
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CostumeChangeUI), nameof(CostumeChangeUI.Cycle))]
        public static void BeforeCycle()
        {
            if (!GameUtil.TryGetLocalPlayer(out var player)) return;

            var manager = player.GetObject<PlayerCostumeManager>();
            if (manager == null) return;

            EnsureCustomCostumesUnlocked(manager);

            int max = manager.unlockedCostumeIndicies.Length - 1;
            if (manager.currentUnlockedCostumeIndex > max)
                manager.currentUnlockedCostumeIndex = max;
            if (manager.currentUnlockedCostumeIndex < 0)
                manager.currentUnlockedCostumeIndex = 0;
        }
    }
}