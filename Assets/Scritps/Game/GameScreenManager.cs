using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenManager : MonoBehaviour
{
    [SerializeField] private float se_CameraSizeFactor = 0.887f;
    [SerializeField] private float se_CameraXPosFactor = 0.5f;
    [SerializeField] private float se_CameraYPosFactor = 0.645f;

    public void Start()
    {
        float screenAspectRatio = (1f * Screen.height) / Screen.width;

        if (Screen.width > Screen.height)
        {
            screenAspectRatio = (1f * Screen.width) / Screen.height;
        }
        
        float CameraYPos = 20f;

        if (screenAspectRatio >= 1.65f && screenAspectRatio <= 1.79f)
        {
            Camera.main.orthographicSize = 27.9f;
            CameraYPos = Utils.s_GridHeight * se_CameraYPosFactor;
        }
        else if (screenAspectRatio < 1.65f && screenAspectRatio >= 1.55f)
        {
            Camera.main.orthographicSize = 25.17f;
            CameraYPos = 24.5f;
        }
        else if (screenAspectRatio >= 2.18f && screenAspectRatio <= 2.3f)
        {
            Camera.main.orthographicSize = 35f;
            CameraYPos = 21.5f;
        }
        else if (screenAspectRatio >= 2.15f && screenAspectRatio < 2.18f)
        {
            Camera.main.orthographicSize = 34f;
            CameraYPos = 21.5f;
        }
        else if (screenAspectRatio > 1.79f && screenAspectRatio <= 1.81f)
        {
            Camera.main.orthographicSize = 32.4f;
            CameraYPos = 21.5f;
        }
        else if (screenAspectRatio > 1.98f && screenAspectRatio <= 2.02f)
        {
            Camera.main.orthographicSize = 31.3f;
            CameraYPos = 22f;
        }
        else if (screenAspectRatio > 1.3f && screenAspectRatio <= 1.4f)
        {
            Camera.main.orthographicSize = 25.8f;
            CameraYPos = 27f;
        }
        else if (screenAspectRatio > 2.02f && screenAspectRatio <= 2.07f)
        {
            Camera.main.orthographicSize = 32.3f;
           
            CameraYPos = 21f;
            
        }

        float CameraXPos = (int)(Utils.s_GridWidth * se_CameraXPosFactor);
        Camera.main.transform.position = new Vector3(CameraXPos, CameraYPos, -10);

    }

   
}
