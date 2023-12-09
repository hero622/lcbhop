using UnityEngine;
using GameNetcodeStuff;

namespace lcbhop {
    public class ComponentAdder : MonoBehaviour {
        void Update( ) {
            foreach ( PlayerControllerB playerControllerB in UnityEngine.Object.FindObjectsOfType<PlayerControllerB>( ) ) {
                if ( playerControllerB != null && playerControllerB.gameObject.GetComponentInChildren<CPMPlayer>( ) == null && playerControllerB.IsOwner && playerControllerB.isPlayerControlled ) {
                    playerControllerB.gameObject.AddComponent<CPMPlayer>( ).player = playerControllerB;
                }
            }
        }
    }
}
