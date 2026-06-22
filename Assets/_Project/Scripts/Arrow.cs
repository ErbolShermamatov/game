using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifetime = 5f;
    public bool isEnemyArrow = false;
    public float embedDepth = 0.4f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    private bool isLaunched = false;
    private bool isStuck = false;
    private Coroutine deathTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void LaunchBallistic(Vector2 velocity)
    {
        rb.velocity = velocity;
        isLaunched = true;
        deathTimer = StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
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

        if (collision.CompareTag("Player"))
        {
            if (!isEnemyArrow) return;

            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1, transform);
                Destroy(gameObject);
            }
            return;
        }

        if (!isEnemyArrow)
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(1, transform.position);
                Destroy(gameObject);
            }
        }
    }

    private void StickIntoGround(Transform groundTransform)
    {
        isStuck = true;
        isLaunched = false;

        if (deathTimer != null) StopCoroutine(deathTimer);

        transform.position += transform.right * embedDepth;
        spriteRenderer.sortingOrder = -3;

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; 

        transform.SetParent(groundTransform);
        gameObject.tag = "CollectibleArrow";
        col.isTrigger = true; 

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCol = player.GetComponent<Collider2D>();
            if (playerCol != null) Physics2D.IgnoreCollision(playerCol, col, false);
        }
    }
}