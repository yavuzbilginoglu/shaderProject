using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
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
    private Vector3[] rgbData;
    

    public WebCamTexture webcamTexture;
    public RenderTexture extractColorRenderTexture;
    public RenderTexture erodeRenderTexture;
    public RenderTexture dilateRenderTexture;

    public TextMeshProUGUI hsvText;
    public TextMeshProUGUI rgbText;
    
    public ComputeBuffer dilatebuffer;
    public ComputeBuffer erodebuffer;
    
    private int extractColorKernel;
    private int dilateKernel;
    private int erodeKernel;
    

    private void Start()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
        
        int bufferSize = 640 * 480;
        
        hsvBuffer = new ComputeBuffer(bufferSize, sizeof(float) * 3);
        rgbBuffer = new ComputeBuffer(bufferSize, sizeof(float) * 3);
        dilatebuffer = new ComputeBuffer(bufferSize, sizeof(float));

        extractColorKernel = shader.FindKernel("ExtractColor");
        dilateKernel = shader.FindKernel("Dilate");

        extractColorRenderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        extractColorRenderTexture.enableRandomWrite = true;
        extractColorRenderTexture.Create();

        dilateRenderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        dilateRenderTexture.enableRandomWrite = true;
        dilateRenderTexture.Create();

        GetComponent<Renderer>().material.mainTexture = dilateRenderTexture;

    }
    private void Update()
    {
        hsvData = new HSVData[width * height];
        hsvBuffer.GetData(hsvData);

        rgbData = new Vector3[width * height];
        rgbBuffer.GetData(rgbData);

        if (webcamTexture.isPlaying && webcamTexture.didUpdateThisFrame)
        {
            shader.SetTexture(extractColorKernel, "SourceTex", webcamTexture);
            shader.SetTexture(extractColorKernel, "ExtractTex", extractColorRenderTexture);
            shader.SetBuffer(extractColorKernel, "rgbBuffer", rgbBuffer);
            shader.SetBuffer(extractColorKernel, "hsvBuffer", hsvBuffer);
            shader.Dispatch(extractColorKernel, width / 8, height / 8, 1);

            shader.SetTexture(dilateKernel, "ExtractTex", extractColorRenderTexture);
            shader.SetTexture(dilateKernel, "DilatedTex", dilateRenderTexture);
            shader.SetBuffer(dilateKernel, "dilateBuffer", dilatebuffer);
            shader.Dispatch(dilateKernel, width / 8, height / 8, 1);
           
        }

        //rgbText.text = "R=" + Mathf.RoundToInt(rgbData[153280].x * 255f).ToString("D3") +
        //    ", G=" + Mathf.RoundToInt(rgbData[153280].y * 255f).ToString("D3") +
        //    ", B=" + Mathf.RoundToInt(rgbData[153280].z * 255f).ToString("D3");

        //hsvText.text = "H=" + Mathf.RoundToInt(hsvData[153280].h * 360f).ToString("D3") +
        //               ", S=" + Mathf.RoundToInt(hsvData[153280].s * 100f).ToString("D3") +
        //               ", V=" + Mathf.RoundToInt(hsvData[153280].v * 100f).ToString("D3") + " FPS: " + (int)(1f / Time.deltaTime);
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

        if (dilatebuffer != null)
        {
            dilatebuffer.Release();
            dilatebuffer = null;
        }
    }
}
