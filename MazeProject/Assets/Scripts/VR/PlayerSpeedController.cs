using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class PlayerSpeedController : MonoBehaviour
{
    public int movementSpeed = 1;
    public int rotationSpeed = 100;

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

    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    void Start()
    {
        uiPanel.SetActive(false);
        
        // Setup slider ranges
        movementSpeedSlider.minValue = 1;
        movementSpeedSlider.maxValue = 10;
        movementSpeedSlider.value = movementSpeed;

        rotationSpeedSlider.minValue = 1;
        rotationSpeedSlider.maxValue = 10;
        rotationSpeedSlider.value = rotationSpeed / 10;
        
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

        float moveHorizontal = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        float moveVertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        transform.Translate(moveHorizontal, 0f, moveVertical);

        float rotationHorizontal = Input.GetAxis("RightJoystickHorizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, rotationHorizontal, 0f);
    }

    void HandleControllerInput()
    {
        leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightHandDevice.isValid)
        {
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool aButtonPressed) && aButtonPressed)
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
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
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
        rotationSpeed = Mathf.Clamp(rotationSpeed + (change * 10), 10, 100);
        rotationSpeedSlider.value = rotationSpeed / 100;
        UpdateSpeedUI();
    }

    void OnMovementSliderChanged(float value)
    {
        movementSpeed = Mathf.RoundToInt(value);
        UpdateSpeedUI();
    }

    void OnRotationSliderChanged(float value)
    {
        rotationSpeed = Mathf.RoundToInt(value) * 10;
        UpdateSpeedUI();
    }

    void UpdateSpeedUI()
    {
        movementSpeedText.text = movementSpeed.ToString();
        rotationSpeedText.text = (rotationSpeed/10).ToString();
    }
}