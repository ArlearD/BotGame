using System;
using UnityEngine;

public class Generation : MonoBehaviour
{
    private MeshRenderer tileRenderer;

    private MeshFilter meshFilter;

    private MeshCollider meshCollider;

    private GenerationSynchonizer _generationSynchonizer;

    private Biome Biome;

    // Start is called before the first frame update
    void Start()
    {
        _generationSynchonizer = GameObject
            .Find("TerrainTypesBuilder")
            .GetComponent<GenerationSynchonizer>();

        tileRenderer = gameObject.GetComponent<MeshRenderer>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshCollider = gameObject.GetComponent<MeshCollider>();

        GenerateTile();
    }
    
    

    void GenerateTile()
    {
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        int tileDepth = (int) Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        var biome = _generationSynchonizer.GetRandomBiome();

        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        // calculate the offsets based on the tile position
        float[,] heightMap = GenerateNoiseMap(tileDepth, tileWidth, biome.mapScale, offsetX, offsetZ, biome.waves);
        
        Texture2D tileTexture = BuildTexture(heightMap, biome);
        tileRenderer.material.mainTexture = tileTexture;

        UpdateMeshVertices(heightMap, biome);
    }

    private Texture2D BuildTexture(float[,] heightMap, Biome biome)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];
                TerrainType terrainType = _generationSynchonizer.ChooseTerrainType(height,
                    biome);
                colorMap[colorIndex] = terrainType.Color;
            }
        }

        // create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

    public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ,
        Wave[] waves)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = (xIndex + offsetX) / scale;
                float sampleZ = (zIndex + offsetZ) / scale;

                float noise = 0f;
                float normalization = 0f;
                foreach (Wave wave in waves)
                {
                    // generate noise value using PerlinNoise for a given Wave
                    noise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed,
                        sampleZ * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }

                // normalize the noise value so that it is within 0 and 1
                noise /= normalization;
                
                noiseMap[zIndex, xIndex] = noise;
            }
        }

        return noiseMap;
    }

    private void UpdateMeshVertices(float[,] heightMap, Biome biome)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = meshFilter.mesh.vertices;

        float minYPos = 500;
        float maxYPos = -500;
        
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];
                
                var yPos = biome.heightCurve.Evaluate(height) * biome.heightMultiplier;

                if (yPos < minYPos)
                {
                    minYPos = yPos;
                }

                if (yPos > maxYPos)
                {
                    maxYPos = yPos;
                }

                Vector3 vertex = meshVertices[vertexIndex];
                meshVertices[vertexIndex] = new Vector3(vertex.x, yPos, vertex.z);

                var number = _generationSynchonizer.GetRandomNumber();

                //if (number > 0.95f)
                //{
                //    var difference = maxYPos - minYPos;

                //    if (yPos < minYPos + difference / 100 * 20)
                //    {
                //        Instantiate(_generationSynchonizer.GetRandomPrefab(), 
                //            _generationSynchonizer.GetRandomPositionForTilePosition(transform.position, yPos, vertex.x, vertex.z), 
                //            Quaternion.identity);
                //    }
                //}

                vertexIndex++;
            }
        }
        
        meshFilter.mesh.vertices = meshVertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        meshCollider.sharedMesh = meshFilter.mesh;
    }
}

[Serializable]
public class TerrainType
{
    public string Name;
    public float Height;
    public Color Color;
}

[Serializable]
public class Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}