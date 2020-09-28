using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float normalJumpForce;
    public float doubleJumpForce;
    public float wallJumpForce;

    private Rigidbody2D rig;
    private bool onAir;
    private bool onGround;
    private bool onWall;
    private bool doubleJump;
    private bool wallJump;
        
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        Vector3 movementX = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movementX * Time.deltaTime * speed;
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump"))
        {
            if  (!onAir)
            {
                NormalJump();
            }
            else if (!doubleJump)
            {
                DoubleJump();
            }
        }
    }

    void NormalJump()
    {
        rig.AddForce(new Vector2(0f, normalJumpForce), ForceMode2D.Impulse);
    }

    void DoubleJump()
    {
        doubleJump = true;
        rig.AddForce(new Vector2(0f, doubleJumpForce), ForceMode2D.Impulse);
    }

    void WallJump()
    {
        wallJump = true;
        rig.AddForce(new Vector2(0f, wallJumpForce), ForceMode2D.Impulse);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Ground")
        {
            onAir = false;
            onGround = true;
            doubleJump = false;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Ground")
        {
            onGround = false;
            onAir = true;
        }
    }
}
