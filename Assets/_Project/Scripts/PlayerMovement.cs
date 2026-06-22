using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    
    public LayerMask groundLayer;
    public float groundCheckLength = 0.2f;

    public ParticleSystem runDust;

    [HideInInspector] public float knockbackTimer = 0f;

    private PlayerShooting playerShooting;
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    private float horizontalInput;
    private bool isGrounded;

    void Start()
    {
        playerShooting = GetComponent<PlayerShooting>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (knockbackTimer > 0f) knockbackTimer -= Time.deltaTime;

        CheckGrounded();
        HandleInput();
        UpdateVisuals();
        HandleJump();
    }

    void FixedUpdate()
    {
        if (knockbackTimer > 0f) return;

        if (playerShooting != null && playerShooting.IsFiringNow)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
    }

    private void CheckGrounded()
    {
        Vector2 rayStart = new Vector2(coll.bounds.center.x, coll.bounds.min.y + 0.05f);
        isGrounded = Physics2D.Raycast(rayStart, Vector2.down, groundCheckLength, groundLayer);
    }

    private void HandleInput()
    {
        if (playerShooting != null && playerShooting.IsFiringNow)
        {
            horizontalInput = 0f;
            return;
        }

        if (knockbackTimer <= 0f)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(horizontalInput) < 0.1f) horizontalInput = 0f;
        }
        else
        {
            horizontalInput = 0f; 
        }
    }

    private void UpdateVisuals()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f;
        
        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
        anim.SetBool("IsGrounded", isGrounded);

        if (runDust != null)
        {
            if (isGrounded && isMoving && !runDust.isPlaying) runDust.Play();
            else if ((!isGrounded || !isMoving) && runDust.isPlaying) runDust.Stop();
        }

        if (horizontalInput != 0)
        {
            Vector3 currentScale = transform.localScale;
            currentScale.x = horizontalInput > 0 ? Mathf.Abs(currentScale.x) : -Mathf.Abs(currentScale.x);
            transform.localScale = currentScale;
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && knockbackTimer <= 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
    }
}