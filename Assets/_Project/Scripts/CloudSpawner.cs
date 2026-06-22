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
        SetNextSpawnTime();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnCloud();
            SetNextSpawnTime();
        }
    }

    private void SpawnCloud()
    {
        GameObject newCloud = Instantiate(cloudPrefab, cloudContainer);
        RectTransform rect = newCloud.GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(startX, Random.Range(minY, maxY));

        if (cloudSprites.Length > 0)
        {
            Image img = newCloud.GetComponent<Image>();
            img.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
            img.SetNativeSize();
        }

        Cloud cloudScript = newCloud.GetComponent<Cloud>();
        if (cloudScript != null)
        {
            cloudScript.Initialize(Random.Range(minSpeed, maxSpeed));
        }
        
        newCloud.transform.SetAsFirstSibling(); 
    }

    private void SetNextSpawnTime()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }
}