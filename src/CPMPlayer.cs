// https://github.com/WiggleWizard/quake3-movement-unity3d/blob/master/CPMPlayer.cs

using System.Collections;
using System.Collections.Generic;

using GameNetcodeStuff;

using lcbhop;

using UnityEngine;

// Contains the command the user wishes upon the character
struct Cmd {
    public float forwardMove;
    public float rightMove;
    public float upMove;
}

public class CPMPlayer : MonoBehaviour {
    /*Frame occuring factors*/
    public float gravity = 20.0f;

    public float friction = 6; //Ground friction

    /* Movement stuff */
    public float moveSpeed = 7.0f;                // Ground move speed
    public float runAcceleration = 14.0f;         // Ground accel
    public float runDeacceleration = 10.0f;       // Deacceleration that occurs when running on the ground
    public float airAcceleration = 2.0f;          // Air accel
    public float airDecceleration = 2.0f;         // Deacceleration experienced when ooposite strafing
    public float airControl = 0.3f;               // How precise air control is
    public float sideStrafeAcceleration = 50.0f;  // How fast acceleration occurs to get up to sideStrafeSpeed when
    public float sideStrafeSpeed = 1.0f;          // What the max speed to generate when side strafing
    public float jumpSpeed = 8.0f;                // The speed at which the character's up axis gains when hitting jump
    public bool holdJumpToBhop = true;            // When enabled allows player to just hold jump button to keep on bhopping perfectly. Beware: smells like casual.

    public PlayerControllerB player;
    private CharacterController _controller;

    private Vector3 moveDirectionNorm = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;

    private bool wishJump = false;

    // Used to display real time fricton values
    private float playerFriction = 0.0f;

    // Player commands, stores wish commands that the player asks for (Forward, back, jump, etc)
    private Cmd _cmd;

    private void Start( ) {
        _controller = player.thisController;
    }

    private void Update( ) {
        if ( ( !player.IsOwner || !player.isPlayerControlled || ( player.IsServer && !player.isHostPlayerObject ) ) && !player.isTestingPlayer ) {
            return;
        }
        if ( player.quickMenuManager.isMenuOpen || player.inSpecialInteractAnimation || player.isTypingChat ) {
            return;
        }

        // Don't patch movement on ladders
        if ( player.isClimbingLadder ) {
            Plugin.patchMove = false;
            return;
        }

        // Allow crouching while mid air, hopefully doesn't cause any side effects
        player.fallValue = 0.0f;

        /* Movement, here's the important part */
        QueueJump( );
        if ( _controller.isGrounded )
            GroundMove( );
        else if ( !_controller.isGrounded )
            AirMove( );

        // Move the controller
        Plugin.patchMove = false; // Disable the Move Patch
        _controller.Move( playerVelocity * Time.deltaTime );
        Plugin.patchMove = true; // Reenable the Move Patch
    }

    /*******************************************************************************************************\
   |* MOVEMENT
   \*******************************************************************************************************/

    /**
     * Sets the movement direction based on player input
     */
    private void SetMovementDir( ) {
        _cmd.forwardMove = player.playerActions.Movement.Move.ReadValue<Vector2>( ).y;
        _cmd.rightMove = player.playerActions.Movement.Move.ReadValue<Vector2>( ).x;
    }

    /**
     * Queues the next jump just like in Q3
     */
    private void QueueJump( ) {
        if ( holdJumpToBhop ) {
            wishJump = player.playerActions.Movement.Jump.ReadValue<float>( ) > 0.0f;
            return;
        }
    }

