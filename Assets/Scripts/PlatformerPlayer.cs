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

    //State variables
    private bool isClimbing;
    private bool isMoving;
    private bool isJumping;
    private bool isCrouching;

    private bool IsClimbing
    {
        get { return isClimbing; }
        set 
        { 
            isClimbing = value;

            if (isClimbing)
            {
                currentJumpCount = 0;
                rb.gravityScale = 0;
            }
            else
            {
                rb.gravityScale = 1;
            }
        }
    }
    private bool canClimb;

    //Multipliers just to make the initial values more reasonable
    private float movementSpeedMultiplier = 300f;
    private float jumpVelocityMultiplier = 5f;

    public int currentJumpCount; //To keep track of the amount of jumps since last standing on the ground

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
        JumpGravityScript();
        CheckForLadder();
        if (InputManager.current.GetInputMapping(InputMapping.Down) && !IsClimbing)
        {
            isCrouching = true;
        }
        else 
        {
            isCrouching = false;
        }
    }

    void FixedUpdate()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsClimbing", IsClimbing);
            animator.SetBool("IsJumping", isJumping);
            animator.SetBool("IsCrouching", isCrouching);
        }
    }

    void CheckForLadder()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(ladderRaycastTarget.position, Vector2.up, raycastDistance, ladderLayerMask);
        canClimb = hitInfo.collider != null;
        if (!canClimb) IsClimbing = false;
    }

    void JumpGravityScript() 
    {
        //Artificially increase the velocity on downward arch of jump, also changes jump height based on length of jump button held
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !InputManager.current.GetInputMapping(InputMapping.Jump))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    public void Jump() 
    {
        //trigger jumping but only when the player can jump I.e. has not reached the max jumps
        if (currentJumpCount < maxJumpCount && !IsClimbing)
        {
            rb.velocity = new Vector2(0, jumpVelocity * jumpVelocityMultiplier);
            currentJumpCount++;
            isJumping = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentJumpCount = 0;
        isJumping = false;
    }

    public void Move(Vector2 movement) 
    {
        //Do the movement thing
        Vector2 targetVelocity = movement * (movementSpeed * movementSpeedMultiplier) * Time.deltaTime;

        //Check whether targetVelocity includes y speed, if so then check if we can climb
        if (canClimb && targetVelocity.y != 0f) IsClimbing = true;
        isMoving = targetVelocity.x != 0f || (IsClimbing && targetVelocity.y != 0f);

        //Set sprite direction
        if (movement.x != 0)
        {
            bool directionIsNegative = movement.x < 0f;
            sr.flipX = directionIsNegative;
        }

        rb.velocity = IsClimbing ? targetVelocity : new Vector2 (targetVelocity.x, rb.velocity.y);
    }

}
