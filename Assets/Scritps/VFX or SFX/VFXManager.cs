using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public enum eVisualEffectType
{
    EnemyExplosion,
    PlayerExplosion,
    PlayerProjectileExplosion,
    PlayPathExplode,
    EnemyProjectileExplosion,
    TileExplosion,
    FillEffect,
    PowerupExplosion,
    LevelComplete,
    EnemyBigExplosionVFX
}

public class VFXManager : MonoBehaviour
{
    [System.Serializable]
    public class VisualEffect
    {
        [SerializeField] public eVisualEffectType se_VFXType;
        [SerializeField] public GameObject se_VFXPrefab = null;

        public void PlayVFX(Vector3 i_Position)
        {
            if (se_VFXPrefab != null)
            {
                Instantiate(se_VFXPrefab, i_Position, Quaternion.identity);
            }
        }

        public void PlayVFXWithColor(Vector3 i_Position, Color i_Color)
        {
            var settings = se_VFXPrefab.GetComponent<ParticleSystem>().main;
            Color FXColor = i_Color;
            FXColor.a = 1f;
            settings.startColor = new ParticleSystem.MinMaxGradient(FXColor);
            Instantiate(se_VFXPrefab, i_Position, Quaternion.identity);
        }
    }

    [SerializeField] private VisualEffect[] se_VFXEffects = null;

    public void PlayVFX(eVisualEffectType i_EffectType, Vector3 i_Position)
    {
        foreach (var effect in se_VFXEffects.ToList().Where(effect => effect.se_VFXType == i_EffectType))
        {
            effect.PlayVFX(i_Position);
        }
    }

    public void PlayVFXWithColor(eVisualEffectType i_EffectType, Vector3 i_Position, Color i_Color)
    {
        foreach (var effect in se_VFXEffects.ToList().Where(effect => effect.se_VFXType == i_EffectType))
        {
            effect.PlayVFXWithColor(i_Position, i_Color);
        }
    }
}
