using System;
using UnityEngine;

public class Generation : MonoBehaviour
{
    private MeshRenderer _tileRenderer;

    private MeshFilter _meshFilter;

    private MeshCollider _meshCollider;

    private GenerationSynchonizer _generationSynchonizer;

    public Biome Biome;

    private float _prefabNotProbability;
    private float _eventNotProbability;

    public void Init(GenerationSynchonizer generationSynchonizer)
    {
        _generationSynchonizer = generationSynchonizer;
        _prefabNotProbability = 0.99f;
        _eventNotProbability = 0.99f;

        CreateTile(null);
    }

    public void Init(GenerationSynchonizer generationSynchonizer, float? yPos, Biome arenaBiome)
    {
        _generationSynchonizer = generationSynchonizer;
        _prefabNotProbability = 1;
        _eventNotProbability = 1;
        Biome = arenaBiome;

        CreateTile(yPos);
    }

    private void CreateTile(float? yPos)
    {
        if (yPos == null)
        {
            AssignBiome();
        }
        
        _tileRenderer = gameObject.GetComponent<MeshRenderer>();
        _meshFilter = gameObject.GetComponent<MeshFilter>();
        _meshCollider = gameObject.GetComponent<MeshCollider>();

        GenerateTile(yPos);
    }

    void AssignBiome()
    {
        var position = transform.position;
        Biome = _generationSynchonizer.BiomeDictionary[new Vector3(position.x, position.z)];
    }

    void GenerateTile(float? yPos)
    {
        var meshVertices = _meshFilter.mesh.vertices;
        var tileDepth = (int) Mathf.Sqrt(meshVertices.Length);
        var tileWidth = tileDepth;

        var position = gameObject.transform.position;
        var offsetX = -position.x;
        var offsetZ = -position.z;

        var heightMap = GenerateNoiseMap(tileDepth, tileWidth, Biome.mapScale, offsetX, offsetZ, Biome.waves);

        var tileTexture = BuildTexture(heightMap, Biome);
        _tileRenderer.material.mainTexture = tileTexture;
        UpdateMeshVertices(heightMap, Biome, yPos);
    }

    private Texture2D BuildTexture(float[,] heightMap, Biome biome)
    {
        var tileDepth = heightMap.GetLength(0);
        var tileWidth = heightMap.GetLength(1);

        var colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                var colorIndex = zIndex * tileWidth + xIndex;
                var height = heightMap[zIndex, xIndex];
                var terrainType = _generationSynchonizer.ChooseTerrainType(height, biome);
                colorMap[colorIndex] = terrainType.Color;
            }
        }

        var tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

    public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ, Wave[] waves)
    {
        var noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                float sampleX = (xIndex + offsetX) / scale;
                float sampleZ = (zIndex + offsetZ) / scale;

                float noise = 0f;
                float normalization = 0f;
                foreach (Wave wave in waves)
                {
                    noise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed,
                        sampleZ * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }

                noise /= normalization;

                noiseMap[zIndex, xIndex] = noise;
            }
        }

        return noiseMap;
    }

    private void UpdateMeshVertices(float[,] heightMap, Biome biome, float? tileHeight)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = _meshFilter.mesh.vertices;

        float minYPos = 500;
        float maxYPos = -500;

        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                var height = heightMap[zIndex, xIndex];
                float yPos;
                if (tileHeight != null)
                {
                    yPos = tileHeight.Value;
                }
                else
                {
                    yPos = biome.heightCurve.Evaluate(height) * biome.heightMultiplier;
                }

                if (yPos < minYPos)
                {
                    minYPos = yPos;
                }

                if (yPos > maxYPos)
                {
                    maxYPos = yPos;
                }

                var vertex = meshVertices[vertexIndex];
                meshVertices[vertexIndex] = new Vector3(vertex.x, yPos, vertex.z);

                if (tileHeight == null)
                    GeneratePrefabsAndEvents(xIndex, zIndex, tileDepth, tileWidth, maxYPos, minYPos, yPos, vertex);

                vertexIndex++;
            }
        }

        _meshFilter.mesh.vertices = meshVertices;
        _meshFilter.mesh.RecalculateBounds();
        _meshFilter.mesh.RecalculateNormals();
        _meshCollider.sharedMesh = _meshFilter.mesh;
    }

    private void GeneratePrefabsAndEvents(int xIndex, int zIndex, int tileDepth, int tileWidth, float maxYPos,
        float minYPos,
        float yPos, Vector3 vertex)
    {
        if (xIndex != 0 && zIndex != 0 && xIndex != tileDepth - 1 && zIndex != tileWidth - 1)
        {
            var number = _generationSynchonizer.GetRandomNumber();

            if (number > _prefabNotProbability)
            {
                var difference = maxYPos - minYPos;

                if (yPos < minYPos + difference / 100 * 20)
                {
                    var subject = Instantiate(Biome.GetRandomPrefab(),
                        _generationSynchonizer.GetRandomPositionForTilePosition(transform.position, yPos,
                            vertex.x, vertex.z),
                        Quaternion.Euler(UnityEngine.Random.Range(-45, 45), UnityEngine.Random.Range(-45, 45), UnityEngine.Random.Range(-45, 45)));

                    subject.transform.parent = transform;
                }
            }

            number = _generationSynchonizer.GetRandomNumber();
            if (number > _eventNotProbability)
            {
                var subject = Instantiate(_generationSynchonizer.Event,
                    _generationSynchonizer.GetRandomPositionForTilePosition(transform.position, yPos,
                        vertex.x, vertex.z),
                    Quaternion.identity);

                subject.transform.parent = transform;
            }
        }
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