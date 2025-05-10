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
    // Initialize Harmony
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            Log.Message("[TribalMod] Initializing Harmony...");
            var harmony = new Harmony("tribal.enhance.mod");
            harmony.PatchAll();
            Log.Message("[TribalMod] Harmony patching complete.");
        }
    }
}
