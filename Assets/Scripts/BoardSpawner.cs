using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpawner : MonoBehaviour {

	[SerializeField]
	private GameObject boardBottom;

	[SerializeField]
	private int boardWidth;
	[SerializeField]
	private int boardHeight;
	[SerializeField]
	private float spawnSpeed;


	[SerializeField]
	private Color[] spawnColors;
	[SerializeField]
	private string[] colorStrings;

	private Vector2[] spawnPositions;
	private BoardColumn[] columns;

	[SerializeField]
	private float timeToMaxSpawnSpeedSec = 240;

	[SerializeField]
	private float timeBetweenSpawnsMax = 0.75f;
	[SerializeField]
	private float timeBetweenSpawnsMin = 0;

	[SerializeField]
	private float timeToMinDelay = 240;
	[SerializeField]
	private float delayBeforeFallMax = 1;
	[SerializeField]
	private float delayBeforeFallMin = 0.5f;

	[SerializeField]
	private string gemPrefabString = "Gem";
	[SerializeField]
	private string columnPrefabString = "BoardColumn";

	private Gem SpawnGem() {

		GameObject go = (GameObject) Instantiate(Resources.Load(gemPrefabString, typeof(GameObject)));
		Gem retGem = go.GetComponent<Gem> ();

		int colorIndex = Random.Range (0, spawnColors.Length);
		Color gemColor = spawnColors [colorIndex];
		string colorString = colorStrings [colorIndex];
			
		retGem.SetColor (gemColor,colorString);

		return retGem;
	}

	// Use this for initialization
	void Start () {
		
		this.spawnPositions = new Vector2[boardWidth];
		this.columns = new BoardColumn[boardWidth];

		float maxBoardHeight = boardHeight;// * gemSize;
		float maxBoardWidth = boardWidth;// * gemSize;


		float heightAdjust = -0.5f;
		if (maxBoardHeight % 2 == 0) {

			heightAdjust = -0.5f;
		}
		float topOfBoard = (maxBoardHeight / 2) + 1 + heightAdjust;

		float widthAdjust = 0.5f;
		if (maxBoardWidth % 2 == 0) {

			widthAdjust = 0.5f;
		}
		float xSpawnPoint = ((maxBoardWidth / 2) * -1) + widthAdjust;

		float bottomSlot = -1 * (maxBoardHeight / 2) - heightAdjust;
		float[] slotHeights = new float[boardHeight];
		for (int i = 0; i < boardHeight; i++) {

			slotHeights [i] = bottomSlot + i;
		}

		for (int i = 0; i < boardWidth; i++) {

			spawnPositions [i] = new Vector2 (xSpawnPoint + i, topOfBoard);

			GameObject go = (GameObject) Instantiate(Resources.Load(columnPrefabString, typeof(GameObject)));
			go.transform.SetParent (this.transform);
			go.transform.localScale = new Vector3 (1, boardHeight, 1);
			go.transform.localPosition = new Vector3(spawnPositions[i].x, 0, 0);

			BoardColumn col = go.GetComponent<BoardColumn> ();
			this.columns[i] = col;
			col.SetColNum(i);
			col.SetSlots(slotHeights);
		}

		float bottomOfBoard = -1 * ((maxBoardHeight / 2) + 1 + heightAdjust);
		this.boardBottom.transform.localPosition = new Vector2 (0, bottomOfBoard);

		StartCoroutine (SpawnGemTimer());
	}
	
	private IEnumerator SpawnGemTimer() {

		float spawnSpeed = 0;
		float delaySpeed = 0;
		while (true) {

			float waitBeforeSignalTime = Mathf.Lerp (timeBetweenSpawnsMax, timeBetweenSpawnsMin, spawnSpeed);
			Debug.Log ("Time before signal: " + waitBeforeSignalTime);
			if (spawnSpeed < 1) {

				spawnSpeed += Time.deltaTime / timeToMaxSpawnSpeedSec;
			}

			yield return new WaitForSeconds (waitBeforeSignalTime);

			int gemChannel = Random.Range (0, boardWidth);
			this.columns[gemChannel].SignalSpawnPosition(true);


			float delayTimer = Mathf.Lerp (delayBeforeFallMax, delayBeforeFallMin, delaySpeed);
			Debug.Log ("Time before delay over: " + delayTimer);
			if (delaySpeed < 1) {

				delaySpeed += Time.deltaTime / timeToMinDelay;
			}

			yield return new WaitForSeconds (delayTimer / 2);

			this.columns[gemChannel].SignalSpawnPosition(false);
			SoundManager.PlaySpawnGem ();

			Gem gem = SpawnGem ();
			gem.transform.position = spawnPositions [gemChannel];
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
