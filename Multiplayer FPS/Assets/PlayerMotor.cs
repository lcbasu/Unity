using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

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

    // Gets a rotation vector
    public void Rotate(Vector3 rotation)
    {
        this.rotation = rotation;
    }

    // Runs every physics iteration
    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    // Perform movement based on velocity variable
    private void PerformMovement()
    {
        if (this.velocity != Vector3.zero)
        {
            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
        }
    }

    // Perform rotation based on rotation variable
    private void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
    }

}
