using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 1;

    [Header("Jumping")]
    public float jumpVelocity = 2;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public int maxJumpCount = 1;

    [Header("Climbing")]
    public LayerMask ladderLayerMask;
    public Transform ladderRaycastTarget;
    public float raycastDistance; //This is to check for ladders

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;

    private float horizontalValue;
    private float verticalValue;

    //State variables
    private bool isClimbing;
    private bool isMoving;
    private bool isJumping;
    private bool isWalking;

    //Multipliers just to make the initial values more reasonable
    private float movementSpeedMultiplier = 300f;
    private float jumpVelocityMultiplier = 5f;
    
    private int currentJumpCount; //To keep track of the amount of jumps since last standing on the ground

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (GameManager.current != null)
        {
            GameManager.current.player = this;
        }
    }

    void Update()
    {
        ReadInput();
        CheckForLadderAndInput();
    }

    void FixedUpdate()
    {
        //Vertical Movement (Walking)
        Move(MovementOrientation.Horizontal, horizontalValue);
        
        //Horizontal Movement (Climbing)
        if (isClimbing)
        {
            currentJumpCount = 0;
            Move(MovementOrientation.Vertical, verticalValue);
            rb.gravityScale = 0;
        }
        else 
        {
            rb.gravityScale = 1;
        }

        //isFalling = rb.velocity.y != 0;


        UpdateAnimator();
    }

    private void UpdateAnimator() 
    {

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsClimbing", isClimbing);
            animator.SetBool("IsJumping", isJumping);
            animator.SetBool("IsWalking", isWalking);
            //animator.SetBool("IsFalling", isFalling);
        }
    }

    void CheckForLadderAndInput() 
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(ladderRaycastTarget.position, Vector2.up, raycastDistance, ladderLayerMask);

        if (hitInfo.collider != null)
        {
            if (verticalValue > 0) isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
    }

    void ReadInput() 
    {
        horizontalValue = Input.GetAxisRaw("Horizontal");
        verticalValue = Input.GetAxisRaw("Vertical");
        isWalking = Input.GetKey("a") || Input.GetKey("d");
        isMoving = isWalking || Input.GetKey("w") || Input.GetKey("s");

        if (Input.GetKey("s")) 
        { 
            isJumping = true; 
        }
        else if (rb.velocity.y == 0f)
        {
            isJumping = false;
        }

        //Jump
        JumpScript();
    }


    void JumpScript() 
    {

        //trigger jumping but only when the player can jump I.e. has not reached the max jumps
        if (Input.GetButtonDown("Jump") && currentJumpCount < maxJumpCount && !isClimbing)
        {
            rb.velocity = new Vector2(0, jumpVelocity * jumpVelocityMultiplier);
            currentJumpCount++;
            isJumping = true;
        }
        else if (isClimbing) 
        {
            currentJumpCount = 0;
            isJumping = false;
        }

        //Artificially increase the velocity on downward arch of jump, also changes jump height based on length of jump button held
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Player Collided with : " + collision.gameObject.name);
        //https://docs.unity3d.com/ScriptReference/Physics2D.OverlapCapsule.html ?
        //For resetting jump
        currentJumpCount = 0;
        isJumping = false;
    }

    void Move(MovementOrientation movementOrientation, float direction) 
    {
        //Ensure movement is consistent regardless of framerate by tying to Time.deltaTime
        float movementValue = direction * (movementSpeed * movementSpeedMultiplier) * Time.deltaTime;

        //Set horizontal or vertical velocity based on the set orientation
        Vector2 targetVelocity = movementOrientation == MovementOrientation.Horizontal 
            ? new Vector2(movementValue, rb.velocity.y) : new Vector2(rb.velocity.x, movementValue);

        //Set sprite direction if horizontal movement occurs
        if (movementOrientation == MovementOrientation.Horizontal && direction != 0) 
        {
            bool directionIsNegative = direction < 0f;
            sr.flipX = directionIsNegative;
        }


        //Set the final velocity
        rb.velocity = targetVelocity;
    }
}

public enum MovementOrientation 
{
    Horizontal,
    Vertical
}
