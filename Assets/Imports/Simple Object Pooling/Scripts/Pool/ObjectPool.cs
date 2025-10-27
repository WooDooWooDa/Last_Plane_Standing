using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlackCatPool
{
    public class ObjectPool : MonoBehaviour
    {
        public GameObject PooledObject { get; private set; }
        public int Capacity { get; private set; }
        public bool IsExpandable { get; private set; } = true;
        public bool PersistBetweenScenes { get; private set; }
        public int ActiveCount => Capacity - pool.Count;
        public int PooledCount => pool.Count - nullInPool;
        public int MissingCount => destroyedObject;
        public bool IsEmpty => pool.Count == 0;

#if UNITY_EDITOR
        public float LastObtainedTime { get; private set; } = -1;
        public float LastReturnedTime { get; private set; } = -1;

        private bool isReturningParent = false;
        private Queue<Transform> toBeReturned;
#endif

        private int destroyedObject = 0;
        private int nullInPool = 0;
        private bool initialised = false;
        private bool isRemoved = false;
        private Queue<PooledObject> pool;
        private Dictionary<GameObject, PooledObject> pooledObjectCache;
        private Dictionary<GameObject, IPoolable> interfaceCache;

        private void OnDisable()
        {
            if (!isRemoved)
            {
                foreach (var i in pooledObjectCache.Keys.ToList())
                {
                    ReturnToPool(i);
                }
            }
        }

        private void OnDestroy()
        {
            RemovePool();

            //just in case
            pool?.Clear();
            pool = null;
#if UNITY_EDITOR
            toBeReturned?.Clear();
            toBeReturned = null;
#endif
            pooledObjectCache?.Clear();
            pooledObjectCache = null;
            interfaceCache?.Clear();
            interfaceCache = null;
        }

        public void Create(GameObject pooledObject, int capacity, bool isExpandable)
        {
            PooledObject = pooledObject;
            Capacity = capacity;
            IsExpandable = isExpandable;            
            pool = new Queue<PooledObject>(capacity);
#if UNITY_EDITOR
            toBeReturned = new Queue<Transform>(capacity);
#endif
            pooledObjectCache = new Dictionary<GameObject, PooledObject>(capacity);
            interfaceCache = new Dictionary<GameObject, IPoolable>(capacity);

            if (Capacity <= 0)
            {
                Capacity = 10;
            }

            ObjectPoolEvents.EventInvoker.OnPoolCreated(this);

            for (int i = 0; i < Capacity; i++)
            {
                CreatePoolObject();
            }

            initialised = true;
        }

        /// <summary>
        /// Obtain a pooled object from this pool. Returns null if the pool is empty and it is unexpandable.<br></br><br></br>
        /// Time Complexity: O(1) on average. O(n) where n is the pool capacity if the pool needs to expand.
        /// </summary>        
        /// <param name="setActive">Whether the game object should be activated.</param>
        /// <returns>The pooled game object.</returns>
        public GameObject Get(bool setActive = false)
        {
            while (!IsEmpty && pool.Peek() == null)
            {
                nullInPool--;
                pool.Dequeue();                
            }

            for (int i = 0; i < destroyedObject; i++)
            {
                CreatePoolObject();
            }
            destroyedObject = 0;

            if (IsEmpty && IsExpandable)
            {
                for (int i = 0; i < Capacity; i++)
                {                    
                    CreatePoolObject();
                }
                Capacity *= 2;

                ObjectPoolEvents.EventInvoker.OnPoolExpanded(this);
            }

            GameObject obj = pool.TryDequeue(out var dequeued) ? dequeued.gameObject : null;
            if (obj != null)
            {
                if (pooledObjectCache.TryGetValue(obj, out var po))
                {
                    pooledObjectCache[obj].isPooled = false;
#if UNITY_EDITOR
                    if (ObjectPoolManager.Instance.ShouldOrganiseHierarchy)
                    {
                        obj.transform.SetParent(null, false);
                    }

                    LastObtainedTime = 0;
#endif
                    obj.SetActive(setActive);
                }
                else
                {
                    throw new System.Exception($"For some reason, the game object \"{obj.name}\" does not belong to this pool.");
                }

                if (interfaceCache.TryGetValue(obj, out var inter))
                {
                    inter.OnObtained();
                }

                ObjectPoolEvents.EventInvoker.OnObjectObtained(obj, this);
            }

            return obj;
        }


        /// <summary>
        /// Obtain a specific component attached to the pooled object from this pool. Returns null if the pool is empty and unexpandable, or the pooled object doesn't have the specified component attached.<br></br><br></br>
        /// Time Complexity: O(1) on average. O(n) where n is the pool capacity if the pool needs to expand.
        /// </summary>
        /// <typeparam name="T">The type of component to be obtained.</typeparam>
        /// <param name="setActive">Whether the game object should be activated.</param>
        /// <returns>The specified component attached to the game object.</returns>
        /// <exception cref="System.Exception"></exception>
        public T Get<T>(bool setActive = false) where T : Component
        {            
            if (TryGet(setActive, out GameObject go))
            {
                if (go.TryGetComponent<T>(out var component))
                {
                    return component;
                }
                else
                {
                    throw new System.Exception($"Game object \"{go.name}\" does not have a component of {typeof(T).Name} attached.");
                }
            }
            return null;
        }

        /// <summary>
        /// Try to obtain a pooled object from this pool.<br></br><br></br>
        /// Time Complexity: O(1) on average. O(n) where n is the pool capacity if the pool needs to expand.
        /// </summary>
        /// <param name="setActive">Whether the game object should be activated.</param>
        /// <param name="gameObject">The pooled game object.</param>
        /// <returns>True if getting a pooled object is successful. Otherwise false.</returns>
        public bool TryGet(bool setActive, out GameObject gameObject)
        {
            gameObject = Get(setActive);
            return gameObject != null;
        }

        /// <summary>
        /// Attempts to obtain a specific component attached to the pooled object.<br></br><br></br>
        /// Time Complexity: O(1) on average. O(n) where n is the pool capacity if the pool needs to expand.
        /// </summary>
        /// <typeparam name="T">The type of component to obtain.</typeparam>
        /// <param name="setActive">Whether the game object should be activated.</param>
        /// <param name="component">The component if found, otherwise null.</param>
        /// <returns>Whether the component is found on the game object.</returns>
        public bool TryGet<T>(bool setActive, out T component) where T : Component
        {            
            component = Get<T>(setActive);
            return component != null;
        }

        /// <summary>
        /// Return a game object to this pool. <br></br><br></br>
        /// Time Complexity: O(1) on average. Might become O(n) where n is the number of component attached to the game object if the game object doesn't belong to this pool.
        /// </summary>
        /// <param name="obj">The game object to be pooled.</param>
        /// <exception cref="System.NullReferenceException"></exception>
        /// <exception cref="System.Exception"></exception>
        public void ReturnToPool(GameObject obj)
        {
            if (obj == null)
            {
                throw new System.NullReferenceException("Attempting to pool a null game object.");
            }
            if (!pooledObjectCache.TryGetValue(obj, out var cache))
            {
                if (obj.TryGetPool(out var pool))
                {
                    if (pool != null)
                    {
                        Debug.LogWarning($"Game object \"{obj.name}\" doesn't belong to the {gameObject.name}. It is automatically returned to the correct one.");
                        pool.ReturnToPool(obj);
                    }
                    else
                    {
                        throw new System.NullReferenceException($"Pool of {obj.name} doesn't exist.");
                    }
                }
                else
                {
                    throw new System.Exception($"Game object \"{obj.name}\" is not a pooled object.");
                }
                return;
            }

            var po = cache;
            if (po.isPooled)
            {
                return;
            }

            po.isPooled = true;
            if (obj.activeSelf)
            {
                obj.SetActive(false);
            }
            pool.Enqueue(po);

#if UNITY_EDITOR
            if (ObjectPoolManager.Instance.ShouldOrganiseHierarchy)
            {
                toBeReturned.Enqueue(obj.transform);
                if (!isReturningParent)
                {
                    isReturningParent = true;
                    StartCoroutine(ReturnParent());
                }
            }

            if (initialised)
            {
                LastReturnedTime = 0;
            }
#endif

            if (interfaceCache.TryGetValue(obj, out var inter))
            {
                inter.OnPooled();
            }

            ObjectPoolEvents.EventInvoker.OnObjectPooled(obj, this);
        }

#if UNITY_EDITOR
        private IEnumerator ReturnParent()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            while (toBeReturned.Count > 0)
            {                
                var obj = toBeReturned.Dequeue();
                if (!obj.gameObject.activeSelf)
                {
                    obj.SetParent(transform, true);
                }
            }
            isReturningParent = false;
        }
#endif

        /// <summary>
        /// Obtain multiple pooled objects from this pool. Only returns the remaining pooled objects if the amount is more than the number of pooled objects in this pool and the pool is unexpandable.<br></br><br></br>
        /// Time Complexity: O(m) where m is the number of objects to be obtained. O(m + n) where n is the pool capacity if the pool needs to expand.
        /// </summary>
        /// <param name="amount">The number of objects to be obtained.</param>        
        /// <param name="setActive">Whether the game object should be activated.</param>
        /// <returns>The list containing the pooled objects.</returns>
        public List<GameObject> GetMultiple(int amount, bool setActive = false)
        {
            var list = new List<GameObject>(amount);
            if (amount > pool.Count && !IsExpandable)
            {
                amount = pool.Count;
            }

            for (int i = 0; i < amount; i++)
            {
                list.Add(Get(setActive));
            }
            return list;
        }

        /// <summary>
        /// Obtain multiple components on multiple pooled objects from this pool. Only returns the remaining components if the amount is more than the number of pooled objects in this pool and the pool is unexpandable.<br></br><br></br>
        /// Time Complexity: O(m) where m is the number of components to obtain. O(m + n) where n is the pool capacity if the pool needs to expand.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="amount"></param>
        /// <param name="setActive"></param>
        /// <returns></returns>
        public List<T> GetMultiple<T>(int amount, bool setActive = false) where T : Component
        {
            var list = new List<T>(amount);
            if (amount > pool.Count && !IsExpandable)
            {
                amount = pool.Count;
            }
            for (int i = 0; i < amount; i++)
            {
                if (TryGet<T>(setActive, out var component))
                {
                    list.Add(component);
                }
            }
            return list;
        }

        /// <summary>
        /// Remove this pool.<br></br><br></br>
        /// Time Complexity: O(n) where n is the pool capacity.
        /// </summary>
        public void RemovePool()
        {
            if (!isRemoved)
            {                
                isRemoved = true;
                foreach (var po in pooledObjectCache.Values.ToList())
                {
                    Destroy(po.gameObject);
                }
                while (pool.TryDequeue(out var po))
                {                    
                    if (po != null)
                    {                        
                        Destroy(po.gameObject);
                    }                    
                }         
                Destroy(gameObject);                

                ObjectPoolEvents.EventInvoker.OnWillDestroyPool(this);
            }            
        }

        private void CreatePoolObject()
        {            
            GameObject obj = Instantiate(PooledObject);
            var po = obj.TryGetComponent<PooledObject>(out var pooledObject) ? pooledObject : obj.AddComponent<PooledObject>();
            po.Initialise(this);
            pooledObjectCache.Add(obj, po);
            ReturnToPool(obj);

            if (PersistBetweenScenes)
            {
                DontDestroyOnLoad(obj);
            }

            if (obj.TryGetComponent<IPoolable>(out var inter))
            {
                interfaceCache.Add(obj, inter);
                inter.OnCreated();
            }

            ObjectPoolEvents.EventInvoker.OnPooledObjectCreated(obj, this);
        }

        public void PooledObjectDestroyed(GameObject obj)
        {
            if (interfaceCache != null && interfaceCache.TryGetValue(obj, out var inter))
            {
                inter.OnDestroyed();
            }

            if (!isRemoved)
            {
                destroyedObject++;
                nullInPool++;

                pooledObjectCache.Remove(obj);
                interfaceCache.Remove(obj);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (LastObtainedTime != -1)
            {
                LastObtainedTime += Time.deltaTime;
            }
            if (LastReturnedTime != -1)
            {
                LastReturnedTime += Time.deltaTime;
            }
        }
#endif
    }
}