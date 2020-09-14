using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformSkins { BLACK, WHITE, CHROME };

// public enum PlatformPhysics { NORMAL, ICY };

public class Platform : MonoBehaviour {

	public static float rotateSpeed = 1f;

	public static float shrinkSpeed = 0.25f;
	public static float minSize = 2f;

	// public bool icy;

	Material[] materials;
	// PhysicMaterial[] physicMaterials;

	public int color;
	// public int physic;

	void Awake () {
		materials = Resources.LoadAll<Material> ("Platform_Skins");
		// physicMaterials = Resources.LoadAll<PhysicMaterial> ("Physic_Materials");
		GetComponent<MeshRenderer> ().material = materials[Random.Range (0, materials.Length)];
		// GetComponent<Collider> ().material = physicMaterials[Random.Range (0, physicMaterials.Length)];
	}

	void Start () {
		color = System.Array.IndexOf (materials, GetComponent<MeshRenderer> ().material);
		// physic = System.Array.IndexOf (physicMaterials, GetComponent<Collider> ().material);
	}

	void FixedUpdate () {
		if (Player.started) {
			if (PlayerPrefs.GetInt("Shrink", 0) != 0) {
				Shrink ();
			}
			if (PlayerPrefs.GetInt("Rotate", 0) != 0) {
				Rotate ();
			}
		}
	}

	void Shrink () {
		if (transform.localScale.x > minSize) {
			transform.localScale -= (Vector3.forward + Vector3.right) * shrinkSpeed * Time.deltaTime;
		}
	}

	void Rotate () {
		transform.Rotate (Vector3.up, rotateSpeed, Space.World);
	}
}
