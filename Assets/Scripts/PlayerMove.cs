using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController charControl;
    public Transform playerDirection;
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
        if (charControl.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = playerDirection.TransformDirection(moveDirection);
            moveDirection *= moveMulti;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpMulti;
        }
        charControl.Move(moveDirection * Time.deltaTime);
        moveDirection.y -= GravMulti * Time.deltaTime;
        
    }
}
