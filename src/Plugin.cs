using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace SelectMiscreationFeat;

public static class ModInfo
{
    public const string Guid = "SelectMiscreationFeat";
    public const string Name = "Select Miscreation Feat";
    public const string Version = "1.0.1";
}

[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
internal class Plugin : BaseUnityPlugin
{
    internal static Plugin? Instance;
    internal ConfigEntry<bool>? EnableMod;
    internal ConfigEntry<bool>? EnableRandom;
    internal ConfigEntry<int>? ChoiceCount;
    internal ConfigEntry<int>? WindowWidth;

    private void Awake()
    {
        Instance = this;
        EnableMod = Config.Bind("General", "EnableMod", true, "Enable this mod.");
        EnableRandom = Config.Bind("General", "Enable Random", false, "The seed for gene generation will be completely random and not based on character ID. ");
        ChoiceCount = Config.Bind("General", "Choice Count", 10, "The number of choices to display (Max: 1000)");
        WindowWidth = Config.Bind("General", "Window Width", 1000, "Width of the selection list (Adjust this if the text overflows.)");
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModInfo.Guid);
    }

    internal static void LogDebug(object message, [CallerMemberName] string caller = "")
    {
        Instance?.Logger.LogDebug($"[{caller}] {message}");
    }

    internal static void LogInfo(object message)
    {
        Instance?.Logger.LogInfo(message);
    }

    internal static void LogError(object message)
    {
        Instance?.Logger.LogError(message);
    }
}
