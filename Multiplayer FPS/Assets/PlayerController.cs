using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

    private PlayerMotor motor;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        // Calculate movement velovity as 3D Vector

        float xMovement = Input.GetAxisRaw("Horizontal");
        float zMovement = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMovement;
        Vector3 moveVertical = transform.forward * zMovement;

        // Final movement vector
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        // Apply movement

        motor.Move(velocity);

        // Calculate rotation as a 3D Vector (Turning around)
        float yRotation = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0, yRotation, 0) * lookSensitivity;

        // Apply rotation
        motor.Rotate(rotation);
    }
}
