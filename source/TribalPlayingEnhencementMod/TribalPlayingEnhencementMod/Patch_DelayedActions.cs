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
    [StaticConstructorOnStartup]
    public static class DelayedActions
    {
        private static List<DelayedAction> queue = new List<DelayedAction>();

        static DelayedActions()
        {
            GameComponentUtil.OnGameTick += OnTick;
        }

        public static void AddDelayedAction(Map map, int delayTicks, System.Action action)
        {
            queue.Add(new DelayedAction { remainingTicks = delayTicks, action = action, map = map });
        }

        private static void OnTick()
        {
            for (int i = queue.Count - 1; i >= 0; i--)
            {
                queue[i].remainingTicks--;
                if (queue[i].remainingTicks <= 0)
                {
                    queue[i].action?.Invoke();
                    queue.RemoveAt(i);
                }
            }
        }

        private class DelayedAction
        {
            public int remainingTicks;
            public System.Action action;
            public Map map;
        }
    }
}
