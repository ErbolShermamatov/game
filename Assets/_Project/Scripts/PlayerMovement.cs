using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public ParticleSystem runDust;

    public LayerMask groundLayer;
    public float groundCheckLength = 0.2f;

    private PlayerAiming playerAiming;
    private PlayerShooting playerShooting;
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    private float horizontalInput;
    private bool isGrounded;
    
    public float knockbackTimer = 0f;

    void Start()
    {
        playerAiming = GetComponent<PlayerAiming>();
        playerShooting = GetComponent<PlayerShooting>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
        }

        Vector2 rayStart = new Vector2(coll.bounds.center.x, coll.bounds.min.y + 0.05f);
        isGrounded = Physics2D.Raycast(rayStart, Vector2.down, groundCheckLength, groundLayer);

        if (playerShooting != null && playerShooting.IsFiringNow)
        {
            horizontalInput = 0f;
            anim.SetFloat("Speed", 0f);
            if (runDust != null && runDust.isPlaying) runDust.Stop();
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

        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f;
        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
        anim.SetBool("IsGrounded", isGrounded);

        if (runDust != null)
        {
            if (isGrounded && isMoving) { if (!runDust.isPlaying) runDust.Play(); }
            else { if (runDust.isPlaying) runDust.Stop(); }
        }

        if (Input.GetButtonDown("Jump") && isGrounded && knockbackTimer <= 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }

        Vector3 currentScale = transform.localScale;
        if (horizontalInput > 0) currentScale.x = Mathf.Abs(currentScale.x);
        else if (horizontalInput < 0) currentScale.x = -Mathf.Abs(currentScale.x);
        transform.localScale = currentScale;
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
}