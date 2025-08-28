using BepInEx;
using BepInEx.Configuration;
using BepInEx.NET.Common;
using BepInExResoniteShim;
using FrooxEngine;
using HarmonyLib;

namespace LocalMenu;

[ResonitePlugin(PluginMetadata.GUID, PluginMetadata.NAME, PluginMetadata.VERSION, PluginMetadata.AUTHORS, PluginMetadata.REPOSITORY_URL)]
[BepInDependency(BepInExResoniteShim.PluginMetadata.GUID, BepInDependency.DependencyFlags.HardDependency)]
public class Patch : BasePlugin 
{
    private static ConfigEntry<bool> HIDE_LASERS;
    private static ConfigEntry<bool> HIDE_CONTEXTMENU;

    public override void Load()
    {
        HIDE_LASERS = Config.Bind("General", "Hide Lasers", false, "Locally hide your lasers for everyone else");
        HIDE_CONTEXTMENU = Config.Bind("General", "Hide Context Menu", false, "Locally hide your context menu for everyone else.");

        HarmonyInstance.PatchAll();
    }

    [HarmonyPatch(typeof(ContextMenu))]
    class PatchContextMenu
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnAwake")]
        static void Postfix(ContextMenu __instance)
        {
            if (!HIDE_CONTEXTMENU.Value) return;

            __instance.RunInUpdates(3, () =>
            {
                if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser)
                    return;

                Slot slot = __instance.Slot;
                var temp = slot.AttachComponent<ValueUserOverride<bool>>();
                temp.Target.Value = slot.ActiveSelf_Field.ReferenceID;
                temp.PersistentOverrides.Value = false;
                temp.Default.Value = false;
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
            if (!HIDE_LASERS.Value) return;

            __instance.RunInUpdates(3, () =>
            {
                if (__instance.Slot.ActiveUserRoot.ActiveUser != __instance.LocalUser)
                    return;

                Slot slot = __instance.Slot;
                var temp = slot.AttachComponent<ValueUserOverride<bool>>();
                temp.Target.Value = slot.ActiveSelf_Field.ReferenceID;
                temp.PersistentOverrides.Value = false;
                temp.Default.Value = false;
                temp.SetOverride(__instance.LocalUser, true);
            });
        }
    }
}