using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovementVR : MonoBehaviour
{
    public float moveSpeed;  // Speed of movement
    public float rotationSpeed; // Speed of rotation
    public float gravity = 9.81f; // Gravity strength

    private Rigidbody rb;
    private InputDevice leftController;
    private InputDevice rightController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing!");
        }
        else
        {
            rb.useGravity = false; // We handle gravity manually
            rb.freezeRotation = true; // Prevents unwanted physics rotation
        }

        InitializeControllers();
    }

    void Update()
    {
        if (!leftController.isValid || !rightController.isValid)
        {
            InitializeControllers();
        }

        MovePlayer();
        RotatePlayer();
        ApplyGravity();
    }

    void InitializeControllers()
    {
        var inputDevices = new List<InputDevice>();

        // Get Left Hand Controller
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, inputDevices);
        if (inputDevices.Count > 0) leftController = inputDevices[0];

        // Get Right Hand Controller
        inputDevices.Clear();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, inputDevices);
        if (inputDevices.Count > 0) rightController = inputDevices[0];
    }

    void MovePlayer()
    {
        if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystickInput))
        {
            Debug.Log("Left Joystick Input: " + leftJoystickInput);

            if (leftJoystickInput.magnitude > 0.1f) // Ignore very small inputs
            {
                Vector3 moveDirection = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y);
                moveDirection = transform.TransformDirection(moveDirection);
                rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
            }
        }
        else
        {
            Debug.LogError("Left joystick input not detected!");
        }
    }

    void RotatePlayer()
    {
        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightJoystickInput))
        {
            transform.Rotate(Vector3.up, rightJoystickInput.x * rotationSpeed * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        if (!IsGrounded())
        {
            rb.linearVelocity += Vector3.down * gravity * Time.deltaTime;
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        float rayDistance = 1.1f; // Adjust based on your player's height

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance))
        {
            return hit.collider.CompareTag("Ground");
        }

        return false;
    }
}
