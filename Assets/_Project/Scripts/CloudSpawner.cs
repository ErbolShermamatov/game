using UnityEngine;
using UnityEngine.UI;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public Sprite[] cloudSprites;
    public Transform cloudContainer;

    public float startX = -1200f;
    public float minY = 100f;
    public float maxY = 450f;

    public float minSpeed = 20f;
    public float maxSpeed = 60f;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 4f;

    private float timer;

    void Start()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnCloud();
            timer = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }

    void SpawnCloud()
    {
        GameObject newCloud = Instantiate(cloudPrefab, cloudContainer);
        RectTransform rect = newCloud.GetComponent<RectTransform>();

        float randomY = Random.Range(minY, maxY);
        rect.anchoredPosition = new Vector2(startX, randomY);

        if (cloudSprites.Length > 0)
        {
            Image img = newCloud.GetComponent<Image>();
            img.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
            img.SetNativeSize();
        }

        Cloud cloudScript = newCloud.GetComponent<Cloud>();
        cloudScript.Initialize(Random.Range(minSpeed, maxSpeed));
        
        newCloud.transform.SetAsFirstSibling(); 
    }
}