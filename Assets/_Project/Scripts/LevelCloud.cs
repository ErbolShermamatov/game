using UnityEngine;

public class LevelCloud : MonoBehaviour
{
    public float endPositionX = 30f; 
    private float speed;

    public void Initialize(float randomSpeed)
    {
        speed = randomSpeed;
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x > endPositionX)
        {
            Destroy(gameObject);
        }
    }
}