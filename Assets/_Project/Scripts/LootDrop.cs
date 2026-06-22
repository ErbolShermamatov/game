using UnityEngine;

public class LootDrop : MonoBehaviour
{
    public float dropForce = 5f;
    
    private Rigidbody2D rb;
    private Collider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (rb != null)
        {
            Vector2 dropDirection = new Vector2(Random.Range(-0.5f, 0.5f), 1f).normalized;
            rb.AddForce(dropDirection * dropForce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-15f, 15f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            gameObject.tag = "CollectibleArrow";
            
            gameObject.layer = LayerMask.NameToLayer("Default");
            
            if (col != null) 
            {
                col.isTrigger = true;
            }
            
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }
}