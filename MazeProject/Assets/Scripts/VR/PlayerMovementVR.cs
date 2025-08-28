using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerMovementVR : MonoBehaviour
{
    public MapLoader mapLoader;
    private float moveSpeed;
    private float rotationSpeed;
    public float gravity = 9.81f; // Gravity strength
    public List<int> errors_made = new List<int>();
    public int errors;
    public int maze_no;
    private Rigidbody rb;
    private InputDevice leftController;
    private InputDevice rightController;

    public MazeEventSystem MazeSystem;
    void Start()
    {
        maze_no=0;
        errors=0;
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

        // Get values from EyeTrackingDataManager
        moveSpeed = EyeTrackingDataManager.Instance != null ? EyeTrackingDataManager.Instance.LinearVelocity : 5.0f; // Default speed
        rotationSpeed = EyeTrackingDataManager.Instance != null ? EyeTrackingDataManager.Instance.RotationalVelocity : 50.0f; // Default rotation

        Debug.Log($"Loaded Linear Velocity: {moveSpeed}, Rotational Velocity: {rotationSpeed}");

        InitializeControllers();

        // Listen for scene load to reset position
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        transform.position = new Vector3(1, 0.5f, 2);
    }

    public List<int> get_errors(){
        return errors_made;
    }
    void Update()
    {
        if (!leftController.isValid || !rightController.isValid)
        {
            InitializeControllers();
        }

        MovePlayer();
        RotatePlayer();
        LockHeadTilt();
    }

    void LockHeadTilt()
    {
        Transform cameraTransform = Camera.main.transform;
        Quaternion currentRotation = cameraTransform.rotation;
        Vector3 euler = currentRotation.eulerAngles;
        cameraTransform.rotation = Quaternion.Euler(0, euler.y, 0);
    }

    void InitializeControllers()
    {
        var inputDevices = new List<InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, inputDevices);
        if (inputDevices.Count > 0) leftController = inputDevices[0];

        inputDevices.Clear();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, inputDevices);
        if (inputDevices.Count > 0) rightController = inputDevices[0];
    }

    // void MovePlayer()
    // {
    //     if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystickInput))
    //     {
    //         if (leftJoystickInput.magnitude > 0.1f)
    //         {
    //             Vector3 moveDirection = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y);
    //             moveDirection = transform.TransformDirection(moveDirection);
    //             rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    //         }
    //     }
    // }
void MovePlayer()
{
    if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystickInput))
    {
        if (leftJoystickInput.magnitude > 0.1f)
        {
            // Get headset's forward and right direction
            Transform cam = Camera.main.transform;
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            // Ignore vertical tilt
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            // Calculate movement direction
            Vector3 moveDirection = forward * leftJoystickInput.y + right * leftJoystickInput.x;

            // Apply movement
            rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        }
    }
}
    void RotatePlayer()
    {
        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightJoystickInput))
        {
            float rotationAmount = rightJoystickInput.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotationAmount, 0, Space.World);
        }

        Vector3 eulerRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, eulerRotation.y, 0);
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
        float rayDistance = 1.1f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance))
        {
            return hit.collider.CompareTag("Ground");
        }

        return false;
    }
    public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cheese")) // Ensure the player has the correct tag
            {

                Debug.Log("Player touched the trigger!");
                MazeSystem.OnPlayerTouchedCheese();
            }
            if (other.CompareTag("WallTripWire")) // Ensure the player has the correct tag
            {   
                Debug.Log("Player entered a deadend");
                errors+=1;
            }

        }

    // New method to handle UI interaction

}
