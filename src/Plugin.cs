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
    public const string Version = "1.0.0";
}

[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
internal class Plugin : BaseUnityPlugin
{
    internal static Plugin? Instance;
    internal ConfigEntry<bool>? EnableRandom;
    internal ConfigEntry<int>? ChoiceCount;

    private void Awake()
    {
        Instance = this;
        EnableRandom = Config.Bind("General", "EnableRandom", false, "Enable random gene");
        ChoiceCount = Config.Bind("General", "ChoiceCount", 20, "The number of choices to display");
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