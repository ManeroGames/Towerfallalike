using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
    [SerializeField] private float m_DoubleJumpForce = 400f;					// Amount of force added when the player double jumps.
	[SerializeField] private Vector2 m_WallJumpForce = new Vector2(400f, 400f);
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(1, 5)] [SerializeField] private float m_CrouchFallingSpeed = 2f;
	[Range(1, 5)] [SerializeField] private float m_ClimbFallingSpeed = 0.1f;
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Transform m_FrontCheck;							// A position marking where to check for walls
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	const float k_FrontRadius = .1f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public BoolEvent OnClimbingEvent;
	public UnityEvent OnDoubleJumpEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;
	private bool isClimbing = false;

	private float defaultGravity;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		defaultGravity = m_Rigidbody2D.gravityScale;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		if (OnClimbingEvent == null)
			OnClimbingEvent = new BoolEvent();

		if (OnDoubleJumpEvent == null)
			OnDoubleJumpEvent = new UnityEvent();
	}

	public bool getGrounded() {
		return m_Grounded;
	}

	public void Move(float move, bool crouch, bool jump, bool doubleJump, bool wall, float climb)
	{
		// Check if grounded
		if (Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround))
		{
			if (!m_Grounded)
			{
				m_Grounded = true;
				OnLandEvent.Invoke();
			}

			/*isClimbing = false;
			OnClimbingEvent.Invoke(isClimbing);*/
		}

		// Climb mechanics
		bool nextToWall = Physics2D.OverlapCircle(m_FrontCheck.position, k_FrontRadius, m_WhatIsGround);
		if (wall && nextToWall)
		{
		 	/*m_Rigidbody2D.AddForce(Physics.gravity * m_ClimbFallingSpeed);*/
			m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
			/*m_Rigidbody2D.gravityScale = defaultGravity * m_ClimbFallingSpeed;*/
			isClimbing = true;
			move = 0;
			OnClimbingEvent.Invoke(isClimbing);
			if (jump) {
				WallJump();
			}
		} else
		{
			isClimbing = false;
			m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
			m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
			/*m_Rigidbody2D.gravityScale = defaultGravity;*/
			OnClimbingEvent.Invoke(isClimbing);
		}

		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				if (m_Grounded)
					crouch = true;
			}
		}
		
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching	
			if (crouch && !m_Grounded && !wall) 
			{
				m_Rigidbody2D.AddForce(Physics.gravity * m_CrouchFallingSpeed);
			}
			if (crouch && m_Grounded)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump && !wall)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
		
		if (doubleJump && !wall) // If the player should double jump...
        {
            // Add a vertical force to the player.
			m_Rigidbody2D.AddForce(new Vector2(0f, m_DoubleJumpForce));
			OnDoubleJumpEvent.Invoke();
        }
	}

	private void WallJump() {
		m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
		m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
		Flip();
		m_Rigidbody2D.AddForce(m_WallJumpForce);
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		transform.Rotate(0f, 180f, 0f);
	}
}