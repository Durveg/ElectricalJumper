using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private AudioSource jumpSound;

	[Header("Player Data")]
	[SerializeField]
	private PlayerData playerData;

	[Header("Movement speed / timings")]
	[SerializeField]
	private float jumpHeight = 3;
	[SerializeField]
	private float timeToJumpPeak = 0.3f;
	[SerializeField]
	private float moveSpeed = 6;
	[SerializeField]
	private float throwItemVelocity = 20;
	[SerializeField]
	private float throwItemDisplacementHeight = 0.75f;
	[SerializeField]
	private float wallSlideSpeedMax = 3;
	[SerializeField]
	private float wallStickTime = 0.2f;
	private float timeToWallUnstick;

	[Header("Wall Jump Velocities")]
	[SerializeField]
	private Vector2 wallJumpClimb;
	[SerializeField]
	private Vector2 wallJumpOff;
	[SerializeField]
	private Vector2 wallLeap;

	[Header("Item hold local positions")]
	[SerializeField]
	private Vector2 heldPosition;

	private float gravity;
	private float jumpVelocity;

	private float velocityXSmoothing;
	private float accelerationTimeGrounded = 0.1f;
	private float accelerationTimeAirborn = 0.2f;

	private Vector3 velocity;

	private Controller2D controller;

	// Use this for initialization
	void Start () {

		this.controller = GetComponent<Controller2D> ();

		gravity = -1 * (2 * jumpHeight) / Mathf.Pow (timeToJumpPeak, 2);
		jumpVelocity = Mathf.Abs (gravity) * timeToJumpPeak;

	}
	
	// Update is called once per frame
	void Update () {

		playerData.input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		Vector2 input = playerData.input;

		int wallDirX = (controller.collisions.left ? -1 : 1);

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborn);

		bool wallSlinding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {

			wallSlinding = true;
			if (velocity.y < -wallSlideSpeedMax) {

				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {

				velocityXSmoothing = 0;
				velocity.x = 0;

				if (input.x != wallDirX && input.x != 0) {
				
					timeToWallUnstick -= Time.deltaTime;
				} else {

					timeToWallUnstick = wallStickTime;
				}
			} else {

				timeToWallUnstick = wallStickTime;
			}
		}

		if (controller.collisions.above || controller.collisions.below) {

			velocity.y = 0;
		}

		if (Input.GetKeyDown(KeyCode.Space)) {

			this.jumpSound.Play ();
			if (wallSlinding) {

				if (wallDirX == input.x) {
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				} else if (input.x == 0) {

					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;
				} else {

					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
				}
			}
			else if (controller.collisions.below) {
				velocity.y = jumpVelocity;
			}
			else if(this.playerData.holdingGem == true) {

				this.jumpSound.Play ();

				Vector2 throwPosition = new Vector2 (this.playerData.boardColumn.transform.position.x, this.transform.position.y);
				this.transform.position = new Vector2 (this.transform.position.x, this.transform.position.y + throwItemDisplacementHeight);
				velocity.y = throwItemVelocity;

				this.playerData.ThrowItem().ThrowDownwards (throwPosition);
			}
		}

		if (Input.GetKeyDown (KeyCode.LeftShift)) {

			if (playerData.highlightedGem != null && this.playerData.holdingGem == true) {

				Gem held = this.playerData.ThrowItem();
				Vector3 standingGemPosition = playerData.highlightedGem.transform.position;
				held.PlaceGem(standingGemPosition);

				playerData.highlightedGem.PickUp (this.transform, this.heldPosition);
				this.playerData.HoldItem (playerData.highlightedGem);
			} 
			else if(playerData.highlightedGem != null){ 

				playerData.highlightedGem.PickUp (this.transform, this.heldPosition);
				this.playerData.HoldItem (playerData.highlightedGem);
			}
		}
			
		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	}
}
