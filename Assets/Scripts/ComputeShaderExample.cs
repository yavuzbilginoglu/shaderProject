using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class ComputeShaderExample : MonoBehaviour
{
    
    public ComputeShader shader;
    private readonly int width = 480;
    private readonly int height = 640;
    public int rectWH = 256;
    public int groupSize;
    

    public WebCamTexture webcamTexture;
    public RenderTexture extractColorRenderTexture;
    public RenderTexture dilateAndErodeRenderTexture;
    public RenderTexture colorbuffertex;
    public RenderTexture dilatebuffertex;
    public RenderTexture hedefbuffertex;
    public Material test;
    public Stopwatch stopwatch;
    
    public Text fpsText;
    
    private int extractColorKernel;
    private int showKernel;
    private int dilateKernel1;
    private int erodeKernel1;
    private int dilateKernel2;
    private int erodeKernel2;
    private float cagrilmaSayisi = 0;
    private float toplamSure = 0;

    public Text toleranceValueText;
    public Text hedefHueText;
    public Text lowerSaturationText;
    public Text upperSaturationText;
    public Text lowerValueText;
    public Text upperValueText;
    public Text erodeText;
    public Text dilateText;

    private float tolerance;
    private float hue;
    private float lowerhue;
    private float upperhue;
    private float lowersaturation;
    private float uppersaturation;
    private float lowervalue;
    private float uppervalue;
    private float erodesize; 
    private float dilatesize;
    public bool isDilateButtonActive = false;




    private float[] araliklar = { 0.0f, 0.0f, 0.0f, 0.0f };

    private Vector2Int CalculateRectXY()
    {
        Vector2Int rectXY = new()
        {
            x = (width / 2 - rectWH / 2),
            y = (height / 2 - rectWH / 2)
        };
        return rectXY;
    }

    private void Start()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();

        groupSize = rectWH / 8;

        colorbuffertex = new RenderTexture(rectWH, rectWH, 0, RenderTextureFormat.RInt);
        colorbuffertex.enableRandomWrite = true;
        colorbuffertex.Create();

        dilatebuffertex = new RenderTexture(rectWH, rectWH, 0,RenderTextureFormat.RInt);
        dilatebuffertex.enableRandomWrite = true;
        dilatebuffertex.Create();

        hedefbuffertex = new RenderTexture(rectWH, rectWH, 0, RenderTextureFormat.RInt);
        hedefbuffertex.enableRandomWrite = true;
        hedefbuffertex.Create();

        extractColorKernel = shader.FindKernel("ExtractColorRedTeam");
        showKernel = shader.FindKernel("showKernel");
        erodeKernel1 = shader.FindKernel("erodeKernel");
        dilateKernel2 = shader.FindKernel("dilateKernel2");
        erodeKernel2 = shader.FindKernel("erodeKernel2");
        dilateKernel1 = shader.FindKernel("dilateKernel");



        extractColorRenderTexture = new RenderTexture(rectWH, rectWH, 0, RenderTextureFormat.ARGBFloat);
        extractColorRenderTexture.enableRandomWrite = true;
        extractColorRenderTexture.Create();

        dilateAndErodeRenderTexture = new RenderTexture(rectWH, rectWH, 0, RenderTextureFormat.ARGBFloat);
        dilateAndErodeRenderTexture.enableRandomWrite = true;
        dilateAndErodeRenderTexture.Create();


        GetComponent<Renderer>().material.mainTexture = dilateAndErodeRenderTexture;
        getParams();
        initilizeShader();
        stopwatch = new Stopwatch();

    }
    private void Update()
    {
        if(webcamTexture.isPlaying && webcamTexture.didUpdateThisFrame)
        {
            if (isDilateButtonActive)
            {
                stopwatch.Start();
                cagrilmaSayisi++;
                shader.Dispatch(extractColorKernel, groupSize, groupSize, 1);
                shader.Dispatch(dilateKernel1, groupSize, groupSize, 1);
                shader.Dispatch(dilateKernel2, groupSize, groupSize, 1);
                shader.Dispatch(erodeKernel1, groupSize, groupSize, 1);
                shader.Dispatch(erodeKernel2, groupSize, groupSize, 1);
                stopwatch.Stop();
                fpsText.text = "" + 1000 * (float)stopwatch.ElapsedMilliseconds / cagrilmaSayisi;
            }
            shader.SetTexture(showKernel, "dilateBuffer", dilatebuffertex);
            shader.Dispatch(showKernel, groupSize, groupSize, 1);
            //fpsText.text = "FPS: " + (int)(1f / Time.deltaTime);
        }
    }

    public void buttonOnClick()
    {
        isDilateButtonActive = !isDilateButtonActive;
       
    }

    private void initializeDilateKernel()
    {
        shader.SetTexture(dilateKernel1, "dilateBuffer", dilatebuffertex);
        shader.SetTexture(dilateKernel1, "hedefBuffer", hedefbuffertex);

        shader.SetTexture(dilateKernel2, "dilateBuffer2", hedefbuffertex);
        shader.SetTexture(dilateKernel2, "hedefBuffer2", dilatebuffertex);
    }

    private void initializeErodeKernel()
    {
        shader.SetTexture(erodeKernel1, "dilateBuffer", dilatebuffertex);
        shader.SetTexture(erodeKernel1, "hedefBuffer", hedefbuffertex);

        shader.SetTexture(erodeKernel2, "dilateBuffer2", hedefbuffertex);
        shader.SetTexture(erodeKernel2, "hedefBuffer2", dilatebuffertex);
    }

    private void initilizeShader()
    {
        initializeParams();
        shader.SetTexture(extractColorKernel, "SourceTex", test.mainTexture);
        shader.SetTexture(extractColorKernel, "colorBuffer", colorbuffertex);
        shader.SetTexture(extractColorKernel, "dilateBuffer", dilatebuffertex);
        initializeDilateKernel();
        initializeErodeKernel();
        shader.SetTexture(showKernel, "ExtractTex", dilateAndErodeRenderTexture);    
    }
    private void getParams()
    {
        uppersaturation = 100;
        uppervalue = 100;
        lowersaturation = 20;
        lowervalue = 20;
        hue = 0;
        tolerance = 15;
        dilatesize = 0;
        erodesize = 0;
    }
    

    private void initializeParams()
    {
        HueCalculate();
        shader.SetFloat("lowerSaturation", lowersaturation / 100.0f);
        shader.SetFloat("upperSaturation", uppersaturation / 100.0f);
        shader.SetFloat("lowerValue", lowervalue / 100.0f);
        shader.SetFloat("upperValue", uppervalue / 100.0f);
        shader.SetFloat("lowerHue1", araliklar[0]);
        shader.SetFloat("upperHue1", araliklar[1]);
        shader.SetFloat("lowerHue2", araliklar[2]);
        shader.SetFloat("upperHue2", araliklar[3]);
        shader.SetFloat("erodeSize", erodesize);
        shader.SetFloat("dilateSize", dilatesize);

        Vector2Int rectXY = CalculateRectXY();
        shader.SetInts("RectXY", rectXY.x, rectXY.y); 
    }
    private void HueCalculate()
    {
        float altLimit = hue - tolerance;
        float ustLimit = hue + tolerance;

        if (altLimit >= 0 && ustLimit <= 360)
        {
            
            araliklar[0] = altLimit / 360.0f;
            araliklar[1] = hue / 360.0f;
            araliklar[2] = hue / 360.0f;
            araliklar[3] = ustLimit / 360.0f;
        }
        else if (altLimit < 0 && ustLimit > 360)
        {
            
            araliklar[0] = 0.0f;
            araliklar[1] = 1.0f;
            araliklar[2] = 0.0f;
            araliklar[3] = 1.0f;
        }
        else if (altLimit < 0)
        {
            
            araliklar[0] = 0.0f;
            araliklar[1] = ustLimit / 360.0f;
            araliklar[2] = (360 + altLimit) / 360.0f;
            araliklar[3] = 1.0f;
        }
        else
        {
            
            araliklar[0] = altLimit / 360.0f;
            araliklar[1] = 1.0f;
            araliklar[2] = 0.0f;
            araliklar[3] = (ustLimit - 360) / 360.0f;
        }

    }
    public void OnToleranceValueChanged(Slider slider)
    {
        tolerance = slider.value;
        toleranceValueText.text = slider.value.ToString();
        initializeParams();
    }

    public void OnHueValueChanged(Slider slider)
    {
        hue = slider.value;
        hedefHueText.text = slider.value.ToString();
        initializeParams();
    }

    public void OnLowerSaturationValueChanged(Slider slider)
    {
        lowersaturation = slider.value;
        lowerSaturationText.text = slider.value.ToString();
        initializeParams();
    }

    public void OnUpperSaturationValueChanged(Slider slider)
    {
        uppersaturation = slider.value;
        upperSaturationText.text = slider.value.ToString();
        initializeParams();
    }

    public void OnLowerValueValueChanged(Slider slider)
    {
        lowervalue = slider.value;
        lowerValueText.text = slider.value.ToString();
        initializeParams();
    }

    public void OnUpperValueValueChanged(Slider slider)
    {
        uppervalue = slider.value;
        upperValueText.text = slider.value.ToString();
        initializeParams();
    }

    public void OnErodeValueChanged(Slider slider)
    {
        erodesize = slider.value;
        erodeText.text = slider.value.ToString();
        initializeParams();
    }

    public void OnDilateValueChanged(Slider slider)
    {
        dilatesize = slider.value;
        dilateText.text = slider.value.ToString();
        initializeParams();
    }

    private void OnDestroy()
    {
        if (colorbuffertex != null)
        {
            colorbuffertex.Release();
            colorbuffertex = null;
        }

        if (dilatebuffertex != null)
        {
            dilatebuffertex.Release();
            dilatebuffertex = null;
        }

        if (hedefbuffertex != null)
        {
            hedefbuffertex.Release();
            hedefbuffertex = null;
        }

    }
}