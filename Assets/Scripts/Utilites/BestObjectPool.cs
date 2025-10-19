using UnityEngine;
using UnityEngine.Pool;

public class BestObjectPool<T> where T : MonoBehaviour
{
    private readonly ObjectPool<T> pool;

    public BestObjectPool(T prefab, int defaultPoolSize = 10, int maxSize = 128)
    {
        pool = new ObjectPool<T>(
            createFunc: () => Object.Instantiate(prefab),
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Object.Destroy(obj.gameObject),
            defaultCapacity: defaultPoolSize,
            maxSize: maxSize
        );

        var initialObjects = new T[defaultPoolSize];
        for (var i = 0; i < defaultPoolSize; i++)
        {
            initialObjects[i] = pool.Get();
        }

        for (var i = 0; i < defaultPoolSize; i++)
        {
            pool.Release(initialObjects[i]);
        }
    }

    public T GetObject() => pool.Get();

    public void ReleaseObject(T obj) => pool.Release(obj);
}