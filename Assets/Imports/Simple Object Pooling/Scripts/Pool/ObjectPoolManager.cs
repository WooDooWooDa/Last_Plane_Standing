using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackCatPool
{   
    [Serializable]
    public class PoolObjectData
    {
        [SerializeField] private GameObject poolObject;
        [Min(1)][SerializeField] private int capacity = 10;
        [SerializeField] private bool isExpandable = true;
        [SerializeField] private bool persistBetweenScenes = false;
        public GameObject PoolObject => poolObject;
        public int Capacity => capacity;
        public bool IsExpandable => isExpandable;
    }

    public class ObjectPoolManager : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private bool hierarchyOrganisation = true;
        public bool ShouldOrganiseHierarchy => hierarchyOrganisation;
#endif

        [SerializeField] private List<PoolObjectData> ObjectsToPool = new();

        private Dictionary<GameObject, ObjectPool> pools;

        public static ObjectPoolManager Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeObjectPoolManager()
        {
            if (Instance == null)
            {
                Instance = FindFirstObjectByType<ObjectPoolManager>() ?? new GameObject(nameof(ObjectPoolManager)).AddComponent<ObjectPoolManager>();
            }
            //DontDestroyOnLoad(Instance);
        }
        
        private void Awake()
        {
            Instance = this;
            pools = new Dictionary<GameObject, ObjectPool>(ObjectsToPool.Count);
            
            foreach (var poolData in ObjectsToPool)
            {
                CreateNewPool(poolData.PoolObject, poolData.Capacity, poolData.IsExpandable);
            }
        }

        private void OnEnable()
        {
            ObjectPoolEvents.onWillDestroyPool += OnWillDestroyPool;
        }

        private void OnDisable()
        {
            ObjectPoolEvents.onWillDestroyPool -= OnWillDestroyPool;
        }

        private void OnDestroy()
        {
            foreach (var pool in pools.Values)
            {
                pool.RemovePool();
            }
            pools = null;
        }
        
        public ObjectPool CreateNewPool(GameObject poolObject, int capacity = 10, bool isExpandable = true)
        {
            if (poolObject == null)
            {
                throw new NullReferenceException("Attempted to create a pool with a null GameObject.");
            }

            if (!TryGetPool(poolObject, out var pool))
            {
                var container = new GameObject($"Pool of {poolObject.name}s");
                var newPool = container.AddComponent<ObjectPool>();
                newPool.transform.SetParent(transform);
                newPool.Create(poolObject, capacity, isExpandable);
                pools.Add(poolObject, newPool);
                container.transform.localScale = Vector3.one;

                return newPool;
            }

            Debug.LogWarning($"Pool identified by \"{poolObject}\" already exists.");
            return pool;
        }

        public ObjectPool GetPool(GameObject identifier)
        {
            if (TryGetPool(identifier, out var pool))
            {
                return pool;
            }

            Debug.LogWarning($"Pool identified by \"{identifier}\" doesn't exist.");
            return null;
        }

        public bool TryGetPool(GameObject identifier, out ObjectPool pool)
        {
            if (pools.TryGetValue(identifier, out var objectPool))
            {
                pool = objectPool;
                return true;
            }
            pool = null;
            return false;
        }

        public GameObject GetObject(GameObject identifier, bool setActive = false)
        {
            if (TryGetPool(identifier, out var pool))
            {
                return pool.Get(setActive);
            }
            Debug.LogWarning($"Pool identified by \"{identifier}\" doesn't exist.");
            return null;
        }
        
        public T GetObject<T>(GameObject identifier, bool setActive = false) where T : Component 
        {
            if (TryGetPool(identifier, out var pool))
            {
                return pool.Get<T>(setActive);
            }
            Debug.LogWarning($"Pool identified by \"{identifier}\" doesn't exist.");
            return null;
        }

        /// <summary>
        /// Try to obtain a pooled object from the specific pool.<br></br><br></br>
        /// Time Complexity: O(1) on average. O(n) where n is the pool capacity if the pool needs to expand.
        /// </summary>
        /// <param name="identifier">The pool identifier.</param>
        /// <param name="setActive">Whether the game object should be activated.</param>
        /// <param name="obj">The pooled game object.</param>
        /// <returns>True if getting a pooled object is successful. Otherwise false.</returns>
        public bool TryGetObject(GameObject identifier, bool setActive, out GameObject obj)
        {
            obj = GetObject(identifier, setActive);
            return obj != null;
        }

        public void ReturnToPool(GameObject obj)
        {            
            if (obj.GetPool() is not null)
            {
                obj.GetPool().ReturnToPool(obj);                
            }
            else
            {
                Debug.LogWarning($"The game object \"{obj.name}\" is not a pooled object.");
            }
        }

        public List<GameObject> GetMultipleObjects(GameObject identifier, int amount, bool setActive = false)
        {
            if (TryGetPool(identifier, out var pool))
            {
                return pool.GetMultiple(amount, setActive);
            }
            else
            {
                Debug.LogWarning($"Pool identified by \"{identifier}\" doesn't exist.");
                return null;
            }
        }

        public void RemovePool(GameObject identifier)
        {
            if (TryGetPool(identifier, out var pool))
            {
                pool.RemovePool();                                
            }
            else
            {
                Debug.LogWarning($"Pool identified by \"{identifier}\" doesn't exist.");
            }
        }

        private void OnWillDestroyPool(ObjectPool pool)
        {
            pools.Remove(pool.PooledObject);            
        }
    }
}