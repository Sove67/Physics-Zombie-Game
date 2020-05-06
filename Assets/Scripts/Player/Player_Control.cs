using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    // Variables
    [Header("Player Components")]
    public CharacterController controller;
    public GameObject model;
    public Animator animator;
    public Camera playerCamera;
    public Transform camX;
    public Transform camY;
    public GameObject lookIndicator;

    [Header("Control Variables")]
    public float yLimit;
    public float moveMulti;
    public float jumpMulti;
    public float gravMulti;
    public float midairMulti;
    public float rayLength;

    // Movement
    private Vector3 localInput
    {
        get
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float y = Convert.ToInt32(Input.GetButton("Jump"));
            return camX.TransformDirection(new Vector3(x, y, z));
        }
    }
    private Vector3 movement = Vector3.zero;
    private float localRot;

    // Camera
    private float xRot
    {
        get
        { return camX.localRotation.eulerAngles.y; }
        set
        { camX.localRotation = Quaternion.Euler(new Vector3(0, value, 0)); }
    }
    private float yRot
    {
        get
        { return camY.localRotation.eulerAngles.x; }
        set
        { camY.localRotation = Quaternion.Euler(new Vector3(value, 0, 0)); }
    }
    [HideInInspector] public bool onEnviroment;

    [Header("Player Settings")]
    public float sensitivity = 1;
    public bool yInvert;


    // Functions
    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        RotateCamera();
        Movement();
        Animate();
        LookPoint();
    }

    public void Movement()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        { localRot = ((Mathf.Atan2(localInput.x, localInput.z) / Mathf.PI * 180) + 360) % 360; }

        if (controller.isGrounded)
        {
            movement.x = localInput.x * moveMulti;
            movement.z = localInput.z * moveMulti;
            movement.y = localInput.y * jumpMulti;
        }
        else
        {
            movement.x = localInput.x * midairMulti;
            movement.z = localInput.z * midairMulti;
        }

        controller.Move(movement * Time.deltaTime);
        movement.y -= gravMulti * Time.deltaTime;
    }

    void RotateCamera()
    {
        float xInput = Input.GetAxis("Mouse X");
        float yInput = -Input.GetAxis("Mouse Y");

        if (yInvert)
        { yInput = -yInput; }

        xRot += (xInput * sensitivity);

        yRot = Mathf.Clamp(SplitAngle(yInput * sensitivity + yRot), -yLimit, yLimit);
    }

    void LookPoint()
    {
        Vector3 origin = camY.transform.position;
        Vector3 direction = camY.transform.forward;
        Debug.DrawRay(origin, direction * rayLength);
        Vector3 position = origin + direction * rayLength;
        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        if (Physics.Raycast(new Ray(origin, direction), out RaycastHit hit, rayLength) && hit.collider.CompareTag("Enviroment"))
        {
            onEnviroment = true;
            lookIndicator.transform.Find("Active").gameObject.SetActive(true);
            lookIndicator.transform.Find("Inactive").gameObject.SetActive(false);

            position = hit.point;
            Vector3 normal = Quaternion.LookRotation(hit.normal).eulerAngles;
            normal.x += 90;
            rotation = Quaternion.Euler(normal);
        }
        else
        {
            onEnviroment = false;
            lookIndicator.transform.Find("Active").gameObject.SetActive(false);
            lookIndicator.transform.Find("Inactive").gameObject.SetActive(true);
        }
        lookIndicator.transform.position = position;
        lookIndicator.transform.rotation = rotation;
    }

    public void Animate()
    {
        float velocity = 0;
        float rotation = xRot;

        // If Moving Backwards
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            velocity = -controller.velocity.magnitude;
            rotation = localRot - 180;
        }
        // If Strafing or Moving Forward
        else
        {
            velocity = controller.velocity.magnitude;
            rotation = localRot;
        }

        model.transform.localRotation = Quaternion.Euler(0, rotation, 0);

        animator.SetFloat("Velocity", velocity);
        animator.SetFloat("Look X", SplitAngle(xRot - localRot) / 90);
        animator.SetFloat("Look Y", -(SplitAngle(yRot) / yLimit));
    }

    public float SplitAngle(float angle) // Changes an angle from "0 to 360" to "-180 to 180"
    {
        float result = ((angle + 180 + 360) % 360 - 180);
        return result;
    }
}
