using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private PlayerMovement movementScript;
    private bool isDead = false;

    public float knockbackForceX = 8f;
    public float knockbackForceY = 0f;
    public float invulnerabilityTime = 1f;
    private bool isInvulnerable = false;

    public Image healthBarFill;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRend;
    private Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        movementScript = GetComponent<PlayerMovement>();

        UpdateHealthBar();
    }

    public void TakeDamage(int damage, Transform enemyTransform)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            ApplyKnockback(enemyTransform);
            StartCoroutine(DamageEffect());
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void ApplyKnockback(Transform enemyTransform)
    {
        rb.velocity = Vector2.zero;

        if (movementScript != null) movementScript.knockbackTimer = 0.3f;

        float knockbackDirection = transform.position.x < enemyTransform.position.x ? -1f : 1f;
        rb.AddForce(new Vector2(knockbackForceX * knockbackDirection, knockbackForceY), ForceMode2D.Impulse);
    }

    IEnumerator DamageEffect()
    {
        isInvulnerable = true;

        int blinkCount = 3;
        float blinkDuration = invulnerabilityTime / (blinkCount * 2);

        for (int i = 0; i < blinkCount; i++)
        {
            spriteRend.color = new Color(1f, 0.3f, 0.3f, 1f);
            yield return new WaitForSeconds(blinkDuration);

            spriteRend.color = Color.white;
            yield return new WaitForSeconds(blinkDuration);
        }

        spriteRend.color = Color.white;
        isInvulnerable = false;
    }

    void Die()
    {
        isDead = true;

        currentHealth = 0;
        UpdateHealthBar();

        rb.velocity = Vector2.zero;
        rb.simulated = false;

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerShooting>().enabled = false;

        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
            anim.SetBool("IsGrounded", true);
        }

        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        float fadeDuration = 2f;
        float timer = 0f;

        Color startColor = spriteRend.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            spriteRend.color = new Color(startColor.r, startColor.g, startColor.b, currentAlpha);
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathZone") && !isDead)
        {
            Die();
        }
    }
}