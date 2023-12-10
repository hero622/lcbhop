using BepInEx;
using BepInEx.Logging;

using GameNetcodeStuff;

using HarmonyLib;

using UnityEngine;

namespace lcbhop {
    public class ComponentAdder : MonoBehaviour {
        void Update( ) {
            foreach ( PlayerControllerB playerControllerB in UnityEngine.Object.FindObjectsOfType<PlayerControllerB>( ) ) {
                if ( playerControllerB != null && playerControllerB.gameObject.GetComponentInChildren<CPMPlayer>( ) == null && playerControllerB.IsOwner && playerControllerB.isPlayerControlled ) {
                    Plugin.player = playerControllerB.gameObject.AddComponent<CPMPlayer>( );
                    Plugin.player.player = playerControllerB;
                }
            }
        }
    }

    [BepInPlugin( MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION )]
    public class Plugin : BaseUnityPlugin {
        public static ManualLogSource logger;
        private readonly Harmony harmony = new Harmony( MyPluginInfo.PLUGIN_GUID );

        public static Config cfg { get; set; }

        public static bool patchMove = true;
        public static bool patchJump = true;

        public static CPMPlayer player;

        void Awake( ) {
            cfg = new Config( Config );
            cfg.Init( );

            logger = Logger;

            harmony.PatchAll( );

            // Plugin startup logic
            Logger.LogInfo( $"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!" );
        }

        void OnDestroy( ) {
            GameObject gameObject = new GameObject( "ComponentAdder" );
            UnityEngine.Object.DontDestroyOnLoad( gameObject );
            gameObject.AddComponent<ComponentAdder>( );
        }
    }
}
