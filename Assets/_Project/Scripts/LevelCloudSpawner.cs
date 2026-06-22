using UnityEngine;

public class LevelCloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab; 
    public Sprite[] cloudSprites;

    public float startX = -20f;
    public float minY = 3f;
    public float maxY = 8f;
    
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float minSpawnTime = 2f;
    public float maxSpawnTime = 6f;

    public string sortingLayerName = "Default";
    public int sortingOrder = -10;

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
        Vector2 spawnPos = new Vector2(startX, Random.Range(minY, maxY));
        GameObject newCloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);

        if (cloudSprites.Length > 0)
        {
            SpriteRenderer sr = newCloud.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
                sr.sortingLayerName = sortingLayerName;
                sr.sortingOrder = sortingOrder;
            }
        }

        LevelCloud cloudScript = newCloud.GetComponent<LevelCloud>();
        if (cloudScript != null)
        {
            cloudScript.Initialize(Random.Range(minSpeed, maxSpeed));
        }
    }

    private void SetNextSpawnTime()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }
}