using BepInEx;
using BepInEx.Logging;

using GameNetcodeStuff;

using HarmonyLib;

using UnityEngine;
using UnityEngine.InputSystem;

namespace lcbhop {
    [HarmonyPatch( typeof( CharacterController ), "Move" )]
    class Move_Patch {
        [HarmonyPrefix]
        internal static bool Prefix( ref Vector3 motion ) {
            return !Plugin.patchMove;
        }
    }

    [HarmonyPatch( typeof( PlayerControllerB ), "Crouch_performed" )]
    class Crouch_performed_Patch {
        [HarmonyPrefix]
        internal static bool Prefix( PlayerControllerB __instance, ref InputAction.CallbackContext context ) {
            if ( !context.performed ) {
                return false;
            }
            if ( __instance.quickMenuManager.isMenuOpen ) {
                return false;
            }
            if ( ( !__instance.IsOwner || !__instance.isPlayerControlled || ( __instance.IsServer && !__instance.isHostPlayerObject ) ) && !__instance.isTestingPlayer ) {
                return false;
            }
            if ( __instance.inSpecialInteractAnimation || __instance.isTypingChat ) {
                return false;
            }

            __instance.Crouch( !__instance.isCrouching );

            // Player height fix for non-host
            if ( __instance.isCrouching ) {
                __instance.thisController.center = new Vector3( __instance.thisController.center.x, 0.72f, __instance.thisController.center.z );
                __instance.thisController.height = 1.5f;
            } else {
                __instance.thisController.center = new Vector3( __instance.thisController.center.x, 1.28f, __instance.thisController.center.z );
                __instance.thisController.height = 2.5f;
            }

            return false;
        }
    }

    public class ComponentAdder : MonoBehaviour {
        void Update( ) {
            foreach ( PlayerControllerB playerControllerB in UnityEngine.Object.FindObjectsOfType<PlayerControllerB>( ) ) {
                if ( playerControllerB != null && playerControllerB.gameObject.GetComponentInChildren<CPMPlayer>( ) == null && playerControllerB.IsOwner && playerControllerB.isPlayerControlled ) {
                    playerControllerB.gameObject.AddComponent<CPMPlayer>( ).player = playerControllerB;
                }
            }
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
