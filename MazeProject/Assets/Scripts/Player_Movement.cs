using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float movedrag;
    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 movedirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        rb.linearDamping = movedrag;
    }
    private void FixedUpdate()
    {
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
        rb.AddForce(movedirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }
    /*    private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed;

            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        }*/
}
