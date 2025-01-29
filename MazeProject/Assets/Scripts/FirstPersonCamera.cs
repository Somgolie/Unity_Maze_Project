using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform Player;
    public float Mouse_sensitivity = 2f;
    float Cam_Vertical_Rotation = 0f;

    private void Update()
    {
        //Get mouse input
        float input_X = Input.GetAxis("Mouse X")* Mouse_sensitivity;
        float input_Y = Input.GetAxis("Mouse Y")* Mouse_sensitivity;

        //rotate camera
        Cam_Vertical_Rotation -= input_Y;
 
        //clamp camera
        Cam_Vertical_Rotation = Mathf.Clamp(Cam_Vertical_Rotation, -90f, 90f);
        transform.localEulerAngles = Vector3.right * Cam_Vertical_Rotation;

        Player.Rotate(Vector3.up * input_X);
    }
}
