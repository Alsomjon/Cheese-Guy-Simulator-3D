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

    (float, float) MovementInputHandler() {
        float v = 0;
        float h = 0;
        if (Input.GetKey(KeyCode.W)){
            v = 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            v = -1;
        }
        if (Input.GetKey(KeyCode.A)) {
            h = -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            h = 1;
        }
        if (Mathf.Abs(h) + Mathf.Abs(v) > 1) {
            h *= 0.5F;
            v *= 0.5F;
        }
        return (h, v);
    }
    void MovementHandler() {


        Rigidbody rb = GetComponent<Rigidbody>();
        var locVel = transform.InverseTransformDirection(rb.velocity);
        (float h, float v) = MovementInputHandler();
        float x = locVel.x;
        float z = locVel.z;


        if (Mathf.Abs(locVel.x) < movementSpeed) {
            x += h * acceleration;
        } 
        
        if (Mathf.Abs(locVel.z) < movementSpeed) {
            z += v * acceleration;
        } 

        locVel = new Vector3(x, locVel.y, z);
        rb.velocity = transform.TransformDirection(locVel);

    }

    void CameraHandler() {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        playerObj.transform.rotation = transform.rotation;
    }
}
