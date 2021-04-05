using System;
using UnityEngine;

public class GameObjectDestroyer : MonoBehaviour
{
    [SerializeField] private float se_timeToDestroy = 2f;
    [SerializeField] private eVisualEffectType se_DestroyVFXType;

    public event Action<eVisualEffectType, Vector3> DestroyVFXAction;

    private void Start()
    {
        if (se_timeToDestroy != 0)
        {
            Destroy(gameObject, se_timeToDestroy);
        }
    }

    private void OnDestroy()
    {
        DestroyVFXAction?.Invoke(se_DestroyVFXType, transform.position);
    }
}
