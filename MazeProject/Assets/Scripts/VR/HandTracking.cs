using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;

public class HandTracking : MonoBehaviour
{
    public LineRenderer leftLineRenderer;
    public LineRenderer rightLineRenderer;

    public LayerMask uiLayerMask; // Set this to your UI layer
    public Camera vrCamera; // Reference to the camera

    public float lineLength = 1f; // Set maximum length of the line
    public bool areLinesVisible = true;

    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    void Start()
    {
        // Ensure LineRenderers are set up
        if (leftLineRenderer == null) leftLineRenderer = InitializeLineRenderer(XRNode.LeftHand);
        if (rightLineRenderer == null) rightLineRenderer = InitializeLineRenderer(XRNode.RightHand);
    }

    void Update()
    {
        // Update the line position based on hand tracking
        TrackHandMovement(XRNode.LeftHand, leftLineRenderer);
        TrackHandMovement(XRNode.RightHand, rightLineRenderer);

        // Handle UI interactions with lines
        DetectUIInteraction(rightHandDevice, rightLineRenderer);
        DetectUIInteraction(leftHandDevice, leftLineRenderer);
    }

    // Tracks hand position and updates the LineRenderer
    void TrackHandMovement(XRNode handNode, LineRenderer lineRenderer)
    {
        InputDevice handDevice = InputDevices.GetDeviceAtXRNode(handNode);

        if (handDevice.isValid)
        {
            // Get hand position
            if (handDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 handPosition))
            {
                lineRenderer.SetPosition(0, handPosition); // Set start position of line (hand position)

                // Get the hand rotation and compute end position
                if (handDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion handRotation))
                {
                    Vector3 endPosition = handPosition + handRotation * Vector3.forward * lineLength; // Set line length to 1 unit
                    lineRenderer.SetPosition(1, endPosition); // Set end position of line
                }
            }
        }
    }

    // Raycasting to detect UI interactions
    void DetectUIInteraction(InputDevice handDevice, LineRenderer lineRenderer)
    {
        if (handDevice.isValid)
        {
            if (handDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
            {
                // Get hand position
                if (handDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 handPosition))
                {
                    // Perform a raycast from hand position
                    Ray ray = new Ray(handPosition, lineRenderer.GetPosition(1) - handPosition); // Cast a ray to the end of the line
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, lineLength, uiLayerMask))
                    {
                        // Check if the ray hits a button
                        Button button = hit.collider.GetComponent<Button>();
                        if (button != null)
                        {
                            button.onClick.Invoke(); // Simulate button click
                        }
                    }
                }
            }
        }
    }

    // Initialize LineRenderer with basic setup
    LineRenderer InitializeLineRenderer(XRNode handNode)
    {
        GameObject lineObject = new GameObject(handNode.ToString() + " Line");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Material and appearance setup
        Material material = new Material(Shader.Find("Sprites/Default"));
        material.color = Color.red; // Set line color to red for visibility

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.02f; // Thin line
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = material;
        lineRenderer.enabled = false; // Initially hidden

        return lineRenderer;
    }

    // Call to toggle the visibility of the line renderers
    public void ToggleLineVisibility(bool visible)
    {
        leftLineRenderer.enabled = visible;
        rightLineRenderer.enabled = visible;
    }
}
