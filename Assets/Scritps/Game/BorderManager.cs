using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderManager : MonoBehaviour
{
    [SerializeField] private GameObject se_BorderTop = null;
    [SerializeField] private GameObject se_BorderBottom = null;
    [SerializeField] private GameObject se_BorderLeft = null;
    [SerializeField] private GameObject se_BorderRight = null;

    private void Start()
    {
        setBorders();
    }

    private void setBorders()
    {
        if (se_BorderTop != null)
        {
            se_BorderTop.transform.position = new Vector3((int)(Utils.s_GridWidth / 2f), Utils.s_GridHeight);
            se_BorderTop.transform.localScale = new Vector2(Utils.s_GridWidth + 1, se_BorderTop.GetComponent<BoxCollider2D>().size.y);
        }

        if (se_BorderBottom != null)
        {
            se_BorderBottom.transform.position = new Vector3((int)(Utils.s_GridWidth / 2f), -1);
            se_BorderBottom.transform.localScale = new Vector2(Utils.s_GridWidth + 1, se_BorderBottom.GetComponent<BoxCollider2D>().size.y);
        }

        if (se_BorderLeft != null)
        {
            se_BorderLeft.transform.position = new Vector3(-1, (int)(Utils.s_GridHeight / 2f));
            se_BorderLeft.transform.localScale = new Vector2(se_BorderLeft.GetComponent<BoxCollider2D>().size.x, Utils.s_GridHeight + 1);
        }

        if (se_BorderRight != null)
        {
            se_BorderRight.transform.position = new Vector3(Utils.s_GridWidth, (int)(Utils.s_GridHeight / 2f));
            se_BorderRight.transform.localScale = new Vector2(se_BorderRight.GetComponent<BoxCollider2D>().size.x, Utils.s_GridHeight + 1);
        }
    }
}
