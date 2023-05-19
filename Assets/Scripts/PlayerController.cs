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
    public float runModifier;
    public float crouchModifier;
    public float slideModifier;
    public float slideDragModifier;

    public enum MovementState {
        Crouching,
        Walking,
        Running,
        Sliding,
    }
    public MovementState movementState = MovementState.Walking;
    public enum AerialState {
        Grounded,
        Air
    }
    
    public AerialState aerialState = AerialState.Air;

    [Header("Slope Handler")]

    public float minSlope;

    public float maxSlope;
    
    Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }
    void Update() {
        CameraHandler();
        MovementHandler();
        CalculateSlope();
    }



    void Crouch() {
        if (movementState != MovementState.Crouching){
            // Put the player lower so he is properly on the ground when crouching
            MeshRenderer renderer = playerObj.GetComponent<MeshRenderer>();
            float height = renderer.bounds.size.y;
            height /= 4;
            Vector3 pos = transform.position;
            pos.y -= height;
            transform.position = pos;

            // Scales the player down so his hitbox is smaller and consistent with a crouched person
            Vector3 scale = transform.localScale;
            scale.y /= 2;
            transform.localScale = scale;
            
        }
    }
    void UnCrouch() {
        if (movementState == MovementState.Crouching){
            // Scales the player back up to normal size
            Vector3 scale = transform.localScale;
            scale.y *= 2;
            transform.localScale = scale;

            // Puts the player back to normal position, stopping them from spamming crouch to go down.
            MeshRenderer renderer = playerObj.GetComponent<MeshRenderer>();
            float height = renderer.bounds.size.y;
            height /= 4;
            Vector3 pos = transform.position;
            pos.y += height;
            transform.position = pos;
        }
    }

    (float, float) CalculateSlope() {
        RaycastHit rchit;
        Physics.Raycast(transform.position, -Vector3.up, out rchit);
        float slopeAngle = Vector3.Angle(Vector3.up, rchit.normal);
        float slopeDirection = Mathf.Atan2(rchit.normal.x, rchit.normal.z) * Mathf.Rad2Deg;
        return (slopeAngle, slopeDirection);
    }
  

    (float, float) MovementInputHandler() {
        float v = 0;
        float h = 0;
        // Basic WASD movement
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
        // If we move diagonally, slow the player down
        if (Mathf.Abs(h) + Mathf.Abs(v) > 1) {
            h *= 0.5F;
            v *= 0.5F;
        }
        // Handles setting movementStates and changing velocities based on those.
        (float slope, float slopeDirection) = CalculateSlope();
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && v >= 1) {
            if (movementState != MovementState.Sliding) {
                rb.drag = slideDragModifier;
                rb.AddForce(transform.forward * slideModifier, ForceMode.Impulse);
                movementState = MovementState.Sliding;
                if (slopeDirection < 0) {
                    slopeDirection += 360;
                }
            }
        } else {
            rb.drag = movementSpeed;
            if (Input.GetKey(KeyCode.LeftControl)) {
                Crouch();
                movementState = MovementState.Crouching;
                h *= crouchModifier;
                v *= crouchModifier;
            } else {
                UnCrouch();
                if (Input.GetKey(KeyCode.LeftShift)) {
                    movementState = MovementState.Running;
                    if (v > 0) {
                        v *= runModifier;
                    }
                } else {
                    movementState = MovementState.Walking;
                }
            }
        }


        // Handles changing velocity on a slope
        return (h, v);
    }
    void MovementHandler() {

        rb.drag = movementSpeed;
        var locVel = transform.InverseTransformDirection(rb.velocity);
        (float h, float v) = MovementInputHandler();
        float x = locVel.x;
        float z = locVel.z;


        rb.AddForce(transform.forward * (v * acceleration), ForceMode.Impulse);
        rb.AddForce(transform.right * (h * acceleration), ForceMode.Impulse);
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
