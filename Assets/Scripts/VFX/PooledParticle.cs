using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

// Attached automatically to pooled VFX instances (impact/blood effects) so they
// return themselves to their pool after a delay instead of being Destroyed,
// avoiding per-shot Instantiate/Destroy allocations and GC spikes.
public class PooledParticle : MonoBehaviour
{
    private IObjectPool<GameObject> _pool;
    private Coroutine _returnRoutine;

    public void Initialize(IObjectPool<GameObject> pool)
    {
        _pool = pool;
    }

    public void ScheduleReturn(float delay)
    {
        if (_returnRoutine != null) StopCoroutine(_returnRoutine);
        _returnRoutine = StartCoroutine(ReturnAfterDelay(delay));
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _returnRoutine = null;

        if (_pool != null)
        {
            _pool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
