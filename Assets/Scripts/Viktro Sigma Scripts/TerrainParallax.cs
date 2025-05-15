using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain; // Assign your terrain
    public float speed = 0.5f; // Controls how fast the terrain moves
    public float circleRadius = 20f; // The radius of the affected area
    public int terrainResolution = 129;
    public float noiseScale = 20f;
    public float heightMultiplier = 5f;

    private float noiseOffsetZ = 0f; // Tracks the movement forward

    void Start()
    {
        GenerateTerrain(0f);
    }

    void Update()
    {
        // Move the noise offset forward to simulate movement
        noiseOffsetZ += speed * Time.deltaTime;

        GenerateTerrain(noiseOffsetZ);
    }

    void GenerateTerrain(float offsetZ)
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        int centerX = width / 2;
        int centerZ = height / 2;
        int radiusInPoints = Mathf.RoundToInt((circleRadius / terrainData.size.x) * width); // Convert world radius to heightmap points

        // Loop through only the circular area
        for (int x = -radiusInPoints; x <= radiusInPoints; x++)
        {
            for (int z = -radiusInPoints; z <= radiusInPoints; z++)
            {
                int worldX = centerX + x;
                int worldZ = centerZ + z;

                // Ensure within terrain bounds
                if (worldX >= 0 && worldX < width && worldZ >= 0 && worldZ < height)
                {
                    // Check if within the circular area
                    float distance = Mathf.Sqrt(x * x + z * z);
                    if (distance <= radiusInPoints)
                    {
                        // Generate Perlin noise for movement, only inside the circular area
                        float worldPosX = (float)worldX / width * noiseScale;
                        float worldPosZ = (float)worldZ / height * noiseScale + offsetZ;
                        heights[worldX, worldZ] = Mathf.PerlinNoise(worldPosX, worldPosZ) * heightMultiplier / terrainData.size.y;
                    }
                }
            }
        }

        // Apply the updated heights
        terrainData.SetHeights(0, 0, heights);
    }
}