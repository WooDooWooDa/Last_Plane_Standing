using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Map
{
    [ExecuteInEditMode]
    public class EditorMapChunkGenerator : MonoBehaviour
    {
        [Header("Map Chunk")]
        [SerializeField] private GameObject _currentGeneratedMapChunk;
        [SerializeField] private Tilemap _islandTilemap;
        [Header("Save Settings")]
        [SerializeField] private string _mapChunkSavePath;
        [SerializeField] private string _mapChunkSaveNamePrefix;
        [Header("Chunk Settings")]
        [SerializeField] private RuleTile _islandRuleTile;
        [SerializeField] private Vector2Int _mapChunkSize;
        [SerializeField] private Vector2Int _islandCountRange;
        [SerializeField] private Vector2 _averageIslandSizeRange;

        private int _mapChunkPrefabCount;
        private bool[,] _islandTilePositions;

        private void Start()
        {
            var isPlaying = Application.isPlaying;
            gameObject.SetActive(!isPlaying);
            
            #if UNITY_EDITOR
            //Count every chunk prefab as of now
            _mapChunkPrefabCount = AssetDatabase.FindAssets("t:Prefab", new[] { _mapChunkSavePath }).Length;
            #endif
            
            ResetCurrentMapChunk();
        }
        
        [Button, DisableInPlayMode]
        private void GenerateMapChunk()
        {
            ResetCurrentMapChunk();
            var islandCound = Random.Range(_islandCountRange.x, _islandCountRange.y + 1);
            for (var i = 0; i < islandCound; i++)
            {
                var islandSize = Random.Range(_averageIslandSizeRange.x, _averageIslandSizeRange.y);
                //islandSize -= islandSize % 2;   //Make sure island are squared
                var islandCenter = new Vector2Int(
                    Random.Range(0, _mapChunkSize.x + 1),
                    Random.Range(0, _mapChunkSize.y +1)
                );
                
                GrowIsland(islandCenter, islandSize);
            }

            CorrectMap();
            PlaceTiles();
        }

        private void GrowIsland(Vector2Int center, float size)
        {
            var visited = new HashSet<Vector2Int>();
            var frontier = new Queue<Vector2Int>();
            frontier.Enqueue(center);
            visited.Add(center);

            var placed = 0;

            while (frontier.Count > 0 && placed < size)
            {
                var current = frontier.Dequeue();

                if (IsInBounds(current))
                {
                    _islandTilePositions[current.x, current.y] = true;
                    placed++;
                    
                    var neighborCount = Random.Range(2, 5); // 2-4 neighbors
                    for (var i = 0; i < neighborCount; i++)
                    {
                        //tries++;
                        var offset = GetRandomDirection();
                        var neighbor = current + offset;
                        if (!visited.Contains(neighbor) && IsInBounds(neighbor))
                        {
                            visited.Add(neighbor);
                            frontier.Enqueue(neighbor);
                        }
                    }
                }
            }
        }
        
        private bool IsInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _mapChunkSize.x && pos.y >= 0 && pos.y < _mapChunkSize.y;
        }
        
        private static Vector2Int[] dirs = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        
        private static Vector2Int GetRandomDirection()
        {
            return dirs[Random.Range(0, dirs.Length)];
        }

        private void CorrectMap()
        {
            for (var x = 0; x < _mapChunkSize.x; x++)
            {
                for (var y = 0; y < _mapChunkSize.y; y++)
                {
                    if (!_islandTilePositions[x, y]) continue;
                    
                    var pos = new Vector2Int(x, y);   
                    var nbNeighbors = 0;
                    foreach (var dir in dirs)
                    {
                        var n = pos + dir;
                        if (IsInBounds(n) && _islandTilePositions[n.x, n.y])
                            nbNeighbors++;
                    }                        

                    if (nbNeighbors < 2)
                    {
                        _islandTilePositions[x, y] = false;
                    }
                }
            }
        }

        private void PlaceTiles()
        {
            for (var x = 0; x < _mapChunkSize.x; x++)
            {
                for (var y = 0; y < _mapChunkSize.y; y++)
                {
                    if (_islandTilePositions[x, y])
                    {
                        _islandTilemap.SetTile(new Vector3Int(x - _mapChunkSize.x / 2, y - _mapChunkSize.y / 2, 0), _islandRuleTile);
                    }
                    //_islandTilemap.SetTile(new Vector3Int(x, y, 0), _islandRuleTile);
                }
            }
        }

#if UNITY_EDITOR
        [Button, DisableInPlayMode]
        private void SaveMapChunk()
        {
            var path = _mapChunkSavePath + _mapChunkSaveNamePrefix + _mapChunkPrefabCount + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(_currentGeneratedMapChunk, path, out var success);
            if (success)
            {
                _mapChunkPrefabCount++;
            }
            else
            {
                Debug.LogError("Failed to save map chunk");
            }
        }
        #endif

        [Button, DisableInPlayMode]
        private void ResetCurrentMapChunk()
        {
            _islandTilePositions = new bool[_mapChunkSize.x, _mapChunkSize.y];
            _islandTilemap?.ClearAllTiles();
        }
    }
}