using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Custom_Cosmetics.Helpers;

public class AddCostumeToArray
{
    public static void AddCostume(PlayerCostumeManager manager, PlayerCostumeManager.Costume costume)
    {
        if (manager == null) return;

        var list = manager.costumes.ToList();
        list.Add(costume);
        manager.costumes = list.ToArray();

        int newIndex = manager.costumes.Length - 1;
        if (!manager.unlockedCostumeIndicies.Contains(newIndex))
        {
            var indices = manager.unlockedCostumeIndicies.ToList();
            indices.Add(newIndex);
            manager.unlockedCostumeIndicies = indices.ToArray();
        }
    }
}