using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;//How much time the player can hang in the air before jumping
    private float coyoteCounter; //How much time passed since the player ran off the edge

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX; //Horizontal wall jump force
    [SerializeField] private float wallJumpY; //Vertical wall jump force

    [Header("Dash")]
    [SerializeField] private bool canDash = true, isDashing;
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;

    [Header("Phase Through Layers")]
    [SerializeField] private LayerMask phaseThroughLayers;

    private int originalLayer;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    [SerializeField] private TrailRenderer trail;
    // private float wallJumpCooldown;
    private float horizontalInput;

    [Header("SFX")]
    [SerializeField] private AudioClip jumpSound;

    private void Awake()
    {
        //Grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        body.freezeRotation = true; // Ngăn nhân vật xoay

        originalLayer = gameObject.layer;
    }

    private void Update()
    {
        if (isDashing) return;

        horizontalInput = Input.GetAxis("Horizontal");

        //Flip player when moving left-right
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        // Adjustable jump height
        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y / 2);
        }

        if (onWall())
        {
            body.gravityScale = 0;
            body.linearVelocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime; //Reset coyote counter when on the ground
                jumpCounter = extraJumps; //Reset jump counter to extra jump value
            }
            else
                coyoteCounter -= Time.deltaTime; //Start decreasing coyote counter when not on the ground
        }
        //Dash
        if(Input.GetKeyDown(KeyCode.K) && canDash) StartCoroutine(Dash());
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0) return;
        //If coyote counter is 0 or less and not on the wall and don't have any extra jumps don't do anything

        SoundManager.instance.PlaySound(jumpSound);


        if (onWall())
            WallJump();
        else
        {
            if (isGrounded())
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            }
            else
            {
                //If not on the ground and coyote counter bigger than 0 do a normal jump
                if (coyoteCounter > 0)
                    body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
                else
                {
                    if (jumpCounter > 0) //If we have extra jumps then jump and decrease the jump counter
                    {
                        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
                        jumpCounter--;
                    }
                }
            }
            //Reset coyote counter to 0 to avoid double jumps
            coyoteCounter = 0;
        }
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        //wallJumpCooldown = 0;
    }


    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }

    // Thêm phương thức để bật/tắt collision với các layer được chỉ định
    private void SetPhaseThrough(bool phaseThrough)
    {
        if (phaseThrough)
        {
            // Tạo layer riêng cho player khi dash (ví dụ layer 8 - "DashingPlayer")
            gameObject.layer = 8; // Đảm bảo bạn đã tạo layer này trong Unity

            // Vô hiệu hóa collision với các layer được chỉ định
            for (int i = 0; i < 32; i++)
            {
                if ((phaseThroughLayers.value & (1 << i)) != 0)
                {
                    Physics2D.IgnoreLayerCollision(8, i, true);
                }
            }
        }
        else
        {
            // Khôi phục layer ban đầu của player
            gameObject.layer = originalLayer;

            // Khôi phục collision
            for (int i = 0; i < 32; i++)
            {
                if ((phaseThroughLayers.value & (1 << i)) != 0)
                {
                    Physics2D.IgnoreLayerCollision(8, i, false);
                }
            }
        }
    }

    private IEnumerator Dash() 
    {
        canDash = false;
        isDashing = true;
        float originalGravity = body.gravityScale;
        body.gravityScale = 0f;
        body.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        trail.emitting = true;
        SetPhaseThrough(true);
        yield return new WaitForSeconds(dashingTime);
        SetPhaseThrough(false);
        trail.emitting = false;
        body.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}