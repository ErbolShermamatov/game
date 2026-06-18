using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public float knockbackForce = 5f;
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
        if (rb == null) return;

        if (enemyAI != null)
        {
            enemyAI.knockbackTimer = 0.3f;
        }

        Vector2 direction = (Vector2)transform.position - sourcePos;
        direction = direction.normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(direction.x, 0.5f) * knockbackForce, ForceMode2D.Impulse);
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

        if (rb != null) 
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null) coll.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetFloat("Speed", 0f);
        
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

        Destroy(gameObject);
    }
}