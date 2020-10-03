using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    public Animator animator;

    float horizontalMove = 0f;
    bool jump = false;
    bool doubleJump = false;
    bool crouch = false;
    bool canDJump = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
            if (!controller.getGrounded() && canDJump) {
                doubleJump = true;
                canDJump = false;
            }
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            crouch = true;
        } else if (Input.GetAxis("Vertical") >= 0 || Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
        canDJump = true;
    }

    public void OnCrouching (bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);
    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, doubleJump);
        jump = false;
        doubleJump = false;
    }
}
