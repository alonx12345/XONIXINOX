using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    private void Awake()
    {
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize("4046405", false);
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
           
    //    }
    //}

    public void PlayAd()
    {
        if (Advertisement.IsReady("video"))
        {
            Advertisement.Show("video");
        }
    }
}
