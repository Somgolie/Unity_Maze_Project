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
        if (Varjo.XR.VarjoEyeTracking.IsGazeCalibrated())
        {
            Debug.Log("Eye tracking is calibrated.");
            Varjo.XR.VarjoEyeTracking.GazeData gazeData = VarjoEyeTracking.GetGaze();

            if (gazeData.status == VarjoEyeTracking.GazeStatus.Valid)
            {
                Debug.Log("Gaze data is valid.");
                Vector3 gazeDirection = gazeData.gaze.forward;  // Direction in which the user is looking
                Vector3 gazeOrigin = gazeData.gaze.origin;  // Position of the user's eye

                // Debugging the gaze direction and origin
                Debug.Log("Gaze direction: " + gazeDirection);
                Debug.Log("Gaze origin: " + gazeOrigin);

                // Perform raycasting from the gaze origin in the direction of the gaze direction
                RaycastHit hit;
                if (Physics.Raycast(gazeOrigin, gazeDirection, out hit))
                {
                    Debug.Log("Raycast hit something.");
                    string tag = hit.collider.tag;
                    Debug.Log("Hit object tag: " + tag);

                    if (!string.IsNullOrEmpty(tag))
                    {
                        if (!gazeTimePerTag.ContainsKey(tag))
                        {
                            gazeTimePerTag[tag] = 0f;
                        }

                        // Add the time the user spends looking at the object
                        gazeTimePerTag[tag] += Time.deltaTime;
                        Debug.Log("Gaze time for tag " + tag + ": " + gazeTimePerTag[tag]);
                    }
                }
                else
                {
                    // If the raycast doesn't hit anything, draw a debug ray
                    Debug.Log("Raycast did not hit anything.");
                    Debug.DrawRay(gazeOrigin, gazeDirection * 10f, Color.blue, 0.1f);
                }
            }
            else
            {
                Debug.Log("Gaze data is not valid.");
            }
        }
        else
        {
            Debug.Log("Eye tracking is not calibrated.");
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
            Debug.Log("Saving gaze data...");
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