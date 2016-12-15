using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float speed = 0.0f;

    private Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        float x = moveHorizontal;
        float y = 0.0f;
        float z = moveVertical;

        Vector3 movement = new Vector3(x, y, z);

        rigidBody.AddForce(movement * speed);
    }
}
