using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    public float climbSpeed = 20f;
    public Animator animator;

    float horizontalMove = 0f;
    float climbMove = 0f;
    bool jump = false;
    bool doubleJump = false;
    bool crouch = false;
    bool canDJump = true;
    bool wallClimb = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        climbMove = Input.GetAxisRaw("Vertical") * climbSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        animator.SetFloat("ClimbSpeed", Mathf.Abs(climbMove));

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
        } else if (Input.GetAxis("Vertical") >= 0)
        {
            crouch = false;
        }

        if (Input.GetAxis("Climb") > 0 || Input.GetButtonDown("ClimbKey")) {
            wallClimb = true;
        } else if (Input.GetAxis("Climb") == 0 || Input.GetButtonUp("ClimbKey"))
        {
            wallClimb = false;
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsDoubleJumping", false);
        canDJump = true;
    }

    public void OnCrouching (bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);
    }

    public void OnClimbing(bool isClimbing)
    {
        animator.SetBool("IsClimbing", isClimbing);

        if (isClimbing == true) {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsCrouching", false);
            animator.SetBool("IsDoubleJumping", false);
        }
    }

    public void OnDoubleJump() {
        animator.SetBool("IsDoubleJumping", true);
    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, doubleJump, wallClimb, climbMove * Time.fixedDeltaTime);
        jump = false;
        doubleJump = false;
    }
}
