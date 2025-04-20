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
using Verse.AI;
namespace TribalPlayingEnhencementMod
{
    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.StartJob))]
    public static class Patch_DuelInterruptDetect
    {
        public static void Prefix(Job newJob, Pawn_JobTracker __instance)
        {
            if (newJob == null || newJob.targetA == null) return;
            if (newJob.def != JobDefOf.AttackMelee && newJob.def != JobDefOf.AttackStatic) return;

            Pawn target = newJob.targetA.Thing as Pawn;
            if (target == null) return;

            // Use Traverse to safely access the attacker (private field)
            Pawn attacker = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (attacker == null) return;

            if (target == CustomRaidEventSystem.currentEnemy &&
                attacker != CustomRaidEventSystem.currentColonist &&
                attacker.Faction == Faction.OfPlayer)
            {
                CustomRaidEventSystem.OnDuelInterrupted(attacker);
            }
        }
    }
}

