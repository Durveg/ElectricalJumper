using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	private static SoundManager instance;

	[SerializeField]
	private AudioSource clearGem;
	[SerializeField]
	private AudioSource jump;
	[SerializeField]
	private AudioSource combo;
	[SerializeField]
	private AudioSource gameOver;
	[SerializeField]
	private AudioSource spawnGem;

	void Start() {

		SoundManager.instance = this;
	}

	public static void PlayJump() {

		instance.jump.Play();
	}

	public static void PlayClearGem() {

		instance.clearGem.Play ();
	}

	public static void PlayCombo() {

		instance.combo.Play ();
	}

	public static void PlayGameOver() {

		instance.gameOver.Play ();
	}

	public static void PlaySpawnGem() {

		instance.spawnGem.Play ();
	}
}
