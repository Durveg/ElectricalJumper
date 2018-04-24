using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	[SerializeField]
	private Color redColor;
	[SerializeField]
	private Color blueColor;
	[SerializeField]
	private Color greenColor;
	[SerializeField]
	private Color orangeColor;

	[SerializeField]
	private float powerLevelDegradeRate = 1;

	[SerializeField]
	private int match3 = 8;
	[SerializeField]
	private int match4 = 12;
	[SerializeField]
	private int match5 = 18;
	[SerializeField]
	private int match7 = 32;


	public float redSliderValue = 100;
	public float blueSliderValue = 100;
	public float greenSliderValue = 100;
	public float orangeSliderValue = 100;

	[SerializeField]
	private Canvas inGameUI;
	[SerializeField]
	private Canvas GameOverUI;

	public bool gameIsOver = false;
	public int score = 0;

	private bool gameOverPlayed = false;

	public void GameOver() {

		this.gameIsOver = true;
		SoundManager.PlayGameOver();

		inGameUI.gameObject.SetActive (false);
		GameOverUI.gameObject.SetActive (true);
		GameOverUI.GetComponent<GameOverUI> ().scoreText.text = this.score.ToString ();
		Time.timeScale = 0;
	}

	public void MatchMade(int matches, string color) {

		float increaseAmount = 0;
		SoundManager.PlayClearGem ();
		if (matches == 3) {

			increaseAmount = match3;
			score += 100;
		} 
		else if (matches == 4) {

			increaseAmount = match4;
			score += 200;
			SignalBonus ("+4");
		} 
		else if (matches == 5) {

			increaseAmount = match5;
			score += 300;
			SignalBonus("+5");
		} 
		else if (matches == 7) {

			increaseAmount = match7;
			score += 500;
			SignalBonus ("Awesome +7");
		}

		if (color == "red") {

			this.redSliderValue += increaseAmount;
			Mathf.Clamp (this.redSliderValue, 0, 100);
		} else if (color == "blue") {

			this.blueSliderValue += increaseAmount;
			Mathf.Clamp (this.blueSliderValue, 0, 100);
		} else if (color == "green") {

			this.greenSliderValue += increaseAmount;
			Mathf.Clamp (this.greenSliderValue, 0, 100);
		} else if (color == "orange") {

			this.orangeSliderValue += increaseAmount;
			Mathf.Clamp (this.orangeSliderValue, 0, 100);
		}
	}

	private void SignalBonus(string text) {


	}

	void Start () {

		GameOverUI.gameObject.SetActive (false);
		GameManager.instance = this;
	}

	void Update() {

		redSliderValue -= this.powerLevelDegradeRate * Time.deltaTime;
		blueSliderValue -= this.powerLevelDegradeRate * Time.deltaTime;
		greenSliderValue -= this.powerLevelDegradeRate * Time.deltaTime;
		orangeSliderValue -= this.powerLevelDegradeRate * Time.deltaTime;

		if (redSliderValue <= 0 || blueSliderValue <= 0 || greenSliderValue <= 0 || orangeSliderValue <= 0) {

			if (gameOverPlayed == false) {
			
				gameOverPlayed = true;
				SoundManager.PlayGameOver ();

				inGameUI.gameObject.SetActive (false);
				GameOverUI.gameObject.SetActive (true);
				GameOverUI.GetComponent<GameOverUI> ().scoreText.text = this.score.ToString ();
				Time.timeScale = 0;
			}
		}
	}
}
