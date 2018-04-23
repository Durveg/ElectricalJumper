using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))][RequireComponent(typeof(Rigidbody2D))]
public class Gem : MonoBehaviour {

	public static float size = 100;

	[SerializeField]
	private float fallSpeed = -10;

	[SerializeField]
	private SpriteRenderer highLightSprite;

	private bool held = false;
	public bool isFalling = true;
	public bool spawnFalling = true;


	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rBody;

	[SerializeField]
	private Sprite gemSprite;

	[SerializeField]
	private LayerMask gemMask;

	private Transform GameBoard;

	public Vector2 fallToPosition { get; private set; }
	private string matchColor;

	public void PickUp(Transform newParent, Vector2 heldPosition) {

		this.held = true;

		this.rBody.simulated = false;

		this.transform.SetParent (newParent);
		this.transform.localPosition = heldPosition;
	}

	public void ThrowDownwards(Vector3 throwPosition) {

		this.isFalling = true;
		this.held = false;

		this.rBody.simulated = true;

		this.transform.position = throwPosition;
		this.transform.SetParent (GameBoard);
	}

	public void PlaceGem(Vector3 placedPosition) {

		this.isFalling = true;
		this.held = false;

		this.transform.position = placedPosition;

		this.rBody.simulated = true;
		this.transform.SetParent (GameBoard);
	}

	public void SetColor(Color gemColor, string colorString) {

		if (this.spriteRenderer == null) {

			this.spriteRenderer = this.GetComponent<SpriteRenderer> ();
		}
			
		this.matchColor = colorString;
		this.spriteRenderer.color = gemColor;
	}

	public void SetFallToPosition(Vector2 position) {


		this.fallToPosition = position;
		this.isFalling = true;
	}

	public void EnableHighlighting() {

		this.highLightSprite.enabled = true;
	}

	public void DisableHighlighting() {

		this.highLightSprite.enabled = false;
	}
		
	public List<GameObject> CheckVerticalMatches(List<GameObject> verticalMatches, string matchColor) {

		if (verticalMatches.Contains (this.gameObject) == false && this.matchColor == matchColor) {

			verticalMatches.Add (this.gameObject);

			RaycastHit2D[] hitsUp = Physics2D.RaycastAll (this.transform.position, Vector2.up, 1, gemMask);
			if (hitsUp.Length == 2) {

				verticalMatches = hitsUp [1].collider.GetComponent<Gem> ().CheckVerticalMatches (verticalMatches, matchColor);
			}

			RaycastHit2D[] hitsDown = Physics2D.RaycastAll (this.transform.position, Vector2.down, 1, gemMask);
			if (hitsDown.Length == 2) {

				verticalMatches = hitsDown [1].collider.GetComponent<Gem> ().CheckVerticalMatches (verticalMatches, matchColor);
			}
		}

		return verticalMatches;
	}

	public List<GameObject> CheckHorizontalMatches(List<GameObject> horizontalMatches, string matchColor) {

		if (horizontalMatches.Contains (this.gameObject) == false && this.matchColor == matchColor) {

			horizontalMatches.Add (this.gameObject);

			RaycastHit2D[] hitsLeft = Physics2D.RaycastAll (this.transform.position, Vector2.left, 1, gemMask);
			if (hitsLeft.Length == 2) {

				horizontalMatches =  hitsLeft[1].collider.GetComponent<Gem> ().CheckHorizontalMatches (horizontalMatches, matchColor);
			}

			RaycastHit2D[] hitsRight = Physics2D.RaycastAll (this.transform.position, Vector2.right, 1, gemMask);
			if (hitsRight.Length == 2) {

				horizontalMatches =  hitsRight[1].collider.GetComponent<Gem> ().CheckHorizontalMatches (horizontalMatches, matchColor);
			}
		}

		return horizontalMatches;
	}

	// Use this for initialization
	void Start () {

		this.rBody = this.GetComponent<Rigidbody2D> ();
		this.spriteRenderer = this.GetComponent<SpriteRenderer> ();
		this.spriteRenderer.sprite = gemSprite; //ToDo: add an array of sprites and array of colors.

		this.DisableHighlighting ();
	}
	
	// Update is called once per frame
	void Update () {

		if (held == false) {

			Vector2 velocity = new Vector2(0,0);
			velocity.y += fallSpeed * Time.deltaTime;

			if (Mathf.Abs(this.transform.position.y - this.fallToPosition.y) < 0.05f && this.isFalling == true) {

				this.transform.position = this.fallToPosition;

				this.isFalling = false;
				this.spawnFalling = false;

				List<GameObject> verticalMatches = new List<GameObject> ();
				verticalMatches = CheckVerticalMatches (verticalMatches, this.matchColor);

				List<GameObject> horizontalMatches = new List<GameObject> ();
				horizontalMatches = CheckHorizontalMatches (horizontalMatches, this.matchColor);

				bool deleteSelf = false;
				int totalMatches = 0;
				if (verticalMatches.Count >= 3) {

					deleteSelf = true;

					verticalMatches.Remove (this.gameObject);

					totalMatches += verticalMatches.Count;

					GameObject[] gemArray = verticalMatches.ToArray();
					int finalIndex = gemArray.Length - 1;
					for (int i = finalIndex; i >= 0; i--) {

						GameObject.Destroy(gemArray[i]);
					}
				}

				if (horizontalMatches.Count >= 3) {


					deleteSelf = true;

					horizontalMatches.Remove (this.gameObject);

					totalMatches += horizontalMatches.Count;

					GameObject[] gemArray = horizontalMatches.ToArray();
					int finalIndex = gemArray.Length - 1;
					for (int i = finalIndex; i >= 0; i--) {

						GameObject.Destroy(gemArray[i]);
					}
				}

				if (deleteSelf == true) {

					totalMatches += 1;
				
					GameManager.instance.MatchMade (totalMatches, this.matchColor);
					GameObject.Destroy (this.gameObject);
				}
			}

			if (isFalling == true) {
				transform.position = Vector3.MoveTowards(transform.position, fallToPosition, velocity.y);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		
		if (other.tag == "Player" && this.spawnFalling == true) {

			GameManager.instance.GameOver();
		}
	}
}
