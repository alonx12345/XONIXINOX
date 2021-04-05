using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorsManager : MonoBehaviour
{
    [SerializeField] private Color[] se_Colors = null;

    private void Awake()
    {
        Utils.s_GameColors = se_Colors;
    }

    public Color GenerateColor()
    {
        return se_Colors[Random.Range(0, se_Colors.Length)];
    }
}
