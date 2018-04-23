using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlayerData : ScriptableObject {

	public Vector2 input;

	public Gem highlightedGem;

	public bool holdingGem { get{ return heldItem != null;}}
	public BoardColumn boardColumn { get; private set; }

	private Gem heldItem;

	public void HoldItem(Gem item) {

		this.heldItem = item;
	}

	public Gem ThrowItem() {

		Gem held = heldItem;
		heldItem = null;

		return held;
	}

	public void SetBoardColumn(BoardColumn column) {

		this.boardColumn = column;
	}

	public Vector3 velocity;
}
