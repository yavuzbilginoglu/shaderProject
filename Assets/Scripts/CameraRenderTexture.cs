using UnityEngine;

public class CameraRenderTexture : MonoBehaviour
{
    private WebCamTexture camTexture;
    private RenderTexture renderTexture;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("Kamera cihazi bulunamadi");
            return;
        }

        // RenderTexture nesnesini olusturma
        renderTexture = new RenderTexture(640, 480, 24);

        // Kameradan gelen verileri WebCamTexture nesnesine atama
        camTexture = new WebCamTexture(devices[0].name, 640, 480);
        camTexture.Play();

        // RenderTexture nesnesini shaderda kullanmak icin material olusturma
        Material material = new Material(Shader.Find("Custom/HueSaturation"));
        material.SetTexture("_MainTex", camTexture);
        
        GetComponent<Renderer>().material = material;
    }
}
