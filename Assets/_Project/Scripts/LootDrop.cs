using UnityEngine;

public class LootDrop : MonoBehaviour
{
    public float dropForce = 5f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dropDirection = new Vector2(Random.Range(-0.5f, 0.5f), 1f).normalized;
            
            rb.AddForce(dropDirection * dropForce, ForceMode2D.Impulse);
            
            rb.AddTorque(Random.Range(-15f, 15f));
        }
    }
}