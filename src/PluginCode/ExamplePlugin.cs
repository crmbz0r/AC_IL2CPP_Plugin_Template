using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace AC_ExamplePlugin
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class ExamplePlugin : BasePlugin
    {
        /// <summary>
        /// Human-readable name of the plugin. In general, it should be short and concise.
        /// This is the name that is shown to the users who run BepInEx and to modders that inspect BepInEx logs. 
        /// </summary>
        public const string PluginName = "AC_ExamplePlugin";

        /// <summary>
        /// Unique ID of the plugin. Will be used as the default config file name.
        /// This must be a unique string that contains only characters a-z, 0-9 underscores (_) and dots (.)
        /// When creating Harmony patches or any persisting data, it's best to use this ID for easier identification.
        /// </summary>
        public const string GUID = "org.AC_ExamplePlugin";

        /// <summary>
        /// Version of the plugin. Must be in form <major>.<minor>.<build>.<revision>.
        /// Major and minor versions are mandatory, but build and revision can be left unspecified.
        /// </summary>
        public const string Version = "1.0.0";

        internal static BepInEx.Logging.ManualLogSource Logger = null!;

        private ConfigEntry<bool> _exampleConfigEntry = null!;

        public override void Load()
        {
            Logger = Log;

            _exampleConfigEntry = Config.Bind("General", "Enable this plugin", true, "If false, this plugin will do nothing");

            if (_exampleConfigEntry.Value)
            {
                Harmony.CreateAndPatchAll(typeof(Hooks), GUID);
            }
        }

        private static class Hooks
        {
            // Example Harmony patch for IL2CPP:
            // [HarmonyPostfix]
            // [HarmonyPatch(typeof(SomeGameClass), nameof(SomeGameClass.SomeMethod))]
            // private static void SomeMethodPostfix(SomeGameClass __instance)
            // {
            //     ...
            // }
        }
    }
}
