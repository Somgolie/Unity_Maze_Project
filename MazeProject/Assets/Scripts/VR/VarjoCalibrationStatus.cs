using UnityEngine;
using TMPro;
using Varjo.XR;
using System.Reflection;

public class VarjoCalibrationStatus : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI statusText; // Assign in Inspector

    void Start()
    {
        // Call this once at start to see what properties/fields GazeCalibrationQuality has
        PrintGazeCalibrationQualityMembers();
    }

    void Update()
    {
        UpdateCalibrationStatus();
    }

    private void UpdateCalibrationStatus()
    {
        if (!VarjoEyeTracking.IsGazeAllowed())
        {
            statusText.text = "Eye Tracking Not Allowed ❌";
            statusText.color = Color.red;
            return;
        }

        if (!VarjoEyeTracking.IsGazeCalibrated())
        {
            statusText.text = "Calibration Failed ❌";
            statusText.color = Color.red;
            return;
        }

        // Get calibration quality struct
        var quality = VarjoEyeTracking.GetGazeCalibrationQuality();

        // Replace these with actual property names after inspecting output
        // For now, try "left" and "right" as possible common names
        int leftQuality = GetCalibrationQualityValue(quality, "left");
        int rightQuality = GetCalibrationQualityValue(quality, "right");

        if (leftQuality <= -1 && rightQuality <= -1)
        {
            statusText.text = "Calibration Successful ✅";
            statusText.color = Color.green;
        }
        else
        {
            Debug.Log("❌ Calibration is POOR: Left = " + quality.left +" " + leftQuality + leftQuality.GetType() + ", Right = " + quality.right);
            statusText.text = "Calibration Poor ❌";
            statusText.color = Color.red;
        }
    }

    // Helper to get int value from a property or field by name (returns -1 if not found)
    private int GetCalibrationQualityValue(object obj, string name)
    {
        var type = obj.GetType();

        // Try property first
        var prop = type.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop != null)
        {
            object val = prop.GetValue(obj);
            if (val != null && int.TryParse(val.ToString(), out int result))
                return result;
        }

        // Try field next
        var field = type.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            object val = field.GetValue(obj);
            if (val != null && int.TryParse(val.ToString(), out int result))
                return result;
        }

        return -1; // Not found
    }

    // Debug method to print all fields and properties of GazeCalibrationQuality
    private void PrintGazeCalibrationQualityMembers()
    {
        var quality = VarjoEyeTracking.GetGazeCalibrationQuality();

        var type = quality.GetType();
        Debug.Log("GazeCalibrationQuality Type: " + type.FullName);

        var fields = type.GetFields();
        foreach (var field in fields)
        {
            Debug.Log("Field: " + field.Name + " = " + field.GetValue(quality));
        }

        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            Debug.Log("Property: " + prop.Name + " = " + prop.GetValue(quality));
        }
    }
}
