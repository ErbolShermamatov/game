using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public ParticleSystem runDust;

    private PlayerAiming playerAiming;
    private PlayerShooting playerShooting;
    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    private bool isGrounded;

    void Start()
    {
        playerAiming = GetComponent<PlayerAiming>();
        playerShooting = GetComponent<PlayerShooting>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerShooting != null && playerShooting.IsFiringNow)
        {
            horizontalInput = 0f;
            anim.SetFloat("Speed", 0f);

            if (runDust != null && runDust.isPlaying) runDust.Stop();

            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f;

        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
        anim.SetBool("IsGrounded", isGrounded);

        if (runDust != null)
        {
            if (isGrounded && isMoving)
            {
                if (!runDust.isPlaying) runDust.Play();
            }
            else
            {
                if (runDust.isPlaying) runDust.Stop();
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }

        Vector3 currentScale = transform.localScale;
        if (horizontalInput > 0)
        {
            currentScale.x = Mathf.Abs(currentScale.x);
        }
        else if (horizontalInput < 0)
        {
            currentScale.x = -Mathf.Abs(currentScale.x);
        }
        transform.localScale = currentScale;
    }

    void FixedUpdate()
    {
        if (playerShooting != null && playerShooting.IsFiringNow)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}