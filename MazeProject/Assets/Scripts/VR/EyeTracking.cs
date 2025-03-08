using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Varjo.XR;

public class EyeTracking : MonoBehaviour
{
    private Dictionary<string, float> gazeTimePerTag = new Dictionary<string, float>();
    private float totalSessionTime = 0f;
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "EyeTrackingData.csv");
        Debug.Log("File path set to: " + filePath);
        WriteCSVHeader();
    }

    void Update()
    {
        totalSessionTime += Time.deltaTime;
        Debug.Log("Total session time: " + totalSessionTime);
        TrackGaze();
    }

    void TrackGaze()
    {
        if (!VarjoEyeTracking.IsGazeAllowed())
        {
            Debug.LogWarning("Gaze tracking not allowed.");
            return;
        }

        if (!VarjoEyeTracking.IsGazeCalibrated())
        {
            Debug.LogWarning("Eye tracking is not calibrated.");
            return;
        }

        VarjoEyeTracking.GazeData gazeData = VarjoEyeTracking.GetGaze();

        if (gazeData.status != VarjoEyeTracking.GazeStatus.Valid)
        {
            Debug.LogWarning("Invalid gaze data.");
            return;
        }

        Debug.Log("Gaze data is valid.");

        // Get the HMD (headset) transform
        Transform hmdTransform = Camera.main.transform;

        // Compute correct gaze origin & direction in world space
        Vector3 gazeOrigin = hmdTransform.TransformPoint(gazeData.gaze.origin);
        Vector3 gazeDirection = hmdTransform.TransformDirection(gazeData.gaze.forward);

        // Visualize the gaze ray in Unity
        Debug.DrawRay(gazeOrigin, gazeDirection * 10f, Color.green, 1f);

        // Perform raycast from the gaze origin in the gaze direction
        RaycastHit hit;
        if (Physics.Raycast(gazeOrigin, gazeDirection, out hit))
        {
            string tag = hit.collider.tag;

            if (!string.IsNullOrEmpty(tag))
            {
                if (!gazeTimePerTag.ContainsKey(tag))
                {
                    gazeTimePerTag[tag] = 0f;
                }

                // Add the time the user spends looking at the object
                gazeTimePerTag[tag] += Time.deltaTime;
            }

            Debug.Log($"Hit object: {hit.collider.gameObject.name} (Tag: {tag})");
        }
        else
        {
            Debug.Log("No object hit by gaze ray.");
        }
    }


    void OnApplicationQuit()
    {
        SaveGazeData();
    }

    void WriteCSVHeader()
    {
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine("Tag,TotalTime(s),Percentage(%)");
            Debug.Log("CSV header written.");
        }
    }

    void SaveGazeData()
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            foreach (var entry in gazeTimePerTag)
            {
                float percentage = (entry.Value / totalSessionTime) * 100f;
                writer.WriteLine($"{entry.Key},{entry.Value:F2},{percentage:F2}");
                Debug.Log($"Data for tag '{entry.Key}': Time = {entry.Value:F2}, Percentage = {percentage:F2} written to CSV.");
            }
        }
        Debug.Log("Eye tracking data saved at: " + filePath);
    }
}
