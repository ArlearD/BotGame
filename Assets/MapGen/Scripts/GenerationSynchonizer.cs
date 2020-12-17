using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerationSynchonizer : MonoBehaviour
{
    [SerializeField] public int Seed; //-569856270, -174520288
    [SerializeField] public Biome[] Biomes;
    [SerializeField] public GameObject[] Objects;
    [SerializeField] public GameObject Event;
    public Dictionary<Vector3, Biome> BiomeDictionary;

    public void Init(GameObject[] prefabObjects, Biome[] biomes, int mapWidthInTiles, int mapDepthInTiles, GameObject inputEvent)
    {
        Event = inputEvent;
        Objects = prefabObjects;
        if (SceneData.Seed == 0)
        {
            Seed = Random.Range(Int32.MinValue, Int32.MaxValue);
        }
        else
        {
            Seed = SceneData.Seed;
        }

        Random.InitState(Seed);

        if (biomes == null || biomes.Length == 0)
        {
            var biomesCount = Random.Range(4, 10);
            var frequency = 1f / biomesCount;
            Biomes = new Biome[biomesCount];

            for (int i = 0; i < biomesCount; i++)
            {
                var biome = new Biome();
                biome.Generate((i + 1) * frequency, new[] {prefabObjects[i % prefabObjects.Length]});
                Biomes[i] = biome;
            }
        }
        else
        {
            Biomes = biomes;
        }
        
        BiomeDictionary = new Dictionary<Vector3, Biome>();

        for (int x = -100; x < 100; x++)
        {
            for (int z = -100; z < 100; z++)
            {
                var fixedXTileIndex = (x + GetRandomNumber()) * 1.5f / mapWidthInTiles;
                var fixedZTileIndex = (z + GetRandomNumber()) * 1.5f / mapDepthInTiles;    

                var perlinNoiseValue = Mathf.PerlinNoise(fixedXTileIndex, fixedZTileIndex);
                BiomeDictionary[new Vector3(x * 10, z * 10)] = GetBiomeByPerlinNoise(perlinNoiseValue);
            }
        }
    }

    public TerrainType ChooseTerrainType(float height, Biome biome)
    {
        var orderedTerrainTypes = biome
            .terrainTypes
            .OrderBy(x => x.Height)
            .ToList();

        foreach (TerrainType terrainType in orderedTerrainTypes)
        {
            if (height < terrainType.Height)
            {
                return terrainType;
            }
        }

        return orderedTerrainTypes.Last();
    }

    public float GetRandomNumber() =>
        Random.Range(0, 1f);

    public int GetRandomIntNumber(int start, int finish) =>
        Random.Range(start, finish);

    public Vector3 GetRandomPositionForTilePosition(Vector3 tilePosition, float yPos, float xPos, float zPos) =>
        new Vector3(tilePosition.x + xPos, yPos + 0.5f, tilePosition.z + zPos);

    public Biome GetBiomeByPerlinNoise(float value)
    {
        var orderedByFrequencyBiomes = Biomes
            .OrderBy(x => x.Frequency)
            .ToList();

        foreach (Biome biome in orderedByFrequencyBiomes)
        {
            if (value < biome.Frequency)
            {
                return biome;
            }
        }

        return orderedByFrequencyBiomes.Last();
    }
}