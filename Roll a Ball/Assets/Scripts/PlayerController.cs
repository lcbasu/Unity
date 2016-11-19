using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	void FixedUpdate() {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
	}
}
