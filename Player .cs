using UnityEngine;
using Infra;
using Infra.Gameplay;
using Infra.Utils;

namespace Gadget {

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(OverlapChecker))]

public class Player : MonoBehaviour {
    public int health = 1;
    public float cubeJumpMovmentVec = 15f;
    public float cubeRunMovmentVec = 7f;
    public float cubeGravity = 2f;
    public float armAngle = 45f;
    public float bulletspeed = 15f;

    public KeyCode Right;
    public KeyCode left;
    public KeyCode jump;
    public KeyCode shoot;


    public Transform armMovment;
    public Transform cubeMovement;
    public Rigidbody2D BulletPhysics;

    private readonly int isAlive = Animator.StringToHash("Alive");
    private readonly int isJumping = Animator.StringToHash("Jump");

    private Animator playerAnimator;
    private Rigidbody2D cubePhysics;
    private OverlapChecker playerOverlapChecker;


    private bool noCollision {
        get {
            return playerOverlapChecker.isOverlapping;
        }
    }


    protected void Awake() {
        playerAnimator = GetComponent<Animator>();
        cubePhysics = GetComponent<Rigidbody2D>();
        playerOverlapChecker = GetComponent<OverlapChecker>();

        playerAnimator.SetBool(isAlive, true);
    }


    protected void Update() {
		//update arm vector
		var currentArmVector = armMovment.eulerAngles;
        currentArmVector.z = armAngle;
        armMovment.eulerAngles = currentArmVector;
		//update cube parameters: movement vectors in each direction
        cubePhysics.gravityScale = cubeGravity;
		
		var cubeCurrentVelocity = cubePhysics.velocity;
        if (Input.GetKeyDown(jump) && noCollision) {
            cubeCurrentVelocity.y = cubeJumpMovmentVec;
            cubePhysics.velocity = cubeCurrentVelocity;
            playerAnimator.SetTrigger(isJumping);
        } else if (Input.GetKey(Right)) {
            cubeCurrentVelocity.x = cubeRunMovmentVec;
            cubePhysics.velocity = cubeCurrentVelocity;
        } else if (Input.GetKey(left)) {
            cubeCurrentVelocity.x = -cubeRunMovmentVec;
            cubePhysics.velocity = cubeCurrentVelocity;
        } else if (Input.GetKey(shoot)) {
            if (!BulletPhysics.gameObject.activeInHierarchy) {
                BulletPhysics.gameObject.SetActive(true);
                BulletPhysics.position = cubeMovement.position;
                BulletPhysics.velocity = Vector2.right.Rotate(Mathf.Deg2Rad * armAngle) * bulletspeed;
            }
        }
    }
	//checks for different types of cillions
    protected void OnCollisionEnter2D(Collision2D collision) {
		//wall collision
        if (health <= 0) return;
		//if won the game
        if (collision.gameObject.CompareTag("Victory")) {
            DebugUtils.Log("Great Job!");
            return;
        }
		//enemey colliosion
        if (!collision.gameObject.CompareTag("Enemy")) return;

        --health;
        if (health > 0) return;
		//checks is alive
        playerAnimator.SetBool(isAlive, false);
        cubePhysics.velocity = Vector2.zero;
        cubePhysics.gravityScale = 4f;
        enabled = false;
    }
}
}
