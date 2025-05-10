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
    [HarmonyPatch(typeof(Pawn_DraftController), "Drafted", MethodType.Setter)]
    public static class Patch_BlockDuelistDrafting
    {
        public static bool Prefix(Pawn_DraftController __instance, bool value)
        {
            if (value == true) // only block attempts to draft
            {
                Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

                if (pawn != null &&
                    pawn.Faction == Faction.OfPlayer &&
                    pawn.health.hediffSet.HasHediff(HediffDef.Named("TribalMod_Duelist")))
                {
                    Log.Message($"[TribalMod] Blocked drafting duelist pawn: {pawn.Name}");
                    return false; // prevent draft toggle
                }
            }

            return true; // allow normal behavior
        }
    }
}
