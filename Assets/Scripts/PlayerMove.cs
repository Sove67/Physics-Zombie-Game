using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController charControl;
    public Transform player;
    public Transform model;
    public Transform playerXRot;
    public Animator animator;
    public float localRot;
    public float TorsoRot;
    public float modelRot;
    public int lookMargin;
    public float moveMulti = 40f;
    public float jumpMulti = 20f;
    public float GravMulti = 20f;
    private Vector3 moveDirection = Vector3.zero;

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
        Vector3 localInput = playerXRot.TransformDirection(moveInput.x, 0, moveInput.z);

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        { localRot = ((Mathf.Atan2(localInput.x, localInput.z) / Mathf.PI * 180) + 360) % 360; }

        TorsoRot = ((localRot - playerXRot.rotation.eulerAngles.y) + 360 + 90) % 360;

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
        float oldModelRot = modelRot;

        if (((TorsoRot + 1) % 360) - 1 >= (-lookMargin) % 360 && ((TorsoRot + 1) % 360) - 1 < (180 + lookMargin) % 360)
        {
            animator.SetBool("Backpedal", false);
            modelRot = localRot;
            model.transform.rotation = Quaternion.Euler(0, modelRot, 0);
        }
        else
        {
            animator.SetBool("Backpedal", true);
            modelRot = localRot - 180;
            model.transform.rotation = Quaternion.Euler(0, modelRot - 180, 0);
        }

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        { animator.SetBool("Moving", true); }
        else { animator.SetBool("Moving", false); }
        // animate torso turn to modelRot from old modelRot
    }
}
