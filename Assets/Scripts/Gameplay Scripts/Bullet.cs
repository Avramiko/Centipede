using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 1.5f;

    private bool isReturned;
    public bool IsReturned => isReturned;

    private void Update()
    {
        if (!isReturned)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
    }

    public void Activate()
    {
        StopAllCoroutines();
        isReturned = false;
        StartCoroutine(ReturnToPoolAfterTime());
    }

    public void ReturnToPool()
    {
        if (!isReturned)
        {
            isReturned = true;
            GameplayEvents.RaiseBulletReleased(this);
        }
    }

    private IEnumerator ReturnToPoolAfterTime() // wait for the "lifetime", then return the bullet
    {
        yield return new WaitForSeconds(lifetime);

        if (!isReturned)
        {
            ReturnToPool();
        }
    }
}