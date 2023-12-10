using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
    class Jump_performed_Patch {
        [HarmonyPrefix]
        internal static bool Prefix( ref InputAction.CallbackContext context ) {
            Plugin.player.wishJump = true;

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

    [HarmonyPatch( typeof( HUDManager ), "SubmitChat_performed" )]
    class SubmitChat_performed_Patch {
        [HarmonyPrefix]
        internal static bool Prefix( HUDManager __instance ) {
            string text = __instance.chatTextField.text;

            if ( text.StartsWith( "/autobhop" ) ) {
                Plugin.cfg.autobhop = !Plugin.cfg.autobhop;
            } else if ( text.StartsWith( "/speedo" ) ) {
                Plugin.cfg.speedometer = !Plugin.cfg.speedometer;
            } else {
                return true;
            }

            __instance.localPlayer.isTypingChat = false;
            __instance.chatTextField.text = "";
            EventSystem.current.SetSelectedGameObject( null );
            __instance.PingHUDElement( __instance.Chat, 2f, 1f, 0.2f );
            __instance.typingIndicator.enabled = false;

            return false;
        }
    }
}
