using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    private Vector3 velocity = Vector3.zero;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Gets a movement vector
    public void Move(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    // Runs every physics iteration
    void FixedUpdate()
    {
        PerormMovement();
    }

    // Perform movement based on velocity variable
    private void PerormMovement()
    {
        if (this.velocity != Vector3.zero)
        {
            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
        }
    }
}
