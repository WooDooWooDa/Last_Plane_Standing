namespace BlackCatPool
{
    public interface IPoolable
    {
        void OnCreated(); //Called when created in the pool, can replace Awake() or Start()
        void OnObtained(); //Called when obtained from the pool
        void OnPooled(); //Called when returned to the pool, can replace OnDisable()
        void OnDestroyed(); //Called when destroyed, can replace OnDestroy()
    }
}

