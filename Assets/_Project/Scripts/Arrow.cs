using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifetime = 5f;
    private Rigidbody2D rb;
    private bool isLaunched = false;
    public bool isEnemyArrow = false;
    private bool isStuck = false;
    public float embedDepth = 0.4f;

    private Coroutine deathTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void LaunchBallistic(Vector2 velocity)
    {
        rb.velocity = velocity;
        isLaunched = true;

        deathTimer = StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (isLaunched && !isStuck && rb.velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStuck) return;

        if (collision.CompareTag("Ground"))
        {
            StickIntoGround(collision.transform);
            return;
        }

        if (isEnemyArrow)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1, transform);
                    Destroy(gameObject);
                }
            }
        }
        else
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

    void StickIntoGround(Transform groundTransform)
    {
        isStuck = true;
        isLaunched = false;

        if (deathTimer != null)
        {
            StopCoroutine(deathTimer);
        }

        transform.position += transform.right * embedDepth;

        GetComponent<SpriteRenderer>().sortingOrder = -3;

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; 

        transform.SetParent(groundTransform);
        gameObject.tag = "CollectibleArrow";
        GetComponent<Collider2D>().isTrigger = true; 

        Collider2D playerCol = FindObjectOfType<PlayerShooting>().GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(playerCol, GetComponent<Collider2D>(), false);
    }
}