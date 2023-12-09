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

            return false;
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
