using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public GameObject arrowLootPrefab;
    public float knockbackForceX = 5f;
    public float knockbackForceY = 2f;

    private int currentHealth;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Color originalColor;

    private EnemyAI enemyAI;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        currentHealth -= damage;

        ApplyKnockback(damageSourcePosition);

        StopAllCoroutines();
        StartCoroutine(FlashRedSmooth());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ApplyKnockback(Vector2 sourcePos)
    {
        if (rb == null || rb.bodyType != RigidbodyType2D.Dynamic) return; 

        if (enemyAI != null)
        {
            enemyAI.knockbackTimer = 0.3f;
        }
        float knockbackDirection = transform.position.x < sourcePos.x ? -1f : 1f;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(knockbackForceX * knockbackDirection, knockbackForceY), ForceMode2D.Impulse);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        StopAllCoroutines();
        StartCoroutine(FlashRedSmooth());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRedSmooth()
    {
        float duration = 0.2f;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(originalColor, Color.red, timer / duration);
            yield return null;
        }

        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(Color.red, originalColor, timer / duration);
            yield return null;
        }

        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        if (enemyAI != null) enemyAI.enabled = false;

        EnemyArcherAI archerAI = GetComponent<EnemyArcherAI>();
        if (archerAI != null) archerAI.SetDead();

        if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null) coll.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
            anim.enabled = false;
        }

        StartCoroutine(FadeOutDeath());
    }

    private IEnumerator FadeOutDeath()
    {
        float timer = 0f;
        float duration = 1.0f;
        Color startColor = spriteRenderer.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            Color newColor = startColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = newColor;

            yield return null;
        }

        if (arrowLootPrefab != null)
        {
            Instantiate(arrowLootPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}