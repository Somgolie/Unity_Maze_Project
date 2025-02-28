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
        if (VarjoEyeTracking.IsGazeCalibrated())
        {
            Debug.Log("Eye tracking is calibrated.");
            VarjoEyeTracking.GazeData gazeData = VarjoEyeTracking.GetGaze();

            if (gazeData.status == VarjoEyeTracking.GazeStatus.Valid)
            {
                Debug.Log("Gaze data is valid.");

                // Get the camera's position and rotation
                Vector3 cameraPosition = Camera.main.transform.position;
                Quaternion cameraRotation = Camera.main.transform.rotation;

                // Calculate the gaze direction using the eye tracking data
                Vector3 gazeDirection = gazeData.gaze.forward;  // Direction in which the user is looking
                Vector3 gazeOrigin = - gazeData.gaze.origin;  // Position of the user's eye

                // Use the camera's rotation to adjust the gaze direction
                Vector3 adjustedGazeDirection =  cameraRotation * - gazeDirection;

                // Visualize the raycast in the scene view using Debug.DrawRay
                Debug.DrawRay(cameraPosition, adjustedGazeDirection * 10f, Color.red, 0.1f);

                // Perform raycasting from the camera's position in the direction of the adjusted gaze direction
                RaycastHit hit;
                if (Physics.Raycast(cameraPosition, adjustedGazeDirection, out hit))
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
                }
                else
                {
                    // If the raycast doesn't hit anything, draw a debug ray
                    Debug.DrawRay(cameraPosition, adjustedGazeDirection * 10f, Color.blue, 0.1f);
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
