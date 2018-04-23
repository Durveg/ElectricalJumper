using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {

	[SerializeField]
	private Text scoreField;

	[SerializeField]
	private Slider powerSliderRed;
	[SerializeField]
	private Slider powerSliderBlue;
	[SerializeField]
	private Slider powerSliderGreen;
	[SerializeField]
	private Slider powerSliderOrange;

	// Update is called once per frame
	void Update () {
		
		if (GameManager.instance == null) {

			return;
		}


		powerSliderRed.value = GameManager.instance.redSliderValue;
		powerSliderBlue.value = GameManager.instance.blueSliderValue;
		powerSliderGreen.value = GameManager.instance.greenSliderValue;
		powerSliderOrange.value = GameManager.instance.orangeSliderValue;

		scoreField.text = GameManager.instance.score.ToString();
	}
}
