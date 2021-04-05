using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyerEnemy : MonoBehaviour
{
    [SerializeField] private Transform se_TileExplosionFX = null;
    public event Action<eSoundID> PlaySoundAction;

    private void OnCollisionEnter2D(Collision2D i_Other)
    {
        Waypoint pointHit = i_Other.gameObject.GetComponent<Waypoint>();

        if (pointHit != null && !Utils.OnBorder(pointHit.transform.position.x , pointHit.transform.position.y) && !pointHit.ColoredByPlayer)
        {
            if (se_TileExplosionFX != null)
            {
                var settings = se_TileExplosionFX.GetComponent<ParticleSystem>().main;
                Color FXColor = pointHit.Color;
                FXColor.a = 1f;
                settings.startColor = new ParticleSystem.MinMaxGradient(FXColor);
                Instantiate(se_TileExplosionFX, transform.position, Quaternion.identity);
                PlaySoundAction?.Invoke(eSoundID.PercentFilledDecreaseSound);
            }

            FindObjectOfType<GridManager>().ExplodeAroundPoint(Mathf.RoundToInt(i_Other.transform.position.x), Mathf.RoundToInt(i_Other.transform.position.y));
        }
    }
}
