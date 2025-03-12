using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class PlayerSpeedController : MonoBehaviour
{
    public int movementSpeed = 1;
    public int rotationSpeed = 10; // Starts at 10, scales up to 100

    public GameObject uiPanel;
    public Text movementSpeedText;
    public Text rotationSpeedText;

    public Button increaseMovementSpeedButton;
    public Button decreaseMovementSpeedButton;
    public Button increaseRotationSpeedButton;
    public Button decreaseRotationSpeedButton;

    public Slider movementSpeedSlider;
    public Slider rotationSpeedSlider;

    public Camera xrCamera;
    public LayerMask uiLayerMask;

    private bool isUIVisible = false;
    public float uiDistance = 1f;

    private UnityEngine.XR.InputDevice leftHandDevice;
    private UnityEngine.XR.InputDevice rightHandDevice;

    void Start()
    {
        uiPanel.SetActive(false);

        // Set slider ranges
        movementSpeedSlider.minValue = 1;
        movementSpeedSlider.maxValue = 10;
        movementSpeedSlider.value = movementSpeed;

        rotationSpeedSlider.minValue = 1;  // UI always displays 1 - 10
        rotationSpeedSlider.maxValue = 10;
        rotationSpeedSlider.value = ConvertRotationSpeedToUI(rotationSpeed);

        UpdateSpeedUI();

        increaseMovementSpeedButton.onClick.AddListener(() => ChangeMovementSpeed(1));
        decreaseMovementSpeedButton.onClick.AddListener(() => ChangeMovementSpeed(-1));
        increaseRotationSpeedButton.onClick.AddListener(() => ChangeRotationSpeed(10));
        decreaseRotationSpeedButton.onClick.AddListener(() => ChangeRotationSpeed(-10));

        movementSpeedSlider.onValueChanged.AddListener(OnMovementSliderChanged);
        rotationSpeedSlider.onValueChanged.AddListener(OnRotationSliderChanged);
    }

    void Update()
    {
        HandleControllerInput();
        DetectUIButtonPress();

        // Player Movement Logic
        float moveHorizontal = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        float moveVertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        transform.Translate(moveHorizontal, 0f, moveVertical);

        // Player Rotation Logic with UI update
        RotatePlayerWithUIUpdate();
    }

    void HandleControllerInput()
    {
        leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightHandDevice.isValid)
        {
            if (rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool aButtonPressed) && aButtonPressed)
            {
                isUIVisible = !isUIVisible;
                uiPanel.SetActive(isUIVisible);

                if (isUIVisible)
                {
                    PositionUIInFrontOfPlayer();
                }
            }
        }
    }

    void PositionUIInFrontOfPlayer()
    {
        if (xrCamera != null && uiPanel != null)
        {
            uiPanel.transform.position = xrCamera.transform.position + xrCamera.transform.forward * uiDistance;
            uiPanel.transform.LookAt(xrCamera.transform);
            uiPanel.transform.rotation = Quaternion.LookRotation(uiPanel.transform.position - xrCamera.transform.position);
        }
    }

    void DetectUIButtonPress()
    {
        if (rightHandDevice.isValid)
        {
            if (rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
            {
                Ray ray = new Ray(xrCamera.transform.position, xrCamera.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 10f, uiLayerMask))
                {
                    Button button = hit.collider.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.Invoke();
                    }
                }
            }
        }
    }

    void ChangeMovementSpeed(int change)
    {
        movementSpeed = Mathf.Clamp(movementSpeed + change, 1, 10);
        movementSpeedSlider.value = movementSpeed;
        UpdateSpeedUI();
    }

    void ChangeRotationSpeed(int change)
    {
        rotationSpeed = Mathf.Clamp(rotationSpeed + change, 10, 100); // Updated range
        rotationSpeedSlider.value = ConvertRotationSpeedToUI(rotationSpeed);
        UpdateSpeedUI();
    }

    void OnMovementSliderChanged(float value)
    {
        movementSpeed = Mathf.RoundToInt(value);
        UpdateSpeedUI();
    }

    void OnRotationSliderChanged(float value)
    {
        rotationSpeed = ConvertUIToRotationSpeed(value);
        UpdateSpeedUI();
    }

    void UpdateSpeedUI()
    {
        movementSpeedText.text = movementSpeed.ToString();
        rotationSpeedText.text = ConvertRotationSpeedToUI(rotationSpeed).ToString(); // Display 1 - 10
    }

    // Convert 10-100 rotation speed to 1-10 for UI
    float ConvertRotationSpeedToUI(int rotationSpeed)
    {
        return Mathf.Clamp((rotationSpeed - 10) / 10 + 1, 1, 10);
    }

    // Convert 1-10 UI values back to 10-100 rotation speed
    int ConvertUIToRotationSpeed(float uiValue)
    {
        return Mathf.Clamp(((int)uiValue - 1) * 10 + 10, 10, 100);
    }

    // Function to rotate the player and update UI
    void RotatePlayerWithUIUpdate()
    {
        // Get the joystick input for rotation (assuming the right joystick is used)
        if (rightHandDevice.isValid && rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 rightJoystickInput))
        {
            // Calculate rotation amount based on joystick input
            float rotationAmount = rightJoystickInput.x * rotationSpeed * Time.deltaTime;

            // Apply rotation to the player (rotation along the Y axis)
            transform.Rotate(0, rotationAmount, 0, Space.World);
        }

        // Lock X and Z rotation to prevent tilting
        Vector3 eulerRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, eulerRotation.y, 0);

        // Update the UI dynamically with the current rotation speed
        rotationSpeedSlider.value = ConvertRotationSpeedToUI(rotationSpeed);
        rotationSpeedText.text = ConvertRotationSpeedToUI(rotationSpeed).ToString(); // Display from 1 - 10
    }
}
