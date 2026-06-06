using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 0.2f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.5f;
    public LayerMask groundLayer;
    public bool onGround;

    private Rigidbody2D rb;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        Jump();
        CheckGround();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void CheckGround()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}