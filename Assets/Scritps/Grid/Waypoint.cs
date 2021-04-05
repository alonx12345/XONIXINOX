using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] public Collider2D se_BorderCollider = null;

    public bool IsExploredByBFS { get; set; } = false;
    public bool IsExploredByPlayer { get; set; } = false;
    public bool IsExploredByEnemy { get; set; } = false;
    public bool Colored { get; set; } = false;
    public bool ColoredByPlayer { get; set; } = false;
    public bool Flooded { get; set; } = false;

    public Waypoint ExploredFrom { get; set; }

    private SpriteRenderer m_SpriteRenderer;

    private bool m_Nothing;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        if (se_BorderCollider != null)
        {
            se_BorderCollider.enabled = true;
        }

        gameObject.layer = 9;

        //Physics2D.IgnoreLayerCollision(9, );
    }

    public Color Color
    {
        get => m_SpriteRenderer.color;
        set => m_SpriteRenderer.color = value;
    }

    public void ColorPoint(Color i_FillColor, bool i_ActivateCollider)
    {
        Colored = true;
        Flooded = true;
        m_SpriteRenderer.color = i_FillColor;

        gameObject.layer = 12;

        //if (i_ActivateCollider && se_BorderCollider != null)
        //{
        //    se_BorderCollider.enabled = true;
        //}
    }

    public void ActivateBorderCollider(bool i_Value)
    {
        gameObject.layer = i_Value ? 12 : 9;
    }

    public bool BorderColliderActive
    {
        set
        {
            ActivateBorderCollider(value);
            m_Nothing = value; 
        }
    }

    public void ResetPoint()
    {
        IsExploredByEnemy = false;
        IsExploredByBFS = false;
        Colored = false;
        ColoredByPlayer = false;
        Flooded = false;

        gameObject.layer = 9;

        //if (se_BorderCollider != null)
        //{
        //    se_BorderCollider.enabled = false;
        //}
    }
}
