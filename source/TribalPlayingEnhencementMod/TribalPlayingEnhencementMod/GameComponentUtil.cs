
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
    public class GameComponentUtil : GameComponent
    {
        public static event System.Action OnGameTick;

        public GameComponentUtil(Game _) { }

        public override void GameComponentTick()
        {
            OnGameTick?.Invoke();

            // Check if duel ended (colonist or enemy downed/dead)
            if (CustomRaidEventSystem.currentEnemy != null &&
                (CustomRaidEventSystem.currentEnemy.Dead || CustomRaidEventSystem.currentEnemy.Downed))
            {
                EndDuel();
            }

            if (CustomRaidEventSystem.currentColonist != null &&
                (CustomRaidEventSystem.currentColonist.Dead || CustomRaidEventSystem.currentColonist.Downed))
            {
                EndDuel();
            }

            void EndDuel()
            {
                Log.Message("[TribalMod] Duel has ended. Cleaning up...");

                if (CustomRaidEventSystem.currentColonist != null)
                {
                    var hediff = CustomRaidEventSystem.currentColonist.health.hediffSet
                        .GetFirstHediffOfDef(HediffDef.Named("TribalMod_Duelist"));
                    if (hediff != null)
                        CustomRaidEventSystem.currentColonist.health.RemoveHediff(hediff);
                }

                // Reset references
                CustomRaidEventSystem.currentColonist = null;
                CustomRaidEventSystem.currentEnemy = null;
            }
        }
    }
}