using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EyeTrackingUI : MonoBehaviour
{
    public InputField nameInput;
    public InputField linearVelocityInput;
    public InputField rotationalVelocityInput;
    public Button collectDataButton;

    private void Start()
    {
        collectDataButton.onClick.AddListener(SaveAndLoadMazeScene);
    }

    void SaveAndLoadMazeScene()
    {
        string name = nameInput.text;
        float linearVelocity = float.TryParse(linearVelocityInput.text, out float linVel) ? linVel : 0f;
        float rotationalVelocity = float.TryParse(rotationalVelocityInput.text, out float rotVel) ? rotVel : 0f;

        // Store the data
        EyeTrackingDataManager.Instance.SaveData(name, linearVelocity, rotationalVelocity);

        // Load the Maze VR Scene
        SceneManager.LoadScene("MazeScene vr");  // Replace with your actual scene name
    }
}
