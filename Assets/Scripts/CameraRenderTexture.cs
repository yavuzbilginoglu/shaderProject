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
            Debug.LogError("Kamera cihazý bulunamadý");
            return;
        }

        // RenderTexture nesnesini olusturma
        renderTexture = new RenderTexture(512, 512, 24);

        // Kameradan gelen verileri WebCamTexture nesnesine atama
        camTexture = new WebCamTexture(devices[0].name, 512, 512, 30);
        camTexture.Play();

        // RenderTexture nesnesini shaderda kullanmak için material olusturma
        Material material = new Material(Shader.Find("Custom/HSLShader"));
        material.SetTexture("_MainTex", camTexture);
        
        GetComponent<Renderer>().material = material;
    }
}
