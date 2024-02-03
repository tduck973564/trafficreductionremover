using System;
using System.Collections.Generic;
using System.Reflection;
using Colossal.Logging;
using HarmonyLib;
using Game.Prefabs;
using Unity.Entities;

namespace TrafficReductionRemover.Patches 
{
    [HarmonyPatch(typeof(PrefabSystem), "AddPrefab")]
    class PrefabSystem_AddPrefabPatch
    {
        static void Prefix(PrefabSystem __instance, PrefabBase prefab)
        {
            if (prefab is EconomyPrefab)
            {
                FieldInfo trafficReduction = AccessTools.Field(typeof(Game.Prefabs.EconomyPrefab), "m_TrafficReduction");
                trafficReduction.SetValue(prefab, 0f);
                Console.WriteLine("Added m_TrafficReduction value");
            }
        }
    }

    // This shouldn't be necessary, and probably isn't a good idea, but in case it is required I am leaving it
    [HarmonyPatch(typeof(PrefabSystem), "UpdatePrefabs")]
    class PrefabSystem_UpdatePrefabPatch
    {
        static void Prefix(PrefabSystem __instance)
        {
            Dictionary<PrefabBase, Entity> updateMap = Traverse.Create(__instance).Field("m_UpdateMap")
                .GetValue<Dictionary<PrefabBase, Entity>>();
            
            foreach (KeyValuePair<PrefabBase, Entity> item in updateMap)
            {
                if (item.Key is EconomyPrefab)
                {
                    FieldInfo trafficReduction =
                        AccessTools.Field(typeof(Game.Prefabs.EconomyPrefab), "m_TrafficReduction");
                    
                    trafficReduction.SetValue(item.Key, 0f);
                    Traverse.Create(__instance).Field("m_UpdateMap").SetValue(updateMap);
                    Console.WriteLine("Updated m_TrafficReduction value");
                    break;
                }
            }
        }
    }
}