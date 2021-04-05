using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private eVisualEffectType se_VFXType;

    private void OnDestroy()
    {
        FindObjectOfType<VFXManager>().PlayVFX(se_VFXType, transform.position);
    }
}
