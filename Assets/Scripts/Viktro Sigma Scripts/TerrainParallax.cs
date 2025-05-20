using UnityEngine;
using System.Collections.Generic;

public class TerrainParallaxManager : MonoBehaviour
{
    [Header("Terrain")]
    [SerializeField] Terrain terrain;
    [SerializeField] public float scrollSpeed = 10f;
    [SerializeField] public float noiseScale = 20f;
    [SerializeField] public float heightMultiplier = 5f;

    [Header("Prefab Spawning")]
    public GameObject[] spawnPrefabs;
    [Tooltip("Spawn chance for each prefab, values between 0 and 1")]
    [Range(0f, 2f)]
    public float[] spawnChances;  

    [SerializeField] public float spawnInterval = 0.3f;
    [SerializeField] public float objectYOffset = 1f;
    [SerializeField] public float despawnTime = 10f;
    [SerializeField] public float circleRadius = 30f;
    [SerializeField] public float PrefabSpeed = 2f;

    [SerializeField] float noiseOffsetZ = 0f;
    [SerializeField] float spawnTimer = 0f;
    [SerializeField] TerrainData terrainData;
    [SerializeField] List<SpawnedObject> activeObjects = new List<SpawnedObject>();

    class SpawnedObject
    {
        public GameObject instance;
        public float spawnTime;
        public float xNorm;
        public float zNorm;

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

    
        if (spawnChances == null || spawnChances.Length != spawnPrefabs.Length)
        {
            spawnChances = new float[spawnPrefabs.Length];
            for (int i = 0; i < spawnChances.Length; i++)
                spawnChances[i] = 0.3f;
        }
    }

    void Update()
    {
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

        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;

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

        // Now select prefab based on individual spawn chances
        List<int> candidates = new List<int>();
        for (int i = 0; i < spawnPrefabs.Length; i++)
        {
            if (Random.value <= spawnChances[i])
                candidates.Add(i);
        }

        if (candidates.Count == 0) return;  // no prefab selected this frame

        int selectedIndex = candidates[Random.Range(0, candidates.Count)];

        float height = GetNoiseHeight(xNorm, zNorm + noiseOffsetZ / terrainSizeZ);

        Vector3 worldPos = new Vector3(
            xNorm * terrainSizeX,
            height + objectYOffset,
            zNorm * terrainSizeZ
        ) + terrain.transform.position;

        GameObject prefab = spawnPrefabs[selectedIndex];
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

        float normalizedSpeedX = PrefabSpeed;

        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            var obj = activeObjects[i];

            // Check if the instance was destroyed externally or is null
            if (obj.instance == null)
            {
                activeObjects.RemoveAt(i);
                continue;
            }

            obj.xNorm -= normalizedSpeedX * Time.deltaTime;

            Vector2 pos = new Vector2(obj.xNorm, obj.zNorm);
            if (Time.time - obj.spawnTime > despawnTime || Vector2.Distance(pos, new Vector2(centerX, centerZ)) > radiusNormalized)
            {
                Destroy(obj.instance);
                activeObjects.RemoveAt(i);
                continue;
            }

            float noiseZ = obj.zNorm + noiseOffsetZ / terrainSizeZ;
            float height = GetNoiseHeight(obj.xNorm, noiseZ);

            Vector3 worldPos = new Vector3(
                obj.xNorm * terrainSizeX,
                height + objectYOffset,
                obj.zNorm * terrainSizeZ
            ) + terrain.transform.position;

            obj.instance.transform.position = Vector3.Lerp(
                obj.instance.transform.position,
                worldPos,
                Time.deltaTime * 10f
            );
        }
    }

    float GetNoiseHeight(float xNorm, float zNorm)
    {
        float nx = xNorm * noiseScale;
        float nz = zNorm * noiseScale;
        float h = Mathf.PerlinNoise(nx, nz);
        return h * heightMultiplier / terrainData.size.y;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (activeObjects != null)
        {
            foreach (var obj in activeObjects)
            {
                if (obj.instance != null)
                    Gizmos.DrawSphere(obj.instance.transform.position, 0.5f);
            }
        }
    }
}
