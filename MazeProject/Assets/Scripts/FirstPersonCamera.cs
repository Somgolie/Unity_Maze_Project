using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float senX;
    public float senY;

    public Transform Orientation;

    float xRotation;
    float yRotation;

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //rotate cam and player
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    /*    public Transform Player;
        public float Mouse_sensitivity = 2f;
        float Cam_Vertical_Rotation = 0f;

        private void Update()
        {
            //Get mouse input
            float input_X = Input.GetAxis("Mouse X")* Mouse_sensitivity;
            float input_Y = Input.GetAxis("Mouse Y")* Mouse_sensitivity;

            //rotate camera
            Cam_Vertical_Rotation -= input_X;

            //clamp camera
            Cam_Vertical_Rotation = Mathf.Clamp(Cam_Vertical_Rotation, -90f, 90f);
            transform.localEulerAngles = Vector3.right * Cam_Vertical_Rotation;

            Player.Rotate(Vector3.up * input_X);
        }*/
}