    /**
     * Execs when the player is in the air
    */
    private void AirMove( ) {
        Vector3 wishdir;
        float wishvel = airAcceleration;
        float accel;

        SetMovementDir( );

        wishdir = new Vector3( _cmd.rightMove, 0, _cmd.forwardMove );
        wishdir = transform.TransformDirection( wishdir );

        float wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        wishdir.Normalize( );
        moveDirectionNorm = wishdir;

        // CPM: Aircontrol
        float wishspeed2 = wishspeed;
        if ( Vector3.Dot( playerVelocity, wishdir ) < 0 )
            accel = airDecceleration;
        else
            accel = airAcceleration;
        // If the player is ONLY strafing left or right
        if ( _cmd.forwardMove == 0 && _cmd.rightMove != 0 ) {
            if ( wishspeed > sideStrafeSpeed )
                wishspeed = sideStrafeSpeed;
            accel = sideStrafeAcceleration;
        }

        Accelerate( wishdir, wishspeed, accel );
        if ( airControl > 0 )
            AirControl( wishdir, wishspeed2 );
        // !CPM: Aircontrol

        // Apply gravity
        playerVelocity.y -= gravity * Time.deltaTime;
    }

    /**
     * Air control occurs when the player is in the air, it allows
     * players to move side to side much faster rather than being
     * 'sluggish' when it comes to cornering.
     */
    private void AirControl( Vector3 wishdir, float wishspeed ) {
        float zspeed;
        float speed;
        float dot;
        float k;

        // Can't control movement if not moving forward or backward
        if ( Mathf.Abs( _cmd.forwardMove ) < 0.001 || Mathf.Abs( wishspeed ) < 0.001 )
            return;
        zspeed = playerVelocity.y;
        playerVelocity.y = 0;
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        speed = playerVelocity.magnitude;
        playerVelocity.Normalize( );

        dot = Vector3.Dot( playerVelocity, wishdir );
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if ( dot > 0 ) {
            playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

            playerVelocity.Normalize( );
            moveDirectionNorm = playerVelocity;
        }

        playerVelocity.x *= speed;
        playerVelocity.y = zspeed; // Note this line
        playerVelocity.z *= speed;
    }

    /**
     * Called every frame when the engine detects that the player is on the ground
     */
    private void GroundMove( ) {
        Vector3 wishdir;

        // Do not apply friction if the player is queueing up the next jump
        if ( !wishJump )
            ApplyFriction( 1.0f );
        else
            ApplyFriction( 0 );

        SetMovementDir( );

        wishdir = new Vector3( _cmd.rightMove, 0, _cmd.forwardMove );
        wishdir = transform.TransformDirection( wishdir );
        wishdir.Normalize( );
        moveDirectionNorm = wishdir;

        var wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        Accelerate( wishdir, wishspeed, runAcceleration );

        // Reset the gravity velocity
        playerVelocity.y = -gravity * Time.deltaTime;

        if ( wishJump ) {
            playerVelocity.y = jumpSpeed;
            wishJump = false;
        }
    }

    /**
     * Applies friction to the player, called in both the air and on the ground
     */
    private void ApplyFriction( float t ) {
        Vector3 vec = playerVelocity; // Equivalent to: VectorCopy();
        float speed;
        float newspeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        /* Only if the player is on the ground then apply friction */
        if ( _controller.isGrounded ) {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * friction * Time.deltaTime * t;
        }

        newspeed = speed - drop;
        playerFriction = newspeed;
        if ( newspeed < 0 )
            newspeed = 0;
        if ( speed > 0 )
            newspeed /= speed;

        playerVelocity.x *= newspeed;
        playerVelocity.z *= newspeed;
    }

    private void Accelerate( Vector3 wishdir, float wishspeed, float accel ) {
        float addspeed;
        float accelspeed;
        float currentspeed;

        currentspeed = Vector3.Dot( playerVelocity, wishdir );
        addspeed = wishspeed - currentspeed;
        if ( addspeed <= 0 )
            return;
        accelspeed = accel * Time.deltaTime * wishspeed;
        if ( accelspeed > addspeed )
            accelspeed = addspeed;

        playerVelocity.x += accelspeed * wishdir.x;
        playerVelocity.z += accelspeed * wishdir.z;
    }
}
