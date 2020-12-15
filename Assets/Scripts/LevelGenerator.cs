using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public int MapWidthInTiles, MapDepthInTiles;
    private GenerationSynchonizer _generationSynchonizer;
    [SerializeField] private GameObject TerrainTypesBuilder;

    [SerializeField] private GameObject TilePrefab;

    [SerializeField] private GameObject Player;

    [SerializeField] private Vector3 MinXMinZPosition;
    [SerializeField] private Vector3 MaxXMaxZPosition;
    [SerializeField] private Vector3 MaxXMinZPosition;
    [SerializeField] private Vector3 MinXMaxZPosition;
    [SerializeField] private bool DisableGeneration;

    [SerializeField] private float AcceptableRange;

    [SerializeField] private Vector3 PlayerPosition;

    [SerializeField] private float PlayerMaxXDifference;
    [SerializeField] private float PlayerMinXDifference;
    [SerializeField] private float PlayerMaxZDifference;
    [SerializeField] private float PlayerMinZDifference;

    [SerializeField] private int TileWidth;
    [SerializeField] private int TileDepth;

    [SerializeField] private Dictionary<Vector3, GameObject> Tiles;
    [SerializeField] private Dictionary<Vector3, GameObject> TilesToFix;

    [SerializeField] private GameObject[] PrefabObjects;
    [SerializeField] private Biome[] Biomes;
    public GameObject EventObject;

    [SerializeField] private Material[] Skyboxes;

    [SerializeField] private GameObject TimeController;
    private bool _wasInited = false;

    [SerializeField] public GameObject Event;
    private int _halfOfMapSize;
    private int _startArenaTileIndex;
    private int _finishArenaTileIndex;

    [SerializeField] private GameObject navMesh;

    void Start()
    {
        if (SceneData.GenerateArena) DisableGeneration = true;
        
        MapDepthInTiles = SceneData.MapSize;
        MapWidthInTiles = SceneData.MapSize;
        _halfOfMapSize = SceneData.MapSize * 10 / 2;
        Player.transform.position = new Vector3(_halfOfMapSize, SceneData.ArenaHeight + 1, _halfOfMapSize);

        if (SceneData.ArenaSize % 2 == 0)
            _startArenaTileIndex = SceneData.MapSize / 2 - SceneData.ArenaSize / 2;
        else
            _startArenaTileIndex = SceneData.MapSize / 2 - SceneData.ArenaSize / 2 - 1;

        _finishArenaTileIndex = SceneData.MapSize / 2 + SceneData.ArenaSize / 2;

        var ruller = TimeController.GetComponent<Ruller>();
        ruller.K = Random.Range(0, 1);
        ruller.StartTime = Random.Range(20000, 30000);

        _generationSynchonizer = TerrainTypesBuilder.AddComponent<GenerationSynchonizer>();
        _generationSynchonizer.Init(PrefabObjects, Biomes, MapWidthInTiles, MapDepthInTiles, Event);

        ApplyRandomSkybox();
        GenerateMap();

        TilesToFix = Tiles;

        Invoke("FixMap", 0.001f);

        AcceptableRange = MapWidthInTiles * 10 / 2f - 20;
        PlayerPosition = Player.transform.position;

        Invoke("UpdateNavMesh", 0.2f);
    }

    private void UpdateNavMesh()
    {
        var a = navMesh.GetComponent<NavMeshSurface>();
        a.BuildNavMesh();
    }

    public void ApplyRandomSkybox()
    {
        RenderSettings.skybox = Skyboxes[_generationSynchonizer.GetRandomIntNumber(0, Skyboxes.Length)];
    }

    void FixMap()
    {
        foreach (var keyValue in TilesToFix)
        {
            var generation = keyValue.Value.GetComponent<Generation>();
            var biome = generation.Biome;

            var plusXTilePosition = new Vector3(keyValue.Key.x + 10, keyValue.Key.y, keyValue.Key.z);

            if (TilesToFix.ContainsKey(plusXTilePosition))
            {
                var plusXTile = TilesToFix[plusXTilePosition];
                var plusXTileBiome = plusXTile.GetComponent<Generation>().Biome;
                if (biome.Id != plusXTileBiome.Id)
                {
                    var currentMesh = keyValue.Value.GetComponent<MeshFilter>().mesh;
                    var otherMesh = plusXTile.GetComponent<MeshFilter>().mesh;

                    var currentMeshCollider = keyValue.Value.GetComponent<MeshCollider>();
                    var otherMeshCollider = plusXTile.GetComponent<MeshCollider>();

                    var currentMeshVertices = currentMesh.vertices;
                    var otherMeshVertices = otherMesh.vertices;

                    var otherRowIndex = 10;

                    for (int i = 0; i < 11; i++)
                    {
                        var currentVertice = currentMeshVertices[i * 11];
                        var otherVertice = otherMeshVertices[otherRowIndex + i * 11];

                        if (currentVertice.y > otherVertice.y)
                        {
                            currentMeshVertices[i * 11].y = otherVertice.y;
                        }
                        else
                        {
                            otherMeshVertices[otherRowIndex + i * 11].y = currentVertice.y;
                        }
                    }

                    currentMesh.vertices = currentMeshVertices;
                    currentMesh.RecalculateBounds();
                    currentMesh.RecalculateNormals();
                    currentMeshCollider.sharedMesh = currentMesh;

                    otherMesh.vertices = otherMeshVertices;
                    otherMesh.RecalculateBounds();
                    otherMesh.RecalculateNormals();
                    otherMeshCollider.sharedMesh = otherMesh;
                }
            }

            var plusZTilePosition = new Vector3(keyValue.Key.x, keyValue.Key.y, keyValue.Key.z + 10);
            if (TilesToFix.ContainsKey(plusZTilePosition))
            {
                var plusZTile = TilesToFix[plusZTilePosition];
                var plusZTileBiome = plusZTile.GetComponent<Generation>().Biome;

                if (biome.Id != plusZTileBiome.Id)
                {
                    var currentMesh = keyValue.Value.GetComponent<MeshFilter>().mesh;
                    var otherMesh = plusZTile.GetComponent<MeshFilter>().mesh;

                    var currentMeshCollider = keyValue.Value.GetComponent<MeshCollider>();
                    var otherMeshCollider = plusZTile.GetComponent<MeshCollider>();

                    var currentMeshVertices = currentMesh.vertices;
                    var otherMeshVertices = otherMesh.vertices;

                    var otherRowIndex = 10;

                    for (int i = 0; i < 11; i++)
                    {
                        var currentVertice = currentMeshVertices[i];
                        var otherVertice = otherMeshVertices[otherRowIndex * 11 + i];

                        if (currentVertice.y > otherVertice.y)
                        {
                            currentMeshVertices[i].y = otherVertice.y;
                        }
                        else
                        {
                            otherMeshVertices[otherRowIndex * 11 + i].y = currentVertice.y;
                        }
                    }

                    currentMesh.vertices = currentMeshVertices;
                    currentMesh.RecalculateBounds();
                    currentMesh.RecalculateNormals();
                    currentMeshCollider.sharedMesh = currentMesh;

                    otherMesh.vertices = otherMeshVertices;
                    otherMesh.RecalculateBounds();
                    otherMesh.RecalculateNormals();
                    otherMeshCollider.sharedMesh = otherMesh;
                }
            }

            var plusXPlusZTilePosition = new Vector3(keyValue.Key.x + 10, keyValue.Key.y, keyValue.Key.z + 10);

            if (TilesToFix.ContainsKey(plusXTilePosition) && TilesToFix.ContainsKey(plusZTilePosition) &&
                TilesToFix.ContainsKey(plusXPlusZTilePosition))
            {
                var plusXTile = TilesToFix[plusXTilePosition];
                var plusZTile = TilesToFix[plusZTilePosition];
                var plusXPlusZTile = TilesToFix[plusXPlusZTilePosition];

                var currentMesh = keyValue.Value.GetComponent<MeshFilter>().mesh;
                var plusXTileMesh = plusXTile.GetComponent<MeshFilter>().mesh;
                var plusZTileMesh = plusZTile.GetComponent<MeshFilter>().mesh;
                var plusXPlusZTileMesh = plusXPlusZTile.GetComponent<MeshFilter>().mesh;

                var currentMeshCollider = keyValue.Value.GetComponent<MeshCollider>();
                var plusXTileMeshCollider = plusXTile.GetComponent<MeshCollider>();
                var plusZTileMeshCollider = plusZTile.GetComponent<MeshCollider>();
                var plusXPlusZTileMeshCollider = plusXPlusZTile.GetComponent<MeshCollider>();

                var currentMeshVertices = currentMesh.vertices;
                var plusXPlusZTileMeshVertices = plusXPlusZTileMesh.vertices;
                var plusXTileMeshVertices = plusXTileMesh.vertices;
                var plusZTileMeshVertices = plusZTileMesh.vertices;

                var minYPosition = new[]
                {
                    currentMeshVertices[0].y,
                    plusXTileMeshVertices[10].y,
                    plusZTileMeshVertices[110].y,
                    plusXPlusZTileMeshVertices[120].y
                }.Min();

                currentMeshVertices[0].y = minYPosition;
                plusXTileMeshVertices[10].y = minYPosition;
                plusZTileMeshVertices[110].y = minYPosition;
                plusXPlusZTileMeshVertices[120].y = minYPosition;

                currentMesh.vertices = currentMeshVertices;
                currentMesh.RecalculateBounds();
                currentMesh.RecalculateNormals();
                currentMeshCollider.sharedMesh = currentMesh;

                plusXTileMesh.vertices = plusXTileMeshVertices;
                plusXTileMesh.RecalculateBounds();
                plusXTileMesh.RecalculateNormals();
                plusXTileMeshCollider.sharedMesh = plusXTileMesh;

                plusZTileMesh.vertices = plusZTileMeshVertices;
                plusZTileMesh.RecalculateBounds();
                plusZTileMesh.RecalculateNormals();
                plusZTileMeshCollider.sharedMesh = plusZTileMesh;

                plusXPlusZTileMesh.vertices = plusXPlusZTileMeshVertices;
                plusXPlusZTileMesh.RecalculateBounds();
                plusXPlusZTileMesh.RecalculateNormals();
                plusXPlusZTileMeshCollider.sharedMesh = plusXPlusZTileMesh;
            }
        }

        _wasInited = true;
    }

    void Update()
    {
        if (DisableGeneration || !_wasInited)
        {
            return;
        }

        PlayerPosition = Player.transform.position;

        PlayerMaxXDifference = Math.Abs(MaxXMaxZPosition.x - PlayerPosition.x);
        PlayerMaxZDifference = Math.Abs(MaxXMaxZPosition.z - PlayerPosition.z);
        PlayerMinXDifference = Math.Abs(MinXMinZPosition.x - PlayerPosition.x);
        PlayerMinZDifference = Math.Abs(MinXMinZPosition.z - PlayerPosition.z);

        var newTilesDictionary = new Dictionary<Vector3, GameObject>();

        if (PlayerMaxXDifference < AcceptableRange) // +x
        {
            var needToRemoveTiles = Tiles
                .Keys
                .Where(x => x.x == MinXMaxZPosition.x)
                .ToList();

            foreach (var needToRemoveTile in needToRemoveTiles)
            {
                var tile = Tiles[needToRemoveTile];
                Destroy(tile);
                Tiles.Remove(needToRemoveTile);
            }

            var minXTile = Tiles
                .Min(x => x.Key.x);

            MinXMaxZPosition.x = minXTile;
            MinXMinZPosition.x = minXTile;

            var newXPosition = MaxXMaxZPosition.x + TileWidth;
            for (int zTileIndex = 0; zTileIndex < MapDepthInTiles; zTileIndex++)
            {
                var tilePosition = new Vector3(newXPosition,
                    transform.position.y,
                    MinXMinZPosition.z + zTileIndex * TileDepth);

                var tile = Instantiate(TilePrefab, tilePosition, Quaternion.identity);
                var tileGeneration = tile.AddComponent<Generation>();
                tileGeneration.Init(_generationSynchonizer);

                newTilesDictionary[tilePosition] = tile;

                MaxXMinZPosition.x = tilePosition.x;
                MaxXMaxZPosition.x = tilePosition.x;
            }

            PlayerMinXDifference += 10;
        }

        if (PlayerMinXDifference < AcceptableRange) // -x
        {
            var needToRemoveTiles = Tiles
                .Keys
                .Where(x => x.x == MaxXMaxZPosition.x)
                .ToList();

            foreach (var needToRemoveTile in needToRemoveTiles)
            {
                var tile = Tiles[needToRemoveTile];
                Destroy(tile);
                Tiles.Remove(needToRemoveTile);
            }

            var maxXTile = Tiles
                .Max(x => x.Key.x);

            MaxXMaxZPosition.x = maxXTile;
            MaxXMinZPosition.x = maxXTile;

            var newXPosition = MinXMaxZPosition.x - TileWidth;
            for (int zTileIndex = 0; zTileIndex < MapDepthInTiles; zTileIndex++)
            {
                var tilePosition = new Vector3(newXPosition,
                    transform.position.y,
                    MinXMinZPosition.z + zTileIndex * TileDepth);

                var tile = Instantiate(TilePrefab, tilePosition, Quaternion.identity);
                var tileGeneration = tile.AddComponent<Generation>();
                tileGeneration.Init(_generationSynchonizer);

                newTilesDictionary[tilePosition] = tile;

                MinXMinZPosition.x = tilePosition.x;
                MinXMaxZPosition.x = tilePosition.x;
            }

            PlayerMaxXDifference += 10;
        }

        if (PlayerMaxZDifference < AcceptableRange) // +z
        {
            var needToRemoveTiles = Tiles
                .Keys
                .Where(x => x.z == MinXMinZPosition.z)
                .ToList();

            foreach (var needToRemoveTile in needToRemoveTiles)
            {
                var tile = Tiles[needToRemoveTile];
                Destroy(tile);
                Tiles.Remove(needToRemoveTile);
            }

            var minZTile = Tiles
                .Min(x => x.Key.z);

            MinXMinZPosition.z = minZTile;
            MaxXMinZPosition.z = minZTile;

            var newZPosition = MaxXMaxZPosition.z + TileDepth;
            for (int xTileIndex = 0; xTileIndex < MapWidthInTiles; xTileIndex++)
            {
                var tilePosition = new Vector3(MinXMaxZPosition.x + xTileIndex * TileWidth,
                    transform.position.y,
                    newZPosition);

                var tile = Instantiate(TilePrefab, tilePosition, Quaternion.identity);
                var tileGeneration = tile.AddComponent<Generation>();
                tileGeneration.Init(_generationSynchonizer);

                newTilesDictionary[tilePosition] = tile;

                MaxXMaxZPosition.z = tilePosition.z;
                MinXMaxZPosition.z = tilePosition.z;
            }

            PlayerMinZDifference += 10;
        }

        if (PlayerMinZDifference < AcceptableRange) // -z
        {
            var needToRemoveTiles = Tiles
                .Keys
                .Where(x => x.z == MinXMaxZPosition.z)
                .ToList();

            foreach (var needToRemoveTile in needToRemoveTiles)
            {
                var tile = Tiles[needToRemoveTile];
                Destroy(tile);
                Tiles.Remove(needToRemoveTile);
            }

            var maxZTile = Tiles
                .Max(x => x.Key.z);

            MinXMaxZPosition.z = maxZTile;
            MaxXMaxZPosition.z = maxZTile;

            var newZPosition = MinXMinZPosition.z - TileDepth;
            for (int xTileIndex = 0; xTileIndex < MapWidthInTiles; xTileIndex++)
            {
                var tilePosition = new Vector3(MinXMaxZPosition.x + xTileIndex * TileWidth,
                    transform.position.y,
                    newZPosition);

                var tile = Instantiate(TilePrefab, tilePosition, Quaternion.identity);
                var tileGeneration = tile.AddComponent<Generation>();
                tileGeneration.Init(_generationSynchonizer);

                newTilesDictionary[tilePosition] = tile;

                MaxXMinZPosition.z = tilePosition.z;
                MinXMinZPosition.z = tilePosition.z;
            }

            PlayerMaxZDifference += 10;
        }

        if (newTilesDictionary.Count > 0)
        {
            TilesToFix = newTilesDictionary;

            var firstNewTilePosition = newTilesDictionary
                .First()
                .Value
                .transform
                .position;

            var plusXNewTilePosition =
                new Vector3(firstNewTilePosition.x + 10, firstNewTilePosition.y, firstNewTilePosition.z);
            var minusXNewTilePosition =
                new Vector3(firstNewTilePosition.x - 10, firstNewTilePosition.y, firstNewTilePosition.z);
            var plusZNewTilePosition =
                new Vector3(firstNewTilePosition.x, firstNewTilePosition.y, firstNewTilePosition.z + 10);
            var minusZNewTilePosition =
                new Vector3(firstNewTilePosition.x, firstNewTilePosition.y, firstNewTilePosition.z - 10);

            if (Tiles.ContainsKey(plusXNewTilePosition)) //-x
            {
                foreach (var positionTile in TilesToFix)
                {
                    Tiles[positionTile.Key] = positionTile.Value;
                }

                var additionalTilesToFix = Tiles
                    .Where(x => x.Key.x == plusXNewTilePosition.x)
                    .ToList();

                TilesToFix = TilesToFix
                    .Concat(additionalTilesToFix)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                if (Tiles.ContainsKey(minusXNewTilePosition)) //+x
                {
                    foreach (var positionTile in TilesToFix)
                    {
                        Tiles[positionTile.Key] = positionTile.Value;
                    }

                    var additionalTilesToFix = Tiles
                        .Where(x => x.Key.x == minusXNewTilePosition.x)
                        .ToList();

                    TilesToFix = TilesToFix
                        .Concat(additionalTilesToFix)
                        .ToDictionary(x => x.Key, x => x.Value);
                }
                else
                {
                    if (Tiles.ContainsKey(plusZNewTilePosition)) //-z
                    {
                        foreach (var positionTile in TilesToFix)
                        {
                            Tiles[positionTile.Key] = positionTile.Value;
                        }

                        var additionalTilesToFix = Tiles
                            .Where(x => x.Key.z == plusZNewTilePosition.z)
                            .ToList();

                        TilesToFix = TilesToFix
                            .Concat(additionalTilesToFix)
                            .ToDictionary(x => x.Key, x => x.Value);
                    }
                    else
                    {
                        if (Tiles.ContainsKey(minusZNewTilePosition)) //+z
                        {
                            foreach (var positionTile in TilesToFix)
                            {
                                Tiles[positionTile.Key] = positionTile.Value;
                            }

                            var additionalTilesToFix = Tiles
                                .Where(x => x.Key.z == minusZNewTilePosition.z)
                                .ToList();

                            TilesToFix = TilesToFix
                                .Concat(additionalTilesToFix)
                                .ToDictionary(x => x.Key, x => x.Value);
                        }
                    }
                }
            }

            Invoke("FixMap", 0.01f);
        }
    }

    void GenerateMap()
    {
        var tileSize = TilePrefab.GetComponent<MeshRenderer>().bounds.size;
        TileWidth = (int) tileSize.x;
        TileDepth = (int) tileSize.z;

        Tiles = new Dictionary<Vector3, GameObject>();

        for (int xTileIndex = 0; xTileIndex < MapWidthInTiles; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < MapDepthInTiles; zTileIndex++)
            {
                if (SceneData.GenerateArena && xTileIndex >= _startArenaTileIndex &&
                    xTileIndex < _finishArenaTileIndex &&
                    zTileIndex >= _startArenaTileIndex && zTileIndex < _finishArenaTileIndex)
                {
                    continue;
                }
                var position = transform.position;
                var tilePosition = new Vector3(position.x + xTileIndex * TileWidth,
                    position.y,
                    position.z + zTileIndex * TileDepth);

                if (xTileIndex == 0 && zTileIndex == 0)
                {
                    MinXMinZPosition = tilePosition;
                }

                if (xTileIndex == MapWidthInTiles - 1 && zTileIndex == MapDepthInTiles - 1)
                {
                    MaxXMaxZPosition = tilePosition;
                }

                if (xTileIndex == MapWidthInTiles - 1 && zTileIndex == 0)
                {
                    MaxXMinZPosition = tilePosition;
                }

                if (xTileIndex == 0 && zTileIndex == MapDepthInTiles - 1)
                {
                    MinXMaxZPosition = tilePosition;
                }

                var tile = Instantiate(TilePrefab, tilePosition, Quaternion.identity);
                var tileGeneration = tile.AddComponent<Generation>();

                tileGeneration.Init(_generationSynchonizer);

                Tiles[tile.transform.position] = tile;
            }
        }

        if (SceneData.GenerateArena)
        {
            var arenaBiome = new Biome();
            arenaBiome.Generate(-1, null);
            
            var nearlyTiles = new List<GameObject>();

            for (int i = _startArenaTileIndex - 1; i < _finishArenaTileIndex + 1; i++)
            {
                var position = transform.position;
                var x = _startArenaTileIndex - 1;
                nearlyTiles.Add(Tiles[new Vector3(x * 10, position.y, i * 10)]);

                x = _finishArenaTileIndex + 1;
                nearlyTiles.Add(Tiles[new Vector3(x * 10, position.y, i * 10)]);
            }

            for (int i = _startArenaTileIndex; i < _finishArenaTileIndex; i++)
            {
                var position = transform.position;
                var z = _startArenaTileIndex - 1;
                nearlyTiles.Add(Tiles[new Vector3(i * 10, position.y, z * 10)]);
            
                z = _finishArenaTileIndex + 1;
                nearlyTiles.Add(Tiles[new Vector3(i * 10, position.y, z * 10)]);
            }

            var maxTileYPosition = nearlyTiles
                .SelectMany(x => x.GetComponent<MeshFilter>().mesh.vertices)
                .Max(x => x.y);
        
            Player.transform.position = new Vector3(_halfOfMapSize, maxTileYPosition + 15, _halfOfMapSize);

            for (int xTileIndex = _startArenaTileIndex; xTileIndex < _finishArenaTileIndex; xTileIndex++)
            {
                for (int zTileIndex = _startArenaTileIndex; zTileIndex < _finishArenaTileIndex; zTileIndex++)
                {
                    var position = transform.position;
                    var tilePosition = new Vector3(position.x + xTileIndex * TileWidth,
                        position.y,
                        position.z + zTileIndex * TileDepth);
                
                    var tile = Instantiate(TilePrefab, tilePosition, Quaternion.identity);
                
                    var tileGeneration = tile.AddComponent<Generation>();

                    tileGeneration.Init(_generationSynchonizer, maxTileYPosition + 10, arenaBiome);
                
                    Tiles[tile.transform.position] = tile;
                }
            }
        }
    }
}