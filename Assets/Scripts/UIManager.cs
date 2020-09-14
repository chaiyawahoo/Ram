using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	public bool paused;

	public static UIManager singleton;

	void Start() {
		singleton = this;
	}

	public void TogglePause () {
		Time.timeScale = paused ? 1 : 0;
		paused = !paused;
	}

	public void Restart () {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
}
