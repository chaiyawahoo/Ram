using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour {

	public bool rotate {
		get => PlayerPrefs.GetInt("Rotate", 1) == 1;
		set { PlayerPrefs.SetInt("Rotate", Convert.ToInt32(value)); UpdateUI(); }
	}
	public bool shrink {
		get => PlayerPrefs.GetInt("Shrink", 1) == 1;
		set { PlayerPrefs.SetInt("Shrink", Convert.ToInt32(value)); UpdateUI(); }
	}
	public bool touchControls {
		get => PlayerPrefs.GetInt("Touch Controls", 0) == 1;
		set { PlayerPrefs.SetInt("Touch Controls", Convert.ToInt32(value)); UpdateUI(); }
	}
	public bool lefty {
		get => PlayerPrefs.GetInt("Lefty", 0) == 1;
		set { PlayerPrefs.SetInt("Lefty", Convert.ToInt32(value)); UpdateUI(); }
	}

	private static GameSettings singleton;
	public static GameSettings Instance { get { return singleton; } }
	void Awake () {
		singleton = this;
	}

	void UpdateUI () {
		UIManager.Instance.UpdateUI();
	}
}
