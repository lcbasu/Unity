using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	private float fixedUpdateTimer;
	private float updateTimer;

	void FixedUpdate() {
		Debug.Log ("FixedUpdate time: " + Time.deltaTime);
	}

	void Update() {
		Debug.Log ("Update Time: "+Time.deltaTime);
	}
}
