using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public int speed;
    public int MazeSizeX;
    public int MazeSizeY;
    [SerializeField]
    public GameObject ui;
    [SerializeField]
    public Camera Maincamera;
    [SerializeField]
    public Camera Debugcamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Maincamera.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void begin()
    {
        ui.SetActive(false);
    }
    public void Changeview()
    {
        if (Maincamera.enabled==true)
        {
            Maincamera.enabled = false;
            Debugcamera.enabled = true;

        }
        else
        {
            Maincamera.enabled = true;
            Debugcamera.enabled = false;
        }
        
    }

}
