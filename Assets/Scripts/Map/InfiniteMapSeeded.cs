using System;
using System.Collections.Generic;
using System.Linq;
using Plane;
using UnityEngine;

namespace Map
{
    public class InfiniteMapSeeded : MonoBehaviour
    {
        [Header("Chunks Settings")] 
        [SerializeField] private string _preGeneratedMapChunkPath;
        [SerializeField] private Vector2Int _mapChunkSize = new(18, 18);
        
        [Header("Expansion Settings")] 
        [SerializeField] private Transform _mapTransformParent;
        [SerializeField] private PlayerPlane _playerPlane;
        [SerializeField] private int _loadRadius = 5;
        
        private List<GameObject> _mapChunkPrefabs = new();
        
        private Dictionary<Vector2Int, GameObject> _activeMapChunks = new();
        private Vector2Int _lastPlayerChunk;

        private void Awake()
        {
            LoadPreGeneratedChunks();
        }

        private void Start()
        {
            if (_playerPlane is null)
            {
                Debug.LogError("PlayerPlane is null in map");
                enabled = false;
                return;
            }
            
            UpdateChunks();
        }

        private void Update()
        {
            var playerChunk = WorldPosToChunkPos(_playerPlane.transform.position);
            if (playerChunk != _lastPlayerChunk)
            {
                UpdateChunks();
                _lastPlayerChunk = playerChunk;
            }
        }

        private void LoadPreGeneratedChunks()
        {
            _mapChunkPrefabs = Resources.LoadAll<GameObject>(_preGeneratedMapChunkPath).ToList();
        }

        private void UpdateChunks()
        {
            var playerChunk = WorldPosToChunkPos(_playerPlane.transform.position);
            for (int x = -_loadRadius; x <= _loadRadius; x++)
            {
                for (int y = -_loadRadius; y <= _loadRadius; y++)
                {
                    if (_activeMapChunks.ContainsKey(playerChunk)) continue;
                    CreateNewChunk(x, y);
                }
            }
        }

        private void CreateNewChunk(int x, int y)
        {
            var chunkPos = new Vector2Int(x, y);
            if (_activeMapChunks.ContainsKey(chunkPos)) return;
            
            var idx = GetPrefabIndexFromNoise(x, y);
            var newMapChunk = Instantiate(_mapChunkPrefabs[idx], _mapTransformParent);
            newMapChunk.transform.localPosition = ChunkPosToWorldPos(x, y);
            newMapChunk.name = "MapChunk_" + x + "_" + y;
            _activeMapChunks.Add(chunkPos, newMapChunk);
        }
        
        private int GetPrefabIndexFromNoise(int x, int y)
        {
            float nx = (x + 123 * 0.1234f) * .5f;
            float ny = (y + 123 * 0.5678f) * .5f;
            
            float noiseValue = Mathf.PerlinNoise(nx, ny); // 0 → 1
            int index = Mathf.FloorToInt(noiseValue * _mapChunkPrefabs.Count);
            return Mathf.Clamp(index, 0, _mapChunkPrefabs.Count - 1);
        }
        
        private Vector3 ChunkPosToWorldPos(int x, int y)
        {
            return new Vector3(
                x * _mapChunkSize.x + (_mapChunkSize.x / 2f),
                y * _mapChunkSize.y + (_mapChunkSize.y / 2f),
                0);
        }

        private Vector2Int WorldPosToChunkPos(Vector3 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / _mapChunkSize.x),
                Mathf.FloorToInt(pos.y / _mapChunkSize.y));
        }
    }
}