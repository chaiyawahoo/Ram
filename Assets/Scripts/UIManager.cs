using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	public bool paused;

	private static UIManager singleton;
	public static UIManager Instance { get { return singleton; } }

	Image joystick;
	Image jumpButton;

	void Awake () {
		singleton = this;
	}

	void Start () {
		joystick = GameObject.Find("Joystick Container").GetComponent<Image> ();
		jumpButton = GameObject.Find("Jump Button").GetComponent<Image>();
		UpdateUI ();
	}

	public void TogglePause () {
		Time.timeScale = paused ? 1 : 0;
		paused = !paused;
		// TODO: bring up pause menu
	}

	public void Restart () {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void UpdateUI () { 
		if (GameSettings.Instance.lefty) {
			joystick.rectTransform.anchorMax = Vector2.right;
			joystick.rectTransform.anchorMin = Vector2.right;
			joystick.rectTransform.pivot = Vector2.right;

			jumpButton.rectTransform.pivot = new Vector2(0f, 0.5f);
		} else {
			joystick.rectTransform.anchorMax = Vector2.zero;
			joystick.rectTransform.anchorMin = Vector2.zero;
			joystick.rectTransform.pivot = Vector2.zero;

			jumpButton.rectTransform.pivot = new Vector2(1f, 0.5f);
		}

		joystick.gameObject.SetActive(GameSettings.Instance.touchControls);
		jumpButton.gameObject.SetActive(GameSettings.Instance.touchControls);
	}
}
