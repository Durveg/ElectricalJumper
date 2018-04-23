using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour {

	public Text scoreText;

	public void Restart() {
	
		Time.timeScale = 1;
		SceneManager.LoadScene (1);
	}

	public void Menu() {

		Time.timeScale = 1;
		SceneManager.LoadScene (0);
	}
}
