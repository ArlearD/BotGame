using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Biome
{
    [SerializeField] public float mapScale;
    [SerializeField] public float heightMultiplier;
    [SerializeField] public TerrainType[] terrainTypes;
    [SerializeField] public Wave[] waves;
    [SerializeField] public AnimationCurve heightCurve;

    public void Generate()
    {
        GenerateWaves();
        GenerateHeightMultiplier();
        GenerateMapScale();
        GenerateTerrainTypes();
        GenerateAnimationCurve();
    }

    public void GenerateAnimationCurve()
    {
        heightCurve = new AnimationCurve();
        for (int i = 0; i < terrainTypes.Length; i++)
        {
            heightCurve.AddKey(terrainTypes[i].Height, Random.value);
        }
    }

    public void GenerateTerrainTypes()
    {
        if (terrainTypes == null || terrainTypes.Length == 0)
        {
            terrainTypes = new TerrainType[Random.Range(1, 15)];

            for (int i = 0; i < terrainTypes.Length; i++)
            {
                terrainTypes[i] = new TerrainType
                {
                    Name = i.ToString(),
                    Color = Color.HSVToRGB(Random.value, Random.value, Random.value),
                    Height = Random.value
                };
            }

            terrainTypes = terrainTypes
                .OrderBy(x => x.Height)
                .ToArray();
        }
    }

    public void GenerateHeightMultiplier()
    {
        if (heightMultiplier == 0)
        {
            heightMultiplier = 2;
        }
    }

    public void GenerateMapScale()
    {
        if (mapScale == 0)
        {
            mapScale = Random.Range(0f, 3f);
        }
    }

    public void GenerateWaves()
    {
        if (waves == null || waves.Length == 0)
        {
            waves = new Wave[Random.Range(1, 5)];
            for (int i = 0; i < waves.Length; i++)
            {
                waves[i] = new Wave
                {
                    seed = Random.Range(1, 1000000),
                    amplitude = Random.Range(0, 1f),
                    frequency = Random.Range(0, 0.5f)
                };
            }
        }
    }
}