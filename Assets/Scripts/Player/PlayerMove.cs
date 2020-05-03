using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Player 
    private CharacterController charControl;
    public Transform player;
    public Transform playerXRot;

    // Movement
    private Vector3 moveDirection = Vector3.zero;
    private float localRot;
    private float modelRot;
    public float moveMulti;
    public float jumpMulti;
    public float GravMulti;

    // Animation
    public Transform model;
    public Animator animator;
    private float torsoRot;

    private void Awake()
    {
        charControl = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Movement();
        Animate();
    }
    public void Movement()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), Convert.ToInt32(Input.GetButton("Jump")), Input.GetAxis("Vertical"));
        Vector3 localInput = playerXRot.TransformDirection(moveInput.x, moveInput.y, moveInput.z);

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        { localRot = ((Mathf.Atan2(localInput.x, localInput.z) / Mathf.PI * 180) + 360) % 360; }
        

        if (charControl.isGrounded)
        {
            moveDirection = localInput;
            moveDirection.x *= moveMulti;
            moveDirection.z *= moveMulti;
            moveDirection.y *= jumpMulti;
        }

        charControl.Move(moveDirection * Time.deltaTime);
        moveDirection.y -= GravMulti * Time.deltaTime;
    }

    public void Animate()
    {
        torsoRot = ((playerXRot.rotation.eulerAngles.y - localRot + 180 + 360) % 360 - 180);

        // If Moving Forward
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            animator.SetBool("Backpedal", false);
            animator.SetBool("Moving", true);
            modelRot = localRot;
            model.transform.rotation = Quaternion.Euler(0, modelRot, 0);
        }
        // If Moving Backwards
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            animator.SetBool("Backpedal", true);
            animator.SetBool("Moving", true);
            modelRot = localRot - 180;
            model.transform.rotation = Quaternion.Euler(0, modelRot, 0);
            torsoRot = ((torsoRot + 360) % 360)-180;
        }
        // If Strafing
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            animator.SetBool("Backpedal", false);
            animator.SetBool("Moving", true);
            modelRot = localRot;
            model.transform.rotation = Quaternion.Euler(0, modelRot, 0);
        }
        // Otherwise Not Moving
        else
        {
            animator.SetBool("Backpedal", false);
            animator.SetBool("Moving", false);
            modelRot = playerXRot.rotation.eulerAngles.y;
            model.transform.rotation = Quaternion.Euler(0, modelRot, 0);
            torsoRot = 0;
        }


        animator.Play(0, 1, (0.5f - (torsoRot / 180)));
    }
}
