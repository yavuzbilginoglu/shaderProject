using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ComputeShaderExample : MonoBehaviour
{

    struct HSVData
    {
        public float h;
        public float s;
        public float v;
    };
    
    public ComputeShader shader;
    private int width = 640;
    private int height = 480;

    public ComputeBuffer hsvBuffer;
    public ComputeBuffer rgbBuffer;
    private HSVData[] hsvData;

    private WebCamTexture webcamTexture;
    public Texture2D inputTex;
    public RenderTexture renderTextureKernel1;
    public RenderTexture renderTextureKernel2;
    public TextMeshProUGUI hsvText;
    public TextMeshProUGUI rgbText;
    private int kernel1;
    private Vector3[] rgbData;
    private void Start()
    {
        webcamTexture = new WebCamTexture();
        //rgbText.text = "isdatargb: " + webcamTexture.isDataSRGB.ToString();
        webcamTexture.Play();


        
        hsvBuffer = new ComputeBuffer(width * height, sizeof(float) * 3);
        rgbBuffer = new ComputeBuffer(width * height, sizeof(float) * 3);
        
        kernel1 = shader.FindKernel("testHsv");
        

        renderTextureKernel1 = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        renderTextureKernel1.enableRandomWrite = true;
        renderTextureKernel1.Create();
        
        
        GetComponent<Renderer>().material.mainTexture = renderTextureKernel1;
    }
    private void Update()
    {
        hsvData = new HSVData[width * height];
        hsvBuffer.GetData(hsvData);

        rgbData = new Vector3[width * height];
        rgbBuffer.GetData(rgbData);
        
        if (webcamTexture.isPlaying && webcamTexture.didUpdateThisFrame)
        {
            shader.SetTexture(kernel1, "SourceTex", webcamTexture);
            shader.SetTexture(kernel1, "_ResultTex", renderTextureKernel1);
            shader.SetBuffer(kernel1, "hsvBuffer", hsvBuffer);
            shader.SetBuffer(kernel1, "rgbBuffer", rgbBuffer);
            shader.Dispatch(kernel1, width / 8, height / 8, 1);
        }

        rgbText.text = "R=" + Mathf.RoundToInt(rgbData[153280].x * 255f).ToString("D3") +
            ", G=" + Mathf.RoundToInt(rgbData[153280].y * 255f).ToString("D3") +
            ", B=" + Mathf.RoundToInt(rgbData[153280].z * 255f).ToString("D3");

        hsvText.text = "H=" + Mathf.RoundToInt(hsvData[153280].h * 360f).ToString("D3") +
                       ", S=" + Mathf.RoundToInt(hsvData[153280].s * 100f).ToString("D3") +
                       ", V=" + Mathf.RoundToInt(hsvData[153280].v * 100f).ToString("D3");
    }

    void OnDestroy()
    {
        if (hsvBuffer != null)
        {
            hsvBuffer.Release();
            hsvBuffer = null;
        }

        if (rgbBuffer != null)
        {
            rgbBuffer.Release();
            rgbBuffer = null;
        }
    }
}
