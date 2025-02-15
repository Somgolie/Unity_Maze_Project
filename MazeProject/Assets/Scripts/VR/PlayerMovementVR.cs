using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovementVR : MonoBehaviour
{
    public float moveSpeed = 2.0f;  // Speed of movement
    public float rotationSpeed = 45.0f; // Speed of rotation
    public float gravity = 9.81f; // Gravity strength

    private CharacterController characterController;
    private InputDevice leftController;
    private InputDevice rightController;
    private float verticalVelocity = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
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
            Vector3 moveDirection = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y);
            moveDirection = transform.TransformDirection(moveDirection);
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
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
        if (IsGrounded())
        {
            verticalVelocity = -0.1f; // Keep the player slightly on the ground
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        characterController.Move(new Vector3(0, verticalVelocity * Time.deltaTime, 0));
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        float rayDistance = characterController.height / 2 + 0.1f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance))
        {
            return hit.collider.CompareTag("Ground");
        }

        return false;
    }
}
