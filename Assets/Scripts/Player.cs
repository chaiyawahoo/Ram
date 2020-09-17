using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.OnScreen;
using UnityEditor;

[RequireComponent (typeof (Rigidbody))]
public class Player : MonoBehaviour {

 	enum Skins { RED, GREEN, BLUE, YELLOW, PINK, ORANGE, BLACK, WHITE };

	public static bool started;

	static float collisionForce = 700f;
	static float jumpForce = 450f;
	static float moveForce = 20f;
	static float deathThreshold = -5f;

	public bool mainPlayer = false;

	public int color;


	OnScreenStick joystick;
	OnScreenButton jumpButton;
	Rigidbody rb;
	Material[] materials;
	PlayerInput pInput;

	bool grounded;
	Vector3 moveDir;
	bool jumping;
	bool airborne;
	bool doubleJumped;
	bool xClose; // arbitrary
	bool zClose;

	void Awake () {
		materials = Resources.LoadAll<Material> ("Player_Skins");
		rb = GetComponent<Rigidbody> ();
		pInput = GetComponent<PlayerInput> ();
		GetComponent<MeshRenderer> ().material = materials[Random.Range (0, materials.Length)];
	}

	void Start () {
		joystick = GameObject.Find ("Joystick").GetComponent<OnScreenStick> ();
		jumpButton = GameObject.Find ("Jump Button").GetComponent<OnScreenButton> ();
		grounded = false;
		color = System.Array.IndexOf (materials, GetComponent<MeshRenderer> ().material);
		moveDir = Vector3.zero;
		jumping = false;
		airborne = false;
		doubleJumped = false;
	}

	void FixedUpdate () {
		if (started) {
			Move ();
			if (PlayerPrefs.GetInt("Rotate", 0) != 0) {
				Rotate ();
			}
		}
	}

	void Update () {
		if (mainPlayer) {
			HandleInput ();
		}
		if (started && !UIManager.Instance.paused) {
			Jump ();
			Die ();
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.CompareTag("Player")) {
			rb.AddExplosionForce (collisionForce, collision.contacts[0].point, 0.1f);
			airborne = true;
		}

		if (collision.gameObject.CompareTag("Platform")) {
			airborne = false;
			grounded = true;
			if (!started) {
				started = true;
			}
		}
	}

	void OnCollisionStay (Collision collision) {
		if (collision.gameObject.CompareTag("Platform") && collision.contacts[0].normal == collision.transform.up) {
			grounded = true;
			doubleJumped = false;
		}
	}

	void OnCollisionExit (Collision collision) {
		if (collision.gameObject.CompareTag("Platform") && airborne) {
			float platformWidth = collision.collider.bounds.size.x;
			Transform platformTransform = collision.transform;
			if (Vector3.Distance(transform.position, platformTransform.position) >= platformWidth / 2) {
				airborne = true;
			}
			grounded = false;
		}
	}

	void HandleInput() {
		Vector2 moveInput = Vector2.zero;
		moveDir = Vector3.zero;
		jumping = false;

		if (GameSettings.Instance.touchControls) {
			// TODO: create better joystick handler (override onscreenstick)
			Vector2Control onScreenStick = (Vector2Control)joystick.control;
			ButtonControl onScreenButton = (ButtonControl)jumpButton.control;
			if (onScreenStick != null) {
				moveInput = onScreenStick.ReadValue();
			}
			jumping = onScreenButton.wasPressedThisFrame;
		} else {
			moveInput = pInput.actions["Move"].ReadValue<Vector2> ();
			jumping = pInput.actions["Jump"].triggered;
		}

		if (Keyboard.current.lKey.wasPressedThisFrame) {
			GameSettings.Instance.lefty = !GameSettings.Instance.lefty;
		}

		if (Keyboard.current.tKey.wasPressedThisFrame) {
			GameSettings.Instance.touchControls = !GameSettings.Instance.touchControls;
		}

		moveDir.x = moveInput.x;
		moveDir.z = moveInput.y;
	}

	void Move () {
		rb.AddForce (moveDir * moveForce);
	}

	void Jump () {
		bool canJump = false;
		if (jumping) {
			if (grounded) {
				canJump = true;
			} else if (!doubleJumped) {
				doubleJumped = true;
				canJump = true;
			}
		}

		if (canJump) {
			if (rb.velocity.y < 0) {
				// rb.velocity -= rb.velocity.y * Vector3.up;
				rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
			}
			rb.AddForce (Vector3.up * jumpForce);
			airborne = true;
			xClose = Mathf.Abs(transform.position.x) < 1f; // arbitrary
			zClose = Mathf.Abs(transform.position.z) < 1f;
			if (grounded) {
				// Upon jump, travel in direction based on rotation/momentum
				float t = Time.fixedDeltaTime;
				float angVel = Platform.rotateSpeed * Mathf.Deg2Rad / t;
				float r = Vector3.Distance (Vector3.zero, (transform.position - transform.position.y * Vector3.up));
				float vel = angVel * r;
				Quaternion oldRot = transform.rotation;
				transform.LookAt (Vector3.up * transform.position.y);
				// transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles - (Vector3.up * 90f));
				transform.Rotate(0f, -90f, 0f);

				rb.velocity += transform.forward * vel;
				transform.rotation = oldRot;
			}
		}
	}

	void Rotate () {
		if (!airborne) {
			transform.RotateAround (Vector3.zero, Vector3.up, Platform.rotateSpeed);
		} else {
			if (xClose && zClose) {
				transform.Rotate (Vector3.up * Platform.rotateSpeed, Space.World);
			}
		}
	}

	void Die () {
		if (transform.position.y <= deathThreshold) {
			UIManager.Instance.Restart ();
			Destroy (gameObject);
		}
	}
}
