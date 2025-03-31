using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Varjo.XR;
using UnityEngine.UI;

public class EyeTracking : MonoBehaviour
{
    public GameObject uiPrefab; // Assign the UI prefab
    public GameObject leftController; // Assign the left controller in the inspector
    public GameObject rightController; // Assign the right controller in the inspector

    public SliderTextUpdater sliderTextUpdater; // Reference to the SliderTextUpdater script

    private GameObject uiInstance;
    private Button startButton;
    private Text confidenceText;

    private bool isTracking = false;
    private string confidenceLevel = "0"; // Store as string for CSV output

    private Dictionary<string, float> gazeTimePerTag = new Dictionary<string, float>();
    private Dictionary<string, int> gazeCountPerTag = new Dictionary<string, int>();
    private string lastGazeTag = "";
    private float totalSessionTime = 0f;
    private string filePath;
    private string userName = "DefaultUser";

    void Start()
    {
        // Instantiate UI in front of the player
        if (uiPrefab != null)
        {
            uiInstance = Instantiate(uiPrefab);
            PositionUIInFront(); // Position the UI correctly

            startButton = uiInstance.GetComponentInChildren<Button>();
            GameObject confidenceTextObject = GameObject.Find("Header Text (1)");

            if (confidenceTextObject != null)
            {
                confidenceText = confidenceTextObject.GetComponent<Text>();
            }
            else
            {
                Debug.LogError("Header Text (1) GameObject not found!");
            }

            if (startButton != null)
                startButton.onClick.AddListener(StartTracking);
        }
        else
        {
            Debug.LogError("UI Prefab not assigned!");
        }

        // Ensure controllers are active at the start
        SetControllersActive(true);

        InitializeUserName();
        InitializeFilePath();
        WriteCSVHeader();
    }

    void Update()
    {
        if (!isTracking) return;

        totalSessionTime += Time.deltaTime;
        TrackGaze();
    }

    public void StartTracking()
    {
        isTracking = true;

        // Hide UI and controllers after pressing Start
        if (uiInstance != null) uiInstance.SetActive(false);
        SetControllersActive(false); // Disable controllers after starting

        // Get the LATEST confidence level just before tracking starts
        if (sliderTextUpdater != null)
        {
            confidenceLevel = sliderTextUpdater.GetConfidenceValue(); // ✅ Now it fetches the most recent value
            Debug.Log($"Tracking started with confidence level: {confidenceLevel}"); // Log to confirm
        }
        else
        {
            Debug.LogError("SliderTextUpdater reference is missing!");
        }
    }

    void TrackGaze()
    {
        if (!VarjoEyeTracking.IsGazeAllowed() || !VarjoEyeTracking.IsGazeCalibrated()) return;

        VarjoEyeTracking.GazeData gazeData = VarjoEyeTracking.GetGaze();
        if (gazeData.status != VarjoEyeTracking.GazeStatus.Valid) return;

        Transform hmdTransform = Camera.main.transform;
        Vector3 gazeOrigin = hmdTransform.TransformPoint(gazeData.gaze.origin);
        Vector3 gazeDirection = hmdTransform.TransformDirection(gazeData.gaze.forward);

        Debug.DrawRay(gazeOrigin, gazeDirection * 10f, Color.green, 1f);

        if (Physics.Raycast(gazeOrigin, gazeDirection, out RaycastHit hit))
        {
            string tag = hit.collider.tag;
            if (!string.IsNullOrEmpty(tag))
            {
                UpdateGazeTime(tag);
                TrackGazeTransition(tag);
                lastGazeTag = tag;
            }
        }
    }

    void UpdateGazeTime(string tag)
    {
        if (!gazeTimePerTag.ContainsKey(tag)) gazeTimePerTag[tag] = 0f;
        gazeTimePerTag[tag] += Time.deltaTime;
    }

    void TrackGazeTransition(string tag)
    {
        if (tag != lastGazeTag)
        {
            if (!gazeCountPerTag.ContainsKey(tag)) gazeCountPerTag[tag] = 0;
            gazeCountPerTag[tag]++;
        }
    }

    void SaveGazeDataToCSV(string mazeFileName)
    {
        string csvFilePath = Path.Combine(Application.persistentDataPath, $"{userName}_EyeTrackingData.csv");

        HashSet<string> allPossibleTags = new HashSet<string>();
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (!string.IsNullOrEmpty(obj.tag) && obj.tag != "Untagged")
            {
                allPossibleTags.Add(obj.tag);
            }
        }

        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            writer.WriteLine($"File: {mazeFileName}, Confidence Level: {confidenceLevel}"); // ✅ Save confidence level as string

            foreach (string tag in allPossibleTags)
            {
                if (tag != "MainCamera" && tag != "Player")
                {
                    float totalTime = gazeTimePerTag.ContainsKey(tag) ? gazeTimePerTag[tag] : 0f;
                    int gazeCount = gazeCountPerTag.ContainsKey(tag) ? gazeCountPerTag[tag] : 0;
                    float percentage = totalSessionTime > 0 ? (totalTime / totalSessionTime) * 100f : 0f;

                    writer.WriteLine($"{tag},{totalTime:F2},{percentage:F2},{gazeCount}");
                }
            }

            writer.WriteLine($"Total Time of session: {totalSessionTime:F2}s\n");
            Debug.Log($"Gaze data for {mazeFileName} saved to CSV.");
        }
    }

    void OnApplicationQuit()
    {
        SaveGazeDataToCSV("Maze1.txt");
    }

    void InitializeFilePath()
    {
        filePath = Path.Combine(Application.persistentDataPath, $"{userName}_EyeTrackingData.csv");
    }

    void InitializeUserName()
    {
        if (EyeTrackingDataManager.Instance != null && !string.IsNullOrEmpty(EyeTrackingDataManager.Instance.UserName))
        {
            userName = EyeTrackingDataManager.Instance.UserName;
        }
    }

    void WriteCSVHeader()
    {
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine("Tag,TotalTime(s),Percentage(%),GazeCount,Confidence Level");
        }
    }

    /// <summary>
    /// Positions the UI in front of the player's camera.
    /// </summary>
    void PositionUIInFront()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            uiInstance.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 2.0f; // 2 meters in front
            uiInstance.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }
        else
        {
            Debug.LogError("Main Camera not found.");
        }
    }

    /// <summary>
    /// Enables or disables the controllers.
    /// </summary>
    void SetControllersActive(bool isActive)
    {
        Debug.Log($"SetControllersActive called with {isActive}");

        if (leftController != null)
        {
            leftController.SetActive(isActive);
            Debug.Log("Left controller state changed");
        }
        else
        {
            Debug.LogError("Left controller is NULL!");
        }

        if (rightController != null)
        {
            rightController.SetActive(isActive);
            Debug.Log("Right controller state changed");
        }
        else
        {
            Debug.LogError("Right controller is NULL!");
        }
    }
}
