using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;
using UnityEngine.EventSystems;

public class PlayerSpeedController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 100f;

    public GameObject uiPanel;
    public TMP_Text movementSpeedText;
    public TMP_Text rotationSpeedText;

    public Button increaseMovementSpeedButton;
    public Button decreaseMovementSpeedButton;
    public Button increaseRotationSpeedButton;
    public Button decreaseRotationSpeedButton;

    public Camera xrCamera; // Assign the VR camera
    public LayerMask uiLayerMask; // Set this to UI in the inspector

    private bool isUIVisible = false;
    public float uiDistance = 1f; // Distance in front of the player

    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    void Start()
    {
        uiPanel.SetActive(false);
        UpdateSpeedUI();

        increaseMovementSpeedButton.onClick.AddListener(IncreaseMovementSpeed);
        decreaseMovementSpeedButton.onClick.AddListener(DecreaseMovementSpeed);
        increaseRotationSpeedButton.onClick.AddListener(IncreaseRotationSpeed);
        decreaseRotationSpeedButton.onClick.AddListener(DecreaseRotationSpeed);
    }

    void Update()
    {
        HandleControllerInput();
        DetectUIButtonPress();

        float moveHorizontal = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        float moveVertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        transform.Translate(moveHorizontal, 0f, moveVertical);

        float rotationHorizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, rotationHorizontal, 0f);
    }

    void HandleControllerInput()
    {
        // Get the VR controllers
        leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Handle UI visibility toggle
        if (rightHandDevice.isValid)
        {
            bool aButtonPressed;
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out aButtonPressed) && aButtonPressed)
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
            bool triggerPressed;
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed) && triggerPressed)
            {
                Ray ray = new Ray(xrCamera.transform.position, xrCamera.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10f, uiLayerMask))
                {
                    Button button = hit.collider.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.Invoke(); // Simulate button click
                    }
                }
            }
        }
    }

    void IncreaseMovementSpeed()
    {
        movementSpeed += 1f;
        UpdateSpeedUI();
    }

    void DecreaseMovementSpeed()
    {
        movementSpeed = Mathf.Max(1f, movementSpeed - 1f);
        UpdateSpeedUI();
    }

    void IncreaseRotationSpeed()
    {
        rotationSpeed += 10f;
        UpdateSpeedUI();
    }

    void DecreaseRotationSpeed()
    {
        rotationSpeed = Mathf.Max(10f, rotationSpeed - 10f);
        UpdateSpeedUI();
    }

    void UpdateSpeedUI()
    {
        if (movementSpeedText != null)
            movementSpeedText.text = movementSpeed.ToString("F1");

        if (rotationSpeedText != null)
            rotationSpeedText.text = rotationSpeed.ToString("F1");
    }
}