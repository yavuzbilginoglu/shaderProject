using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public Material cameraMaterial;
    private WebCamTexture camTexture;
    public Shader shader;
    public TextMeshProUGUI resolutionText;

    void Start()
    {
        //camTexture = new WebCamTexture();
        WebCamDevice[] devices = WebCamTexture.devices;
        //camTexture = new WebCamTexture(devices[0].name, 480, 640);

        if (devices.Length == 0)
        {
            Debug.LogError("kamera bulunamadi...");
            return;
        }
        //resolutionText.text = "Resolution: " + camTexture.width + "x" + camTexture.height;

        //camTexture = new WebCamTexture(devices[0].name, devices[0].availableResolutions[Mathf.Min(devices[0].availableResolutions.Length - 1, 0)].width, devices[0].availableResolutions[Mathf.Min(devices[0].availableResolutions.Length - 1, 0)].height);
        GetComponent<Renderer>().material.mainTexture = camTexture;
        
        camTexture.Play();

        //Debug.Log("Camera Resolution: " + camTexture.width + "x" + camTexture.height);


        cameraMaterial = new Material(shader);
        cameraMaterial.SetTexture("_MainTex", camTexture);


        // hsl degerleri
        cameraMaterial.SetFloat("_Hue", 0.5f);
        cameraMaterial.SetFloat("_Saturation", 0.5f);
        cameraMaterial.SetFloat("_Lightness", 0.5f);


        //Camera.main.targetTexture = (RenderTexture)cameraMaterial.mainTexture;

    }
}
