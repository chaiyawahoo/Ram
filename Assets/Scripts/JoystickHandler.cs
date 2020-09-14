using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickHandler : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	private Image container;
	private Image joystick;
	private bool lefty;
	private bool touch;

	public Vector3 inputDirection;

	void Start () {
		container = GetComponent<Image> ();
		joystick = transform.GetChild (0).GetComponent<Image> ();
		inputDirection = Vector3.zero;
		CheckSettings ();
	}

	void Update () {
		CheckSettings ();
	}

	private void CheckSettings () {
		lefty = GameObject.FindWithTag ("Player").GetComponent<Player> ().lefty;
		touch = GameObject.FindWithTag ("Player").GetComponent<Player> ().touchControls;
		if (touch) {
			container.enabled = true;
			joystick.enabled = true;
		} else {
			container.enabled = false;
			joystick.enabled = false;
		}
		if (lefty) {
			container.rectTransform.anchorMax = Vector2.right;
			container.rectTransform.anchorMin = Vector2.right;
			container.rectTransform.pivot = Vector2.right;
		} else {
			container.rectTransform.anchorMax = Vector2.zero;
			container.rectTransform.anchorMin = Vector2.zero;
			container.rectTransform.pivot = Vector2.zero;
		}
	}

	public void OnDrag (PointerEventData ped) {
		Vector2 pos = Vector2.zero;

		RectTransformUtility.ScreenPointToLocalPointInRectangle (
			container.rectTransform,
			ped.position,
			ped.pressEventCamera,
			out pos
		);

		pos.x /= container.rectTransform.sizeDelta.x;
		pos.y /= container.rectTransform.sizeDelta.y;

		float x = lefty ? pos.x * 2 + 1 : pos.x * 2 - 1;
		float y = pos.y * 2 - 1;

		inputDirection = new Vector3 (x, y, 0);
		inputDirection = (inputDirection.magnitude > 1) ? inputDirection.normalized : inputDirection;

		joystick.rectTransform.anchoredPosition = new Vector3 (
			inputDirection.x * container.rectTransform.sizeDelta.x / 3,
			inputDirection.y * container.rectTransform.sizeDelta.y / 3,
			0
		);
	}

	public void OnPointerDown (PointerEventData ped) {
		OnDrag (ped);
	}

	public void OnPointerUp (PointerEventData ped) {
		inputDirection = Vector3.zero;
		joystick.rectTransform.anchoredPosition = Vector3.zero;
	}
}
