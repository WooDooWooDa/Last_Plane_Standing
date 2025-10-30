//using System;

using System;
using System.Collections.Generic;
using System.Linq;
using Plane;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    
    public class InfiniteMap : MonoBehaviour
    {
        private struct MapBorders
        {
            public int MinX;
            public int MaxX;
            public int MinY;
            public int MaxY;
        }
        
        [Header("Chunks Settings")] 
        [SerializeField] private string _preGeneratedMapChunkPath;
        [SerializeField] private Vector2Int _mapChunkSize = new(18, 18);

        [Header("Init Settings")] 
        [SerializeField] private Transform _mapTransformParent;
        [SerializeField] private Vector2Int _mapSizeOnInit;
        [SerializeField] private GameObject _centerChunk;

        [Header("Expansion Settings")] 
        [SerializeField] private PlayerPlane _playerPlane;
        [SerializeField] private int _newChunkAtFromEdge = 1;

        private List<GameObject> _mapChunkPrefabs = new();
        private Dictionary<Vector2Int, GameObject> _activeMapChunks = new();
        private MapBorders _currentMapBorders;

        private void Awake()
        {
            LoadPreGeneratedChunks();
            InitMapChunks();
        }

        private void Update()
        {
            CheckForMapExpansion();
        }

        private void CheckForMapExpansion()
        {
            if (_playerPlane is null) return;
            
            var playerPos = _playerPlane.transform.position;
            var playerCurrentChunk = WorldPosToChunkPos(playerPos);

            if (playerCurrentChunk.x >= _currentMapBorders.MaxX - _newChunkAtFromEdge)
                AddMapColumn(_currentMapBorders.MaxX + 1);
            else if (playerCurrentChunk.x <= _currentMapBorders.MinX + _newChunkAtFromEdge)
                AddMapColumn(_currentMapBorders.MinX - 1);
            if (playerCurrentChunk.y >= _currentMapBorders.MaxY - _newChunkAtFromEdge)
                AddMapRow(_currentMapBorders.MaxY + 1);
            else if (playerCurrentChunk.y <= _currentMapBorders.MinY + _newChunkAtFromEdge)
                AddMapRow(_currentMapBorders.MinY - 1);
        }

        private void LoadPreGeneratedChunks()
        {
            _mapChunkPrefabs = Resources.LoadAll<GameObject>(_preGeneratedMapChunkPath).ToList();
        }

        private void InitMapChunks()
        {
            foreach (var (pos, chunk) in _activeMapChunks)
            {
                Destroy(chunk);
            }

            _activeMapChunks.Clear();

            for (var i = -_mapSizeOnInit.x / 2; i <= _mapSizeOnInit.x / 2; i++)
            {
                for (var j = -_mapSizeOnInit.y / 2; j <= _mapSizeOnInit.y / 2; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        _activeMapChunks.Add(new Vector2Int(0, 0), _centerChunk);
                        continue;
                    }
                    CreateNewChunk(i, j);
                }
            }

            _currentMapBorders = new MapBorders();
            UpdateMapBorders();
        }

        private void AddMapColumn(int newColX)
        {
            for (int y = _currentMapBorders.MinY; y <= _currentMapBorders.MaxY; y++)
            {
                CreateNewChunk(newColX, y);
            }
            
            UpdateMapBorders();
        }
        
        private void AddMapRow(int newRowY)
        {
            for (int x = _currentMapBorders.MinX; x <= _currentMapBorders.MaxX; x++)
            {
                CreateNewChunk(x, newRowY);
            }
            
            UpdateMapBorders();
        }

        private void CreateNewChunk(int x, int y)
        {
            var idx = Random.Range(0, _mapChunkPrefabs.Count);
            var newMapChunk = Instantiate(_mapChunkPrefabs[idx], _mapTransformParent);
            newMapChunk.transform.localPosition = ChunkPosToWorldPos(x, y);
            var shouldFlip = Random.Range(0, 2) == 0;
            newMapChunk.transform.localRotation = shouldFlip ? Quaternion.Euler(0,180,0) : Quaternion.identity;
            newMapChunk.name = "MapChunk_" + x + "_" + y;
            _activeMapChunks.Add(new Vector2Int(x, y), newMapChunk);
        }
        
        private Vector3 ChunkPosToWorldPos(int x, int y)
        {
            return new Vector3(
                x * _mapChunkSize.x,// + (_mapChunkSize.x / 2f),
                y * _mapChunkSize.y,// + (_mapChunkSize.y / 2f),
                0);
        }

        private Vector2Int WorldPosToChunkPos(Vector3 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / _mapChunkSize.x),
                Mathf.FloorToInt(pos.y / _mapChunkSize.y));
        }

        private void UpdateMapBorders()
        {
            var mapPositions = _activeMapChunks.Keys.ToList();
            _currentMapBorders.MinX = mapPositions.Min(p => p.x);
            _currentMapBorders.MaxX = mapPositions.Max(p => p.x);
            _currentMapBorders.MinY = mapPositions.Min(p => p.y);
            _currentMapBorders.MaxY = mapPositions.Max(p => p.y);
        }
    }
}