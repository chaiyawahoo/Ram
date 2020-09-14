using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	public float zoomSpeed = 0.0025f;
	public float zoomStopY = 3.5f;

	// GameObject platform;

	// Use this for initialization
	void Start () {
		// platform = GameObject.FindGameObjectWithTag("Platform");
	}

	// Update is called once per frame
	void Update () {
		if (Player.started) {
				// TODO: Use math to find the distance required to see the platform at all times
				if (PlayerPrefs.GetInt("Shrink", 0) != 0) {
					Zoom ();
				}
			}
	}

	void Zoom () {
		if (transform.position.y > zoomStopY) {
			transform.localPosition += transform.forward * zoomSpeed * Time.timeScale;
		}
	}
}
