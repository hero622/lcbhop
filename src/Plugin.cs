using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using UnityEngine;

namespace lcbhop {
    [HarmonyPatch( typeof( CharacterController ), "Move" )]
    class CharacterController_Patch {
        [HarmonyPrefix]
        internal static bool Prefix( ref Vector3 motion ) {
            return !Plugin.patchMove;
        }
    }

    [BepInPlugin( MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION )]
    public class Plugin : BaseUnityPlugin {
        public static ManualLogSource logger;

        public static bool patchMove = true;

        readonly Harmony harmony = new Harmony( MyPluginInfo.PLUGIN_GUID );

        void Awake( ) {
            // Plugin startup logic
            Logger.LogInfo( $"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!" );

            logger = Logger;

            harmony.PatchAll( );
        }

        void OnDestroy( ) {
            GameObject gameObject = new GameObject( "ComponentAdder" );
            UnityEngine.Object.DontDestroyOnLoad( gameObject );
            gameObject.AddComponent<ComponentAdder>( );
        }
    }
}
