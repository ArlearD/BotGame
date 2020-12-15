using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerationSynchonizer : MonoBehaviour
{
    [SerializeField] public int Seed;
    [SerializeField] public Biome[] Biomes;
    [SerializeField] public GameObject[] Objects;
    void Start()
    {
        if (Seed == 0)
        {
            Seed = Random.Range(Int32.MinValue, Int32.MaxValue);
        }
        Random.InitState(Seed);
        
        if (Biomes.Length == 0)
        {
            var biomesCount = Random.Range(1, 1);
            
            Biomes = new Biome[biomesCount];

            for (int i = 0; i < biomesCount; i++)
            {
                var biome = new Biome();
                biome.Generate();
                Biomes[i] = biome;
            }
        }
    }

    public TerrainType ChooseTerrainType(float height, Biome biome)
    {
        // for each terrain type, check if the height is lower than the one for the terrain type
        foreach (TerrainType terrainType in biome.terrainTypes)
        {
            // return the first terrain type whose height is higher than the generated one
            if (height < terrainType.Height)
            {
                return terrainType;
            }
        }

        return biome.terrainTypes[biome.terrainTypes.Length - 1];
    }

    public Biome GetRandomBiome()
    {
        return Biomes[Random.Range(0, Biomes.Length)];
    }

    public GameObject GetRandomPrefab()
    {
        return Objects[Random.Range(0, Objects.Length)];
    }

    public float GetRandomNumber()
    {
        return Random.Range(0, 1f);
    }

    public Vector3 GetRandomPositionForTilePosition(Vector3 tilePosition, float yPos, float xPos, float zPos)
    {
        return new Vector3(tilePosition.x + xPos, yPos + 0.5f, tilePosition.z + zPos);
    } 
        
    
}
