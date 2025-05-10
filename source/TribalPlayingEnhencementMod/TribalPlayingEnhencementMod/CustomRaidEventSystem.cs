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
    // Custom event entry point
    public static class CustomRaidEventSystem
    {
        public static Pawn currentColonist;
        public static Pawn currentEnemy;

        public static void OnRaidStarted(Map map, List<Pawn> attackers, IncidentParms parms)
        {
            Log.Message($"[TribalMod] CustomRaidEventSystem: dummy raid intercepted for faction '{parms.faction?.Name}' on map '{map}'.");

            // Step 1: Create a fake raider
            //PawnKindDef kind = PawnKindDefOf.RaidFriendly; // or use PawnKindDefOf.SpaceRefugee or similar
            PawnKindDef kind = PawnKindDefOf.SpaceRefugee;
            Faction enemyFaction = parms.faction ?? Find.FactionManager.RandomEnemyFaction();

            Pawn enemy = PawnGenerator.GeneratePawn(new PawnGenerationRequest(
                kind,
                enemyFaction,
                PawnGenerationContext.NonPlayer,
                -1, allowDowned: false
            ));

            GenSpawn.Spawn(enemy, CellFinder.RandomClosewalkCellNear(map.Center, map, 10), map);
            Log.Message($"[TribalMod] Spawned dummy enemy pawn: {enemy.Name}");

            // Step 2: Select a colonist
            //Pawn colonist = map.mapPawns.FreeColonistsSpawned.RandomElementOrDefault();
            Pawn colonist = map.mapPawns.FreeAdultColonistsSpawned.RandomElement();
            if (colonist == null)
            {
                Log.Warning("[TribalMod] No colonist found to participate in event.");
                return;
            }

            Log.Message($"[TribalMod] Selected colonist for event: {colonist.Name}");
            currentColonist = colonist;
            currentEnemy = enemy;

            // Step 3: Choose meeting point
            IntVec3 duelSpot = CellFinder.RandomClosewalkCellNear(map.Center, map, 8);
            Log.Message($"[TribalMod] Duel between {enemy.Name} and {colonist.Name} at {duelSpot}");

            // Apply "duelist" hediff to prevent drafting (soft flag)
            Hediff duelHediff = HediffMaker.MakeHediff(HediffDef.Named("TribalMod_Duelist"), colonist);
            colonist.health.AddHediff(duelHediff);

            // Make both go to the spot, then fight
            colonist.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Goto, duelSpot));
            enemy.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Goto, duelSpot));

            // Wait briefly, then start melee fight
            DelayedActions.AddDelayedAction(map, 120, () =>
            {
                StartDuel(colonist, enemy);
            });
        }
        private static void StartDuel(Pawn colonist, Pawn enemy)
        {
            if (colonist.Dead || enemy.Dead) return;

            Log.Message($"[TribalMod] Duel begins: {colonist.Name} vs {enemy.Name}");

            // Force both to melee each other
            Job job1 = JobMaker.MakeJob(JobDefOf.AttackMelee, enemy);
            job1.expiryInterval = 99999;
            job1.killIncappedTarget = true;

            Job job2 = JobMaker.MakeJob(JobDefOf.AttackMelee, colonist);
            job2.expiryInterval = 99999;
            job2.killIncappedTarget = true;

            colonist.jobs.StopAll();
            colonist.jobs.TryTakeOrderedJob(job1, JobTag.Misc);

            enemy.jobs.StopAll();
            enemy.jobs.TryTakeOrderedJob(job2, JobTag.Misc);
        }
        public static void OnDuelInterrupted(Pawn attacker)
        {
            Log.Warning($"[TribalMod] Duel was interrupted by {attacker.Name}!");

            // You can add consequences here: punish attacker, end duel, etc.
        }
    }
}