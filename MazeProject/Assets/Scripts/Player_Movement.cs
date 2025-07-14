using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private bool hasTriggered;
    public MazeEventSystem MazeSystem;
    public Camera MainCamera;
    public Camera DebugCamera;
  
    public float moveSpeed = 10f;
    public float movedrag;
    public Transform orientation;

    float horizontalInput;
    float verticalInput;




    Vector3 movedirection;

    Rigidbody rb;
    public void ShowOverheadView()
    {
        MainCamera.enabled = false;
        DebugCamera.enabled = true;
    }

    // Call this function to enable FPS camera,
    // and disable overhead camera.
    public void ShowFirstPersonView()
    {
        MainCamera.enabled = true;
        DebugCamera.enabled = false;
    }
    private void Start()
    {
        
        ShowFirstPersonView();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        hasTriggered=false;




    }

    public void returnback()
    {
        transform.position = new Vector3(0.7f, 0.7f, 0.7f);
        hasTriggered = false;
    }
    private void Update()
    {
        MyInput();
        rb.linearDamping = movedrag;
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (MainCamera.enabled == true)
            {
                ShowOverheadView();

            }
            if (MainCamera.enabled == false)
            {
                ShowFirstPersonView();
  
            }
        }
    }
    private void FixedUpdate()
    {
        horizontalInput = 0;
        verticalInput = 0;

        GetKeyboardInput();
 

        moveplayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void moveplayer()
    {
        movedirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (movedirection != Vector3.zero)
        {
            rb.AddForce(movedirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
    }
    /*

     private void FixedUpdate()
     {

     } */
    private void GetKeyboardInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");   // WASD or arrow keys
        verticalInput = Input.GetAxis("Vertical");
    }
}
