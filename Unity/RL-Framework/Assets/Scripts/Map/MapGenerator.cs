using System;
using Assets.Scripts;
using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public Sprite pathPrefab;
    public Sprite spawnPrefab;
    public Sprite castlePrefab;

    private const int mapSize = 15;

    public MapTile[,] Map = new MapTile[mapSize, mapSize];
    public int seed;

    void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // If a seed is not set, use a random seed
        if (seed == 0)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            UnityEngine.Random.InitState(seed);
        }
        else
        {
            // Initialize the random number generator with the provided seed
            UnityEngine.Random.InitState(seed);
        }

        Debug.Log($"Map Seed: {seed}"); // Print the seed to the console

        GenerateTiles();
        MarkBorder();
        var path = GeneratePath().ToArray();
        GameController.Instance.Path = path;
        GameController.Instance.Map = Map;
        PlaceCastle(path);
        PlaceSpawn(path);

        transform.rotation = Quaternion.Euler(-90, 0, 0);
    }

    private void GenerateTiles()
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                var tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tile.transform.Rotate(90, 0, 0);
                tile.name = $"Tile[{x},{y}]";
                Map[x, y] = tile.GetComponent<MapTile>();
                Map[x, y].Type = TileType.Empty;
            }
        }
    }

    private void MarkBorder()
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if (x == 0 || y == 0 || x == mapSize - 1 || y == mapSize - 1)
                {
                    Map[x, y].Type = TileType.Blocked;
                }
            }
        }
    }

    private List<MapTile> GeneratePath()
    {
        List<MapTile> pathTiles = new List<MapTile>();

        int x = mapSize / 2;
        int y = 1;

        MapTile startTile = Map[x, y];
        pathTiles.Add(startTile);
        startTile.Type = TileType.Path;

        bool movingHorizontally = true;
        int direction = 1;

        int noValidMoveCount = 0;

        while (y < mapSize - 2 && noValidMoveCount < 10)
        {
            int nextX = x, nextY = y;

            if (movingHorizontally)
            {
                nextX += direction;

                if (nextX <= 0 || nextX >= mapSize - 1)
                {
                    direction = -direction;
                    nextX = x;
                    movingHorizontally = false;
                }
            }
            else
            {
                nextY += 1;
            }

            if (IsTileValid(nextX, nextY, pathTiles))
            {
                x = nextX;
                y = nextY;
                MapTile currentTile = Map[x, y];
                pathTiles.Add(currentTile);
                currentTile.Type = TileType.Path;
                noValidMoveCount = 0;
                var changeDirectionChance = movingHorizontally ? 5 : 40;
                if (y < mapSize - 1 && UnityEngine.Random.Range(0, 100) < changeDirectionChance)
                {
                    movingHorizontally = !movingHorizontally;
                    direction = UnityEngine.Random.Range(0, 2) * 2 - 1;
                }
            }
            else
            {
                noValidMoveCount++;
                movingHorizontally = !movingHorizontally;
                direction = UnityEngine.Random.Range(0, 2) * 2 - 1;
            }
        }

        if (noValidMoveCount >= 10)
        {
            Debug.LogError("Failed to generate a complete path, terminating early.");
        }

        pathTiles.ForEach(t => t.SetSprite(pathPrefab));

        return pathTiles;
    }

    private bool IsTileValid(int x, int y, List<MapTile> pathTiles)
    {
        if (x < 1 || x >= mapSize - 1 || y < 1 || y >= mapSize - 1)
            return false;
        if (Map[x, y].Type != TileType.Empty || pathTiles.Contains(Map[x, y]))
            return false;
        return true;
    }

    private void PlaceCastle(MapTile[] path)
    {
        var castleTile = path[^1];
        castleTile.Type = TileType.Castle;
        castleTile.SetSprite(castlePrefab);
        castleTile.gameObject.AddComponent<Castle>();
        GameController.Instance.Castle = castleTile.GetComponent<Castle>();
    }

    private void PlaceSpawn(MapTile[] path)
    {
        var spawnTile = path[0];
        spawnTile.Type = TileType.SpawnPoint;
        spawnTile.SetSprite(spawnPrefab);
        spawnTile.gameObject.AddComponent<EnemySpawner>();
        GameController.Instance.Spawn = spawnTile.GetComponent<EnemySpawner>();
    }
}
