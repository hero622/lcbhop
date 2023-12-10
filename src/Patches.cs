using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;

namespace lcbhop {
    [HarmonyPatch( typeof( CharacterController ), "Move" )]
    class Move_Patch {
        [HarmonyPrefix]
        internal static bool Prefix( ref Vector3 motion ) {
            // Patch game movement when not called by us
            return !Plugin.patchMove;
        }
    }

    [HarmonyPatch( typeof( PlayerControllerB ), "Crouch_performed" )]
    class Crouch_performed_Patch {
        [HarmonyPrefix]
        internal static bool Prefix( PlayerControllerB __instance, ref InputAction.CallbackContext context ) {
            // Patch not being able to crouch when jumping

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

    [HarmonyPatch( typeof( PlayerControllerB ), "Jump_performed" )]
    class SJump_performed_Patch {
        [HarmonyPrefix]
        internal static bool Prefix( ref InputAction.CallbackContext context ) {
            // Patch jumping animation, we call it on our own
            return !Plugin.patchJump;
        }
    }

    [HarmonyPatch( typeof( PlayerControllerB ), "ScrollMouse_performed" )]
    class ScrollMouse_performed_Patch {
        [HarmonyPrefix]
        internal static bool Prefix( ref InputAction.CallbackContext context ) {
            // Patch scrolling in the hotbar if not autobhopping
            return Plugin.cfg.autobhop;
        }
    }
}
