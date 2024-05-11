using System;
using Assets.Scripts;
using Assets.Scripts.Map;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameController GameController;
    public GameObject tilePrefab;
    public Sprite pathPrefab;
    public Sprite spawnPrefab;
    public Sprite castlePrefab;

    public const int MapSize = 15;

    public MapTile[,] Map = new MapTile[MapSize, MapSize];
    public int seed;

    void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        if (seed == 0)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            UnityEngine.Random.InitState(seed);
        }
        else
        {
            UnityEngine.Random.InitState(seed);
        }

        Debug.Log($"Map Seed: {seed}"); 

        GenerateTiles();
        MarkBorder();
        var path = GeneratePath().ToArray();
        GameController.Path = path;
        GameController.Map = Map;
        PlaceCastle(path);
        PlaceSpawn(path);

        transform.rotation = Quaternion.Euler(-90, 0, 0);
    }

    private void GenerateTiles()
    {
        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                Vector3 position = new(x + transform.position.x, transform.position.y, y + transform.position.z);
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
        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                if (x == 0 || y == 0 || x == MapSize - 1 || y == MapSize - 1)
                {
                    Map[x, y].Type = TileType.Blocked;
                }
            }
        }
    }

    private List<MapTile> GeneratePath()
    {
        var pathTiles = new List<MapTile>();

        int x = MapSize / 2;
        int y = 1;

        MapTile startTile = Map[x, y];
        pathTiles.Add(startTile);
        startTile.Type = TileType.Path;

        bool movingHorizontally = true;
        int direction = 1;

        int noValidMoveCount = 0;

        while (y < MapSize - 2 && noValidMoveCount < 10)
        {
            int nextX = x, nextY = y;

            if (movingHorizontally)
            {
                nextX += direction;

                if (nextX <= 0 || nextX >= MapSize - 1)
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
                if (y < MapSize - 1 && UnityEngine.Random.Range(0, 100) < changeDirectionChance)
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
        if (x < 1 || x >= MapSize - 1 || y < 1 || y >= MapSize - 1)
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
        castleTile.GetComponent<Castle>().GameController = GameController;
        GameController.Castle = castleTile.GetComponent<Castle>();
    }

    private void PlaceSpawn(MapTile[] path)
    {
        var spawnTile = path[0];
        spawnTile.Type = TileType.SpawnPoint;
        spawnTile.SetSprite(spawnPrefab);
        spawnTile.gameObject.AddComponent<EnemySpawner>();
        spawnTile.GetComponent<EnemySpawner>().GameController = GameController;
        GameController.Spawn = spawnTile.GetComponent<EnemySpawner>();
    }
}
