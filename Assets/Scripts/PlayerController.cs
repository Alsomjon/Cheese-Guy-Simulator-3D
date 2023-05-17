using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Camera playerCam;
    public GameObject playerObj;

    [Header("Camera Script")]
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    float rotationX = 0;

    [Header("Movement Script")]
    public float maxSpeed;
    public float acceleration;
    public float movementSpeed;
    void Update()
    {
        CameraHandler();
        MovementHandler();
    }

    void MovementHandler() {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Rigidbody rb = GetComponent<Rigidbody>();
        float magnitude = rb.velocity.magnitude;
        if !()
        
    }

    void CameraHandler() {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        playerObj.transform.rotation = transform.rotation;
    }
}
