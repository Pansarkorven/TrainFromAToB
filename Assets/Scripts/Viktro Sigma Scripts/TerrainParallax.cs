using UnityEngine;
using System.Collections.Generic;

public class TerrainParallaxManager : MonoBehaviour
{
    [Header("Terrain")]
    public Terrain terrain;
    public float scrollSpeed = 10f;        // Speed of noise scroll vertically (Z)
    public float noiseScale = 20f;
    public float heightMultiplier = 5f;

    [Header("Prefab Spawning")]
    public GameObject[] spawnPrefabs;
    [Range(0f, 1f)]
    public float spawnChance = 0.3f;
    public float spawnInterval = 0.3f;
    public float objectYOffset = 1f;
    public float despawnTime = 10f;
    public float circleRadius = 30f;

    private float noiseOffsetZ = 0f;
    private float spawnTimer = 0f;
    private TerrainData terrainData;

    private List<SpawnedObject> activeObjects = new List<SpawnedObject>();

    class SpawnedObject
    {
        public GameObject instance;
        public float spawnTime;
        public float xNorm;    // normalized X (0-1)
        public float zNorm;    // normalized Z (0-1)

        public SpawnedObject(GameObject obj, float x, float z)
        {
            instance = obj;
            spawnTime = Time.time;
            xNorm = x;
            zNorm = z;
        }
    }

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain not assigned.");
            enabled = false;
            return;
        }
        terrainData = terrain.terrainData;
    }

    void Update()
    {
        // Advance noise offset vertically (Z)
        noiseOffsetZ += scrollSpeed * Time.deltaTime;

        UpdateTerrain();

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;
            TrySpawnPrefab();
        }

        UpdateObjects();
    }

    void UpdateTerrain()
    {
        int res = terrainData.heightmapResolution;
        float[,] heights = new float[res, res];

        for (int x = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++)
            {
                float nx = (float)x / res * noiseScale;
                float nz = (float)z / res * noiseScale + noiseOffsetZ;
                float h = Mathf.PerlinNoise(nx, nz);
                heights[x, z] = h * heightMultiplier / terrainData.size.y;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    void TrySpawnPrefab()
    {
        if (spawnPrefabs.Length == 0) return;
        if (Random.value > spawnChance) return;

        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;

        // Center of circle normalized coords
        float centerX = 0.5f;
        float centerZ = 0.5f;

        float radiusNormalized = circleRadius / terrainSizeX;

        Vector2 spawnPos;
        int attempts = 0;
        do
        {
            spawnPos = new Vector2(
                Random.Range(centerX - radiusNormalized, centerX + radiusNormalized),
                Random.Range(centerZ - radiusNormalized, centerZ + radiusNormalized)
            );
            attempts++;
            if (attempts > 10) break;
        }
        while (Vector2.Distance(spawnPos, new Vector2(centerX, centerZ)) > radiusNormalized);

        float xNorm = Mathf.Clamp01(spawnPos.x);
        float zNorm = Mathf.Clamp01(spawnPos.y);

        float height = GetNoiseHeight(xNorm, zNorm + noiseOffsetZ / terrainSizeZ);

        Vector3 worldPos = new Vector3(
            xNorm * terrainSizeX,
            height + objectYOffset,
            zNorm * terrainSizeZ
        ) + terrain.transform.position;

        GameObject prefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
        GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);

        activeObjects.Add(new SpawnedObject(instance, xNorm, zNorm));
    }

    void UpdateObjects()
    {
        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;

        float centerX = 0.5f;
        float centerZ = 0.5f;
        float radiusNormalized = circleRadius / terrainSizeX;

        // Move prefabs **horizontally** leftward by scrollSpeed (normalized)
        float normalizedSpeedX = scrollSpeed / terrainSizeX;

        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            var obj = activeObjects[i];

            // Move prefab leftward along X axis (normalized)
            obj.xNorm -= normalizedSpeedX * Time.deltaTime;

            // Despawn if outside circle radius or time expired
            Vector2 pos = new Vector2(obj.xNorm, obj.zNorm);
            if (Time.time - obj.spawnTime > despawnTime || Vector2.Distance(pos, new Vector2(centerX, centerZ)) > radiusNormalized)
            {
                Destroy(obj.instance);
                activeObjects.RemoveAt(i);
                continue;
            }

            // Calculate height at current noise position (adjust Z with noiseOffsetZ)
            float noiseZ = obj.zNorm + noiseOffsetZ / terrainSizeZ;
            float height = GetNoiseHeight(obj.xNorm, noiseZ);

            Vector3 worldPos = new Vector3(
                obj.xNorm * terrainSizeX,
                height + objectYOffset,
                obj.zNorm * terrainSizeZ
            ) + terrain.transform.position;

            obj.instance.transform.position = worldPos;
        }
    }

    float GetNoiseHeight(float xNorm, float zNorm)
    {
        float nx = xNorm * noiseScale;
        float nz = zNorm * noiseScale;
        float h = Mathf.PerlinNoise(nx, nz);
        return h * heightMultiplier;
    }
}
