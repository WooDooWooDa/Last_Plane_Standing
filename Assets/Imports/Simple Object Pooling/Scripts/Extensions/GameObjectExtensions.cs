using BlackCatPool;
using UnityEngine;

public static class GameObjectExtensions
{
    public static ObjectPool GetPool(this GameObject obj)
    {
        if (obj.TryGetComponent<PooledObject>(out var po))
        {
            return po.Pool;
        }
        return null;
    }

    public static bool TryGetPool(this GameObject obj, out ObjectPool pool)
    {
        if (obj.TryGetComponent<PooledObject>(out var po))
        {
            pool = po.Pool;
            return true;
        }
        pool = null;
        return false;
    }
    
    public static void ReturnToPool(this GameObject obj)
    {
        ObjectPoolManager.Instance.ReturnToPool(obj);
    }
}