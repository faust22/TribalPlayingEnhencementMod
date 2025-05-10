using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using HugsLib;
using HarmonyLib;
using Verse.AI.Group;
namespace TribalPlayingEnhencementMod
{
    // Intercept the raid before it actually spawns anything
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker")]
    public static class Patch_InterceptRaid
    {
        [HarmonyPrefix]
        public static bool Prefix(IncidentParms parms, ref bool __result)
        {
            Log.Message("[TribalMod] Prefix: TryExecuteWorker (Raid) intercepted.");

            Map map = parms.target as Map;
            if (map == null)
            {
                Log.Warning("[TribalMod] Raid target was not a map. Skipping.");
                __result = false;
                return false; // cancel
            }

            Log.Message($"[TribalMod] Intercepted raid from faction '{parms.faction?.Name}' on map '{map}'.");

            // Raise your custom event instead of spawning pawns
            CustomRaidEventSystem.OnRaidStarted(map, new List<Pawn>(), parms);

            // Cancel the raid execution
            __result = true;  // returning "true" here tells RimWorld "this raid was handled"
            return false;     // prevents the base method from running
        }
    }
}
