using UnityEngine;

public class EyeTrackingDataManager : MonoBehaviour
{
    public static EyeTrackingDataManager Instance { get; private set; }

    public string UserName { get; private set; } = "DefaultUser"; // Default name
    public float LinearVelocity { get; private set; }
    public float RotationalVelocity { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {


            
            Destroy(gameObject);
        }
    }

    public void SaveData(string name, float linearVelocity, float rotationalVelocity)
    {
        UserName = name; // Store the user's name
        LinearVelocity = linearVelocity;
        RotationalVelocity = rotationalVelocity;
        Debug.Log($"Saved Data - Name: {name}, Linear Velocity: {linearVelocity}, Rotational Velocity: {rotationalVelocity}");
    }
}
