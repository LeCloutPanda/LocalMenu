using BepInEx;
using BepInEx.Configuration;
using BepInExResoniteShim;
using FrooxEngine;
using HarmonyLib;

namespace LocalMenu;

[BepInPlugin(GUID, Name, Version)]
public class Patch : BaseResonitePlugin
{
    public const string GUID = "dev.lecloutpanda.localmenu";
    public const string Name = "Local Menu";
    public const string Version = "1.0.3";
    public override string Author => "LeCloutPanda";
    public override string Link => "https://github.com/LeCloutPanda/LocalMenu";

    private static ConfigEntry<bool> HIDE_LASERS;
    private static ConfigEntry<bool> HIDE_CONTEXTMENU;

    public override void Load()
    {
        HIDE_LASERS = Config.Bind("General", "Hide Lasers", true, "Locally hide your lasers for everyone else");
        HIDE_CONTEXTMENU = Config.Bind("General", "Hide Context Menu", true, "Locally hide your context menu for everyone else.");

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