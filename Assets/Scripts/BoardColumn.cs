﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))][RequireComponent(typeof(SpriteRenderer))]
public class BoardColumn : MonoBehaviour {

	public int columnNumber { get; private set; }
	public Vector2 nextSpawnPosition { get; private set; }

	[SerializeField]
	private PlayerData playerData;
	[SerializeField]
	private float[] slotHeights;

	private new BoxCollider2D collider;
	private SpriteRenderer spriteRenderer;
	[SerializeField]
	private GameObject signalPrefab;
	[SerializeField]
	private SpriteRenderer signalSprite;

	private bool flaggedColumn = false;

	private Vector2 rayOriginPoint;
	[SerializeField]
	private LayerMask gemLayerMask;

	public void SetColNum(int colNum) {

		columnNumber = colNum;
	}

	public void SetSlots(float[] heights) {

		this.slotHeights = heights;
	}

	public void SignalSpawnPosition(bool signalEnabled) {

		this.signalSprite.transform.position = this.nextSpawnPosition;
		this.signalSprite.enabled = signalEnabled;
	}

	// Use this for initialization
	void Start () {

		GameObject go = Instantiate(signalPrefab);
		go.transform.localScale = new Vector3 (0.75f, 0.75f, 1);
		go.transform.SetParent (this.transform);

		this.signalSprite = go.GetComponent<SpriteRenderer> ();
		this.signalSprite.enabled = false;

		this.collider = this.GetComponent<BoxCollider2D>();
		this.spriteRenderer = this.GetComponent<SpriteRenderer>();
		this.spriteRenderer.enabled = false;

		this.rayOriginPoint = new Vector2 (this.transform.position.x, this.collider.bounds.min.y);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {

			CheckCenterBounds(other);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {

			CheckCenterBounds(other);
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player") {

			CheckCenterBounds(other);
		}
	}

	private void CheckCenterBounds(Collider2D other) {

		if (this.collider != null) {

			if (this.collider.bounds.Contains (other.transform.position)) {

				this.spriteRenderer.enabled = true;
				if (this.flaggedColumn == false) {

					this.playerData.SetBoardColumn (this);
					this.flaggedColumn = true;
				}
			} else {

				this.spriteRenderer.enabled = false;
				this.flaggedColumn = false;
			}
		} else {
			Debug.Log ("collider is null");
		}
	}

	// Update is called once per frame
	void Update () {


		RaycastHit2D[] hits = Physics2D.RaycastAll (this.rayOriginPoint, Vector2.up, 500, gemLayerMask);
		Debug.DrawRay (this.rayOriginPoint, Vector2.up, Color.green);

		int index = 0;
		foreach(RaycastHit2D thisHit in hits) {

			Gem gem = thisHit.transform.GetComponent<Gem> ();
			if (gem != null) {

				if (gem.isFalling) {

					gem.SetFallToPosition (new Vector2 (this.transform.position.x, this.slotHeights [index]));
				} 
				else if (gem.fallToPosition.y > this.slotHeights [index]) {

					gem.SetFallToPosition(new Vector2(this.transform.position.x, this.slotHeights [index]));
				}
			}
			index++;
		}

		this.nextSpawnPosition = new Vector2 (this.transform.position.x, this.slotHeights [index]);
		if (this.signalSprite.enabled == true) {

			this.signalSprite.transform.position = this.nextSpawnPosition;
		}
	}
}
