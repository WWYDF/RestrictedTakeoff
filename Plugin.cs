using BepInEx;
using HarmonyLib;
using LC_API.Data;
using LC_API.GameInterfaceAPI;
using LC_API.GameInterfaceAPI.Features;
using UnityEngine;

namespace RestrictedTakeoff
{
    [BepInPlugin("net.devante.restrictedtakeoff", "RestrictedTakeoff", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("net.devante.restrictedtakeoff");

        public void Awake()
        {
            harmony.PatchAll();
            sendLogger("Plugin loaded. Original idea by retrotsunami", "info");
        }


        [HarmonyPatch(typeof(StartMatchLever))]
        internal class StartMatchLeverPatch
        {

            [HarmonyPatch("BeginHoldingInteractOnLever")]
            [HarmonyPostfix]
            private static void LeverPatch(ref StartMatchLever __instance)
            {
                if (GameState.ShipState != ShipState.OnMoon) // Prevent from applying to starting the level
                {
                    __instance.triggerScript.timeToHold = 0.5f;
                    return;
                }
                if (__instance.hasDisplayedTimeWarning) { return; }
                if (StartOfRound.Instance.currentLevel.PlanetName == "71 Gordion") { return; } // Prevent mod from applying on company building

                HUDManager.Instance.DisplayTip("ERROR!", "Ship is unable to leave manually, but it will automatically leave at midnight.", true, false, "RT_Warn1");
                __instance.hasDisplayedTimeWarning = true;
                __instance.triggerScript.timeToHold = 1000f;
            }
        }

        public void sendLogger(string msg, string type)
        {
            switch (type)
            {
                case "error":
                    Logger.LogError(msg); break;
                case "info":
                    Logger.LogInfo(msg); break;
                case "debug":
                    Logger.LogDebug(msg); break;
                case "warning":
                    Logger.LogWarning(msg); break;

            }
        }
    }
}