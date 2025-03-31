using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Varjo.XR;

public class EyeTracking : MonoBehaviour
{
    // Store gaze time and count per tag
    private Dictionary<string, float> gazeTimePerTag = new Dictionary<string, float>();
    private Dictionary<string, int> gazeCountPerTag = new Dictionary<string, int>();

    // Track the last gaze tag
    private string lastGazeTag = "";

    // Total session time and file path
    private float totalSessionTime = 0f;
    private string filePath;

    // Default user name, can be set from EyeTrackingDataManager if available
    private string userName = "DefaultUser";

    // Called when the script starts
    void Start()
    {
        InitializeUserName();
        InitializeFilePath();
        WriteCSVHeader();
    }

    // Called every frame to update the gaze data
    void Update()
    {
        totalSessionTime += Time.deltaTime;
        TrackGaze();
    }

    // Track gaze and update gaze time and gaze count for each tag
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

        // Get the gaze origin and direction in world space
        Transform hmdTransform = Camera.main.transform;
        Vector3 gazeOrigin = hmdTransform.TransformPoint(gazeData.gaze.origin);
        Vector3 gazeDirection = hmdTransform.TransformDirection(gazeData.gaze.forward);

        Debug.DrawRay(gazeOrigin, gazeDirection * 10f, Color.green, 1f);

        // Cast a ray from the gaze and check for hit objects
        RaycastHit hit;
        if (Physics.Raycast(gazeOrigin, gazeDirection, out hit))
        {
            string tag = hit.collider.tag;

            if (!string.IsNullOrEmpty(tag))
            {
                // Update gaze time for the tag
                UpdateGazeTime(tag);

                // Track the gaze count on new tag transitions
                TrackGazeTransition(tag);

                // Update the last gaze tag
                lastGazeTag = tag;

                Debug.Log($"Hit object: {hit.collider.gameObject.name} (Tag: {tag})");
            }
        }
        else
        {
            Debug.Log("No object hit by gaze ray.");
        }
    }

    // Update the total gaze time for the current tag
    void UpdateGazeTime(string tag)
    {
        if (!gazeTimePerTag.ContainsKey(tag))
        {
            gazeTimePerTag[tag] = 0f;
        }
        gazeTimePerTag[tag] += Time.deltaTime;
    }

    // Track gaze transition between tags
    void TrackGazeTransition(string tag)
    {
        if (tag != lastGazeTag)
        {
            if (!gazeCountPerTag.ContainsKey(tag))
            {
                gazeCountPerTag[tag] = 0;
            }
            gazeCountPerTag[tag]++;
        }
    }

    // Called when the application quits
    void OnApplicationQuit()
    {
        SaveGazeDataToCSV("Maze1.txt"); // Replace with actual maze filename
    }

    // Initialize the file path for the CSV data
    void InitializeFilePath()
    {
        filePath = Path.Combine(Application.persistentDataPath, $"{userName}_EyeTrackingData.csv");
        Debug.Log("File path set to: " + filePath);
    }

    // Initialize the user's name if available
    void InitializeUserName()
    {
        if (EyeTrackingDataManager.Instance != null && !string.IsNullOrEmpty(EyeTrackingDataManager.Instance.UserName))
        {
            userName = EyeTrackingDataManager.Instance.UserName;
        }
    }

    // Write the CSV header at the beginning
    void WriteCSVHeader()
    {
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine("Tag,TotalTime(s),Percentage(%),GazeCount");
            Debug.Log("CSV header written.");
        }
    }

    // Save the gaze data to a CSV file
    void SaveGazeDataToCSV(string mazeFileName)
    {
        string csvFilePath = Path.Combine(Application.persistentDataPath, $"{userName}_EyeTrackingData.csv");

        // Open the CSV file in append mode
        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            // Write the maze file name header
            writer.WriteLine($"File: {mazeFileName}");
            
            // Write the gaze data for each tag
            foreach (var entry in gazeTimePerTag)
            {
                float percentage = (entry.Value / totalSessionTime) * 100f;
                int gazeCount = gazeCountPerTag.ContainsKey(entry.Key) ? gazeCountPerTag[entry.Key] : 0;
                writer.WriteLine($"{entry.Key},{entry.Value:F2},{percentage:F2},{gazeCount}");
            }
            
            // Write the total session time
            writer.WriteLine($"Total Time of session: {totalSessionTime:F2}s");
            
            // Add a separator line between maze entries
            writer.WriteLine();
            
            Debug.Log($"Gaze data for {mazeFileName} saved to CSV.");
        }
    }
}
