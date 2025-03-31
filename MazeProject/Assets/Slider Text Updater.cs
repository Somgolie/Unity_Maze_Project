using UnityEngine;
using UnityEngine.UI;

public class SliderTextUpdater : MonoBehaviour
{
    public Slider confidenceSlider; // Assign in Inspector
    public Text confidenceValueText; // Assign in Inspector

    void Start()
    {
        if (confidenceSlider == null || confidenceValueText == null)
        {
            Debug.LogError("Missing UI references! Assign the Slider and Text in the Inspector.");
            return;
        }

        // Update text initially
        UpdateConfidenceText(confidenceSlider.value);
        
        // Add listener for slider changes
        confidenceSlider.onValueChanged.AddListener(UpdateConfidenceText);
    }

    void UpdateConfidenceText(float value)
    {
        if (confidenceValueText != null)
        {
            confidenceValueText.text = value.ToString("0"); // Show as an integer
        }
    }

    // ✅ Public method to get slider value
    public string GetConfidenceValue()
    {
        if (confidenceSlider == null)
        {
            Debug.LogError("ConfidenceSlider is NULL in SliderTextUpdater!");
            return "0"; // Return default string value
        }

        string valueStr = confidenceSlider.value.ToString("0"); // Convert float to string
        Debug.Log($"Slider Value: {valueStr}"); // Debug log to see the value
        return valueStr;
    }

}
