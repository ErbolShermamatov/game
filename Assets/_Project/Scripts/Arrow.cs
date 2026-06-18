using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifetime = 5f;
    private Rigidbody2D rb;
    private bool isLaunched = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void LaunchBallistic(Vector2 velocity)
    {
        rb.velocity = velocity;
        isLaunched = true;
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (isLaunched && rb.velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(1, transform.position);
            Destroy(gameObject);
        }
    }
}
