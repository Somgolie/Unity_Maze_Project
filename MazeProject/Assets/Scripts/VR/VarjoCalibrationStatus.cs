using UnityEngine;
using TMPro;
using Varjo.XR;

public class VarjoCalibrationStatus : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI statusText; // Assign in Inspector

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
        }
        else if (!VarjoEyeTracking.IsGazeCalibrated())
        {
            statusText.text = "Calibration Failed ❌";
            statusText.color = Color.red;
        }
        else
        {
            statusText.text = "Calibration Successful ✅";
            statusText.color = Color.green;
        }
    }
}
