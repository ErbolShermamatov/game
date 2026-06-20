using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float endPositionX = 1200f;
    private float speed;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(float randomSpeed)
    {
        speed = randomSpeed;
    }

    void Update()
    {
        // Летим направо
        rectTransform.anchoredPosition += Vector2.right * speed * Time.deltaTime;

        if (rectTransform.anchoredPosition.x > endPositionX)
        {
            Destroy(gameObject);
        }
    }
}