using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour {

	public static bool rotate; // platform rotates
	public static bool shrink; // platform shrinks

	void Awake () {
		PlayerPrefs.SetInt("Rotate", 1);
		PlayerPrefs.SetInt("Shrink", 1);

		rotate = PlayerPrefs.GetInt("Rotate", 0) != 0;
		shrink = PlayerPrefs.GetInt("Shrink", 0) != 0;
	}
}
