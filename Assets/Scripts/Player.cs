using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum PlayerSkins { RED, GREEN, BLUE, YELLOW, PINK, ORANGE, BLACK, WHITE };

[RequireComponent (typeof (Rigidbody))]
public class Player : MonoBehaviour {

	public static bool started;

	static float collisionForce = 700f;
	static float jumpForce = 450f;
	static float moveForce = 20f;
	static float deathThreshold = -5f;

	public bool mainPlayer = false;
	public bool touchControls = false;
	public bool lefty = false;

	public int color;

	JoystickHandler joystick;
	Rigidbody rb;
	Material[] materials;

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
		GetComponent<MeshRenderer> ().material = materials[Random.Range (0, materials.Length)];
	}

	void Start () {
		joystick = GameObject.FindWithTag ("Joystick").GetComponent<JoystickHandler> ();
		grounded = false;
		color = System.Array.IndexOf (materials, GetComponent<MeshRenderer> ().material);
		moveDir = Vector3.zero;
		jumping = false;
		airborne = false;
		doubleJumped = false;
	}

	void FixedUpdate () {
		if (mainPlayer) {
			HandleInput ();
		}
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
		if (started && !UIManager.singleton.paused) {
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
			grounded = false;
		}
	}

	void HandleInput() {
		moveDir = Vector3.zero;
		jumping = false;

		if (!touchControls) {
			if (Input.GetKey (KeyCode.RightArrow)) {
				moveDir += Vector3.right;
			}
			if (Input.GetKey (KeyCode.LeftArrow)) {
				moveDir += Vector3.left;
			}
			if (Input.GetKey (KeyCode.UpArrow)) {
				moveDir += Vector3.forward;
			}
			if (Input.GetKey (KeyCode.DownArrow)) {
				moveDir += Vector3.back;
			}
			if (Input.GetKeyDown (KeyCode.Space)) {
				jumping = true;
			}
		} else {
			foreach (Touch touch in Input.touches) {
				if (!jumping) {
					if (!lefty) {
						if (touch.position.x >= Camera.main.scaledPixelWidth / 2 && touch.phase == TouchPhase.Began) {
							jumping = true;
						}
					} else {
						if (touch.position.x < Camera.main.scaledPixelWidth / 2 && touch.phase == TouchPhase.Began) {
							jumping = true;
						}
					}
				}
			}
			moveDir = new Vector3(joystick.inputDirection.x, 0, joystick.inputDirection.y);
		}
	}

	void Move () {
		// if (airborne) {
			rb.AddForce (moveDir * moveForce);
		// }
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
			UIManager.singleton.Restart ();
			Destroy (gameObject);
		}
	}
}
