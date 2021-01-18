using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerPlayer : MonoBehaviour
{
    public float movementSpeed = 1;
    public float jumpVelocity = 2;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public int maxJumpCount = 1;

    private Rigidbody2D rb;

    private float horizontalValue;


    //Multipliers just to make the initial values more reasonable
    private float movementSpeedMultiplier = 300f;
    private float jumpVelocityMultiplier = 5f;
    
    private int currentJumpCount; //To keep track of the amount of jumps since last standing on the ground

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ReadInput();
    }

    void FixedUpdate()
    {
        Move(horizontalValue);
    }

    void ReadInput() 
    {
        horizontalValue = Input.GetAxisRaw("Horizontal");
        //Jump
        JumpScript();
    }


    void JumpScript() 
    {

        //trigger jumping but only when the player can jump I.e. has not reached the max jumps
        if (Input.GetButtonDown("Jump") && currentJumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(0, jumpVelocity * jumpVelocityMultiplier);
            currentJumpCount++;
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

        //Reset jump count if y velocity hits 0, i.e. player is no longer falling or jumping
        if (rb.velocity.y == 0f) 
        {
            currentJumpCount = 0;
        }
    }

    void Move(float dir)
    {
        //Ensure movement is consistent regardless of framerate by tying to Time.deltaTime
        float xVal = dir * (movementSpeed * movementSpeedMultiplier) * Time.deltaTime;

        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        rb.velocity = targetVelocity;
    }
}
