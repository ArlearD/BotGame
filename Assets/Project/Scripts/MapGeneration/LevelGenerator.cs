using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidthInTiles, mapDepthInTiles;

    [SerializeField] private GameObject tilePrefab;

    [SerializeField] private GameObject navMesh;

    [SerializeField] private Dictionary<Vector3, GameObject> Tiles;

    private Vector3 MinXMinZPosition;
    private Vector3 MaxXMaxZPosition;
    private Vector3 MaxXMinZPosition;
    private Vector3 MinXMaxZPosition;
    private bool DisableGeneration = true;

    private float AcceptableRange;
    private float PlayerMinXDifference;

    private int tileWidth;
    private int tileDepth;

    void Start()
    {
        GenerateMap();
        AcceptableRange = Math.Abs(MinXMinZPosition.x - MaxXMaxZPosition.x) - 40;

        Invoke("UpdateNavMesh", 0.2f);
    }

    private void UpdateNavMesh()
    {
        var a = navMesh.GetComponent<NavMeshSurface>();
        a.BuildNavMesh();
    }

    void Update()
    {
        if (DisableGeneration)
        {
            return;
        }

        if (0 < AcceptableRange)
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

            var minXTile = Tiles.Min(x => x.Key.x);

            MinXMaxZPosition.x = minXTile;
            MinXMinZPosition.x = minXTile;

            var newXPosition = MaxXMaxZPosition.x + tileWidth;
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
            {
                Vector3 tilePosition = new Vector3(newXPosition,
                    transform.position.y,
                    MinXMinZPosition.z + zTileIndex * tileDepth);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                MaxXMinZPosition.x = tilePosition.x;
                MaxXMaxZPosition.x = tilePosition.x;
                Tiles[tile.transform.position] = tile;
            }

            PlayerMinXDifference -= 10;
        }

        if (PlayerMinXDifference < AcceptableRange)
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

            var maxXTile = Tiles.Max(x => x.Key.x);

            MaxXMaxZPosition.x = maxXTile;
            MaxXMinZPosition.x = maxXTile;

            var newXPosition = MinXMaxZPosition.x - tileWidth;
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
            {
                Vector3 tilePosition = new Vector3(newXPosition,
                    transform.position.y,
                    MinXMinZPosition.z + zTileIndex * tileDepth);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                MinXMinZPosition.x = tilePosition.x;
                MinXMaxZPosition.x = tilePosition.x;
                Tiles[tile.transform.position] = tile;
            }
        }
    }

    void GenerateMap()
    {
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        tileWidth = (int)tileSize.x;
        tileDepth = (int)tileSize.z;

        Tiles = new Dictionary<Vector3, GameObject>();

        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
            {
                Vector3 tilePosition = new Vector3(this.gameObject.transform.position.x + xTileIndex * tileWidth,
                    transform.position.y,
                    transform.position.z + zTileIndex * tileDepth);

                if (xTileIndex == 0 && zTileIndex == 0)
                {
                    MinXMinZPosition = tilePosition;
                }

                if (xTileIndex == mapWidthInTiles - 1 && zTileIndex == mapDepthInTiles - 1)
                {
                    MaxXMaxZPosition = tilePosition;
                }

                if (xTileIndex == mapWidthInTiles - 1 && zTileIndex == 0)
                {
                    MaxXMinZPosition = tilePosition;
                }

                if (xTileIndex == 0 && zTileIndex == mapDepthInTiles - 1)
                {
                    MinXMaxZPosition = tilePosition;
                }

                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                Tiles[tile.transform.position] = tile;
            }
        }
    }
}