using System.Collections.Generic;
using Assets.Scripts.Units;
using UnityEngine;

#pragma warning disable CS0649 // Assigned via Unity editor

namespace Assets.Scripts.Map
{
    public class MapGenerator : MonoBehaviour
    {
        public const int MapSize = 15;
        public MapTile[,] Map = new MapTile[MapSize, MapSize];

        [SerializeField] private GameController _gameController;

        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private Sprite _pathPrefab;
        [SerializeField] private Sprite _spawnPrefab;
        [SerializeField] private Sprite _castlePrefab;

        [SerializeField] private bool _useRandomSeed;
        [SerializeField] private int _seed;

        private void Awake()
        {
            _gameController.OnGameReset += GenerateMap;
            GenerateMap();
        }

        public void GenerateMap()
        {
            SetSeed();

            ClearMap();
            GenerateTiles();
            MarkBorder();
            var path = GeneratePath().ToArray();
            PlaceCastle(path);
            PlaceSpawn(path);
            UnMarkBorder();

            _gameController.Path = path;
            _gameController.Map = Map;
        }

        private void SetSeed()
        {
            if (_useRandomSeed)
            {
                _seed = Random.Range(int.MinValue, int.MaxValue);
            }

            Random.InitState(_seed);
        }

        private void ClearMap()
        {
            while (transform.childCount > 0)
                DestroyImmediate(transform.GetChild(0).gameObject);
        }

        private void GenerateTiles()
        {
            for (int x = 0; x < MapSize; x++)
            for (int y = 0; y < MapSize; y++)
                InitializeTile(x, y);
        }

        private void InitializeTile(int x, int y)
        {
            Vector3 position = new(x + transform.position.x, y + transform.position.y, transform.position.z);
            var tile = Instantiate(_tilePrefab, position, Quaternion.Euler(-90, 0, 0), transform);
            tile.transform.Rotate(90, 0, 0);
            tile.name = $"Tile[{x},{y}]";
            Map[x, y] = tile.GetComponent<MapTile>();
            Map[x, y].Type = TileType.Empty;
        }

        private void MarkBorder()
        {
            for (int x = 0; x < MapSize; x++)
            for (int y = 0; y < MapSize; y++)
                if (x == 0 || y == 0 || x == MapSize - 1 || y == MapSize - 1)
                    Map[x, y].Type = TileType.Blocked;
        }

        private void UnMarkBorder()
        {
            for (int x = 0; x < MapSize; x++)
            for (int y = 0; y < MapSize; y++)
                if (x == 0 || y == 0 || x == MapSize - 1 || y == MapSize - 1)
                    Map[x, y].Type = TileType.Empty;
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
                    if (y < MapSize - 1 && Random.Range(0, 100) < changeDirectionChance)
                    {
                        movingHorizontally = !movingHorizontally;
                        direction = (Random.Range(0, 2) * 2) - 1;
                    }
                }
                else
                {
                    noValidMoveCount++;
                    movingHorizontally = !movingHorizontally;
                    direction = (Random.Range(0, 2) * 2) - 1;
                }
            }

            if (noValidMoveCount >= 10)
            {
                Debug.LogError("Failed to generate a complete path, terminating early.");
            }

            pathTiles.ForEach(t => t.SetSprite(_pathPrefab));

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
            castleTile.SetSprite(_castlePrefab);
            castleTile.gameObject.AddComponent<Castle>();
            castleTile.GetComponent<Castle>().GameController = _gameController;
            _gameController.Castle = castleTile.GetComponent<Castle>();
        }

        private void PlaceSpawn(MapTile[] path)
        {
            var spawnTile = path[0];
            spawnTile.Type = TileType.SpawnPoint;
            spawnTile.SetSprite(_spawnPrefab);
            spawnTile.gameObject.AddComponent<EnemySpawner>();
            spawnTile.GetComponent<EnemySpawner>().GameController = _gameController;
            _gameController.Spawn = spawnTile.GetComponent<EnemySpawner>();
        }
    }
}