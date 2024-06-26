﻿using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;

namespace LocalMenu
{
    public class Patch : ResoniteMod
    {
        public override string Name => "Local Menu";
        public override string Author => "LeCloutPanda";
        public override string Version => "1.0.2";
        public override string Link => "https://github.com/LeCloutPanda/LocalMenu/";

        public static ModConfiguration config;

        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> ENABLED = new ModConfigurationKey<bool>("Enabled", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> CONTEXT_MENU_VISIBLE = new ModConfigurationKey<bool>("Allow others to see your Context menu", "", () => true);
        [AutoRegisterConfigKey] private static ModConfigurationKey<bool> INTERACTION_LASER_VISIBLE = new ModConfigurationKey<bool>("Allow others to see your Interaction Lasers", "", () => true);

        public override void OnEngineInit()
        {
            config = GetConfiguration();
            config.Save(true);

            Harmony harmony = new Harmony($"dev.lecloutpanda.localmenu");
            harmony.PatchAll();

        }

        [HarmonyPatch(typeof(ContextMenu))]
        class PatchContextMenu
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnAwake")]
            static void Postfix(ContextMenu __instance)
            {
                if (!config.GetValue(ENABLED)) return;

                __instance.RunInUpdates(3, () =>
                {
                    Msg("Creating value user override");

                    if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser)
                        return;

                    Slot slot = __instance.Slot;
                    var temp = slot.AttachComponent<ValueUserOverride<bool>>();
                    temp.Target.Value = slot.ActiveSelf_Field.ReferenceID;
                    temp.PersistentOverrides.Value = false;
                    temp.Default.Value = config.GetValue(CONTEXT_MENU_VISIBLE);
                    temp.SetOverride(__instance.LocalUser, true);
                });
            }
        }

        [HarmonyPatch(typeof(InteractionLaser))]
        class PatchInteractionLaser
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnAwake")]
            static void Postfix(InteractionLaser __instance)
            {
                if (!config.GetValue(ENABLED)) return;

                __instance.RunInUpdates(3, () =>
                {
                    if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser)
                        return;

                    Slot slot = __instance.Slot;
                    var temp = slot.AttachComponent<ValueUserOverride<bool>>();
                    temp.Target.Value = slot.ActiveSelf_Field.ReferenceID;
                    temp.PersistentOverrides.Value = false;
                    temp.Default.Value = config.GetValue(INTERACTION_LASER_VISIBLE);
                    temp.SetOverride(__instance.LocalUser, true);
                });
            }
        }
    }
}