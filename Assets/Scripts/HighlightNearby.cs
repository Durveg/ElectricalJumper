using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HighlightNearby : MonoBehaviour {

	[SerializeField]
	private PlayerData playerData;

	[SerializeField]
	private LayerMask collisionMask;
	[SerializeField]
	private float raySize = 0.75f;

	const float skinWidth = 0.015f;

	private new BoxCollider2D collider;
	private RaycastOrigins raycastOrigins;
	private Controller2D controller;

	// Use this for initialization
	void Start () {

		this.controller = GetComponent<Controller2D> ();
		this.collider = this.GetComponent<BoxCollider2D>();
	}

	private RaycastHit2D LeftCollision(float raySize) {

		Vector2 rayOrigin = raycastOrigins.middleLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.left, raySize, collisionMask);

		Debug.DrawRay (rayOrigin, Vector2.left * raySize, Color.blue);

		return hit;
	}

	private RaycastHit2D RightCollision(float raySize) {
		
		Vector2 rayOrigin = raycastOrigins.middleRight;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right, raySize, collisionMask);

		Debug.DrawRay (rayOrigin, Vector2.right * raySize, Color.blue);

		return hit;
	}

	private RaycastHit2D DownCollision(float raySize) {

		Vector2 rayOrigin = raycastOrigins.bottomMiddle;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.down, raySize, collisionMask);

		Debug.DrawRay (rayOrigin, Vector2.down * raySize, Color.blue);

		return hit;
	}

	private void UpdateRaycastOrigins() {

		Bounds bounds = this.collider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.middleLeft = new Vector2(bounds.min.x,  bounds.center.y);
		raycastOrigins.middleRight = new Vector2(bounds.min.x,  bounds.center.y);

		raycastOrigins.bottomMiddle = new Vector2 (bounds.center.x, bounds.min.y);
	}

	// Update is called once per frame
	void Update () {
	
		UpdateRaycastOrigins ();

		RaycastHit2D left = LeftCollision(raySize);
		RaycastHit2D right = RightCollision(raySize);
		RaycastHit2D down = DownCollision(raySize);

		if (playerData.facingDir == -1 && controller.collisions.left && left.collider != null) {

			Gem leftGem = left.transform.GetComponent<Gem> ();

			if (playerData.highlightedGem != null) {

				playerData.highlightedGem.DisableHighlighting ();
			}

			playerData.highlightedGem = leftGem;
			leftGem.EnableHighlighting ();
		} else if (playerData.facingDir == 1 && controller.collisions.right && right.collider != null) {

			Gem rightGem = right.transform.GetComponent<Gem> ();

			if (playerData.highlightedGem != null) {

				playerData.highlightedGem.DisableHighlighting ();
			}

			playerData.highlightedGem = rightGem;
			rightGem.EnableHighlighting ();
		} else if (down.collider != null) {

			Gem downGem = down.transform.GetComponent<Gem> ();

			if (playerData.highlightedGem != null) {

				playerData.highlightedGem.DisableHighlighting ();
			}

			playerData.highlightedGem = downGem;
			downGem.EnableHighlighting ();
		} else {

			if (playerData.highlightedGem != null) {

				playerData.highlightedGem.DisableHighlighting ();
			}
			playerData.highlightedGem = null;
		}
	}

	struct RaycastOrigins {
		public Vector2 middleLeft, middleRight;
		public Vector2 bottomMiddle;
	}
}
