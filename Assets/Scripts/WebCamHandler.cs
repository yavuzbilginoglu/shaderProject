using UnityEngine;
using System.Collections;

public class WebCamHandler : MonoBehaviour
{
    public int requestWidth = 480;
    public int requestHeight = 640;
    WebCamTexture webcam;
    //Texture2D texture;

    public bool Ready
    {
        get
        {
            return webcam.isPlaying && webcam.width > 16;
        }
    }

    public void  Start()
    {
        webcam = new WebCamTexture(requestWidth, requestHeight);
        webcam.Play();
        GetComponent<Renderer>().material.mainTexture = webcam;
/*
        while (!Ready)
        {
            yield return null;
        }
*/
        //transform.localScale = new Vector3(1.0f * webcam.width / webcam.height, 1, 1);

       
      
    }

    void Update()
    {
        
    }

    
}