using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	const float skinWidth = 0.015f;

	public GameObject standingOn;

	[SerializeField]
	public PlayerData playerData;

	[SerializeField]
	private int horizontalRayCount = 4;
	[SerializeField]
	private int verticalRayCount = 4;
	[SerializeField]
	private LayerMask collisionMask;

	private float horizontalRaySpacing;
	private float verticalRaySpacing;

	private new BoxCollider2D collider;

	private RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;

	public void Move(Vector3 velocity) {
		UpdateRaycastOrigins ();

		collisions.Reset ();

		if (velocity.x != 0) {

			this.collisions.facingDir = (int)Mathf.Sign (velocity.x);
			playerData.facingDir = (int)Mathf.Sign (velocity.x);
		}

		HorizontalCollisions (ref velocity);
		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		transform.Translate (velocity);
	}

	// Use this for initialization
	void Start () {

		this.collider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();

		this.collisions.facingDir = 1;
	}

	private void HorizontalCollisions(ref Vector3 velocity) {
		
		float dirX = collisions.facingDir;
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		if (Mathf.Abs(velocity.x) < skinWidth) {

			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++) {

			Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * dirX, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.right * dirX * rayLength, Color.red);

			if (hit) {

				velocity.x = (hit.distance - skinWidth) * dirX;
				rayLength = hit.distance;

				collisions.left = dirX == -1;
				collisions.right = dirX == 1;
			}
		}
	}

	private void VerticalCollisions(ref Vector3 velocity) {
		float dirY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {

			Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * dirY, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.up * dirY * rayLength, Color.red);

			if (hit) {

				velocity.y = (hit.distance - skinWidth) * dirY;
				rayLength = hit.distance;

				collisions.below = dirY == -1;
				collisions.above = dirY == 1;

				if (collisions.below) {

					this.standingOn = hit.transform.gameObject;
				} 
			} 
			else {

				this.standingOn = null;
			}
		}
	}

	private void UpdateRaycastOrigins() {

		Bounds bounds = this.collider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	private void CalculateRaySpacing() {

		Bounds bounds = this.collider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo {

		public bool above, below;
		public bool left, right;

		public int facingDir;

		public void Reset() {

			above = below = false;
			left = right = false;
		}
	}

}
