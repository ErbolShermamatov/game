using System.Collections;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    private int unitsOnTower = 0;
    private bool isReady = false;
    private bool isFading = false;

    void Start()
    {
        Invoke("SetReady", 0.5f);
    }

    void SetReady()
    {
        isReady = true; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            unitsOnTower++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            unitsOnTower--;
        }
    }

    void Update()
    {
        if (isReady && unitsOnTower <= 0 && !isFading)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        isFading = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
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
        }

        Destroy(gameObject);
    }
}