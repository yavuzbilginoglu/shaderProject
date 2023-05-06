using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public Slider toleranceSlider;
    public Slider hedefHueSlider;
    public Slider lowerSaturationSlider;
    public Slider upperSaturationSlider;
    public Slider lowerValueSlider;
    public Slider upperValueSlider;
    public Slider erodeSlider;
    public Slider dilateSlider;

    public TextMeshProUGUI toleranceValueText;
    public TextMeshProUGUI hedefHueText;
    public TextMeshProUGUI lowerSaturationText;
    public TextMeshProUGUI upperSaturationText;
    public TextMeshProUGUI lowerValueText;
    public TextMeshProUGUI upperValueText;
    public TextMeshProUGUI erodeText;
    public TextMeshProUGUI dilateText;

    private float tolerance;
    private float hedefhue;
    private float lowerhue;
    private float upperhue;
    private float lowersaturation;
    private float uppersaturation;
    private float lowervalue;
    private float uppervalue;
    private float erodesize; //1-10
    private float dilatesize; //1-10


    private void Start()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
        
        int bufferSize = 640 * 480;
        
        hsvBuffer = new ComputeBuffer(bufferSize, sizeof(float) * 3);
        rgbBuffer = new ComputeBuffer(bufferSize, sizeof(float) * 3);
        dilatebuffer = new ComputeBuffer(bufferSize, sizeof(float));

        hsvData = new HSVData[width * height];
        hsvBuffer.GetData(hsvData);

        rgbData = new Vector3[width * height];
        rgbBuffer.GetData(rgbData);


        extractColorKernel = shader.FindKernel("ExtractColor");
        dilateKernel = shader.FindKernel("Dilate");

        extractColorRenderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        extractColorRenderTexture.enableRandomWrite = true;
        extractColorRenderTexture.Create();

        dilateRenderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        dilateRenderTexture.enableRandomWrite = true;
        dilateRenderTexture.Create();

        GetComponent<Renderer>().material.mainTexture = dilateRenderTexture;

        tolerance = toleranceSlider.value;
        hedefhue = hedefHueSlider.value;
        lowersaturation = lowerSaturationSlider.value;
        uppersaturation = upperSaturationSlider.value;
        lowervalue = lowerValueSlider.value;
        uppervalue = upperValueSlider.value;
        erodesize = erodeSlider.value;
        dilatesize = dilateSlider.value;
    }
    private void Update()
    {
        if (webcamTexture.isPlaying && webcamTexture.didUpdateThisFrame)
        {
            float[] ranges = { 0.0f, 0.0f, 0.0f, 0.0f };
            lowerhue = hedefhue - tolerance;
            upperhue = hedefhue + tolerance;

            if (lowerhue >= 0 && upperhue <= 360) // hue diskin ortasinda
            {
                ranges[0] = lowerhue;
                ranges[1] = hedefhue;
                ranges[2] = hedefhue;
                ranges[3] = upperhue;
            }
            else if (lowerhue < 0 && upperhue > 360) //hatali veri
            {
                ranges[0] = 0.0f;
                ranges[1] = 1.0f;
                ranges[2] = 0.0f;
                ranges[3] = 1.0f;
            }
            else if (lowerhue < 0) // alttan tasma
            {
                ranges[0] = 0.0f;
                ranges[1] = upperhue;
                ranges[2] = 360.0f + lowerhue;
                ranges[3] = 1.0f;
            }
            else //ustten tasma
            {
                ranges[0] = lowerhue;
                ranges[1] = 1.0f;
                ranges[2] = 0.0f;
                ranges[3] = (upperhue - 360);
            }

            shader.SetFloat("lowerSaturation", lowersaturation / 100.0f);
            shader.SetFloat("upperSaturation", uppersaturation / 100.0f);
            shader.SetFloat("lowerValue", lowervalue / 100.0f);
            shader.SetFloat("upperValue", uppervalue / 100.0f);
            shader.SetFloat("lowerHue1", ranges[0] / 360.0f);
            shader.SetFloat("upperHue1", ranges[1] / 360.0f);
            shader.SetFloat("lowerHue2", ranges[2] / 360.0f);
            shader.SetFloat("upperHue2", ranges[3] / 360.0f);
            shader.SetFloat("erodeSize", erodesize);
            shader.SetFloat("dilateSize", dilatesize);
            
            shader.SetTexture(extractColorKernel, "SourceTex", webcamTexture);
            shader.SetTexture(extractColorKernel, "ExtractTex", extractColorRenderTexture);
            shader.SetBuffer(extractColorKernel, "rgbBuffer", rgbBuffer);
            shader.SetBuffer(extractColorKernel, "hsvBuffer", hsvBuffer);
            shader.Dispatch(extractColorKernel, width / 8, height / 8, 1);

            if (dilateSlider.value >= 1)
            {
                shader.SetTexture(dilateKernel, "ExtractTex", extractColorRenderTexture);
                shader.SetTexture(dilateKernel, "DilatedTex", dilateRenderTexture);
                shader.SetBuffer(dilateKernel, "dilateBuffer", dilatebuffer);
                shader.Dispatch(dilateKernel, width / 8, height / 8, 1);
                GetComponent<Renderer>().material.mainTexture = dilateRenderTexture;
            }else
            {
                GetComponent<Renderer>().material.mainTexture = extractColorRenderTexture;
            }
            
            toleranceValueText.text = toleranceSlider.value.ToString();
            hedefHueText.text = hedefHueSlider.value.ToString();
            lowerSaturationText.text = lowerSaturationSlider.value.ToString();
            upperSaturationText.text = upperSaturationSlider.value.ToString();
            lowerValueText.text = lowerValueSlider.value.ToString();
            upperValueText.text = upperValueSlider.value.ToString();
            erodeText.text = erodeSlider.value.ToString();
            dilateText.text = dilateSlider.value.ToString();

            rgbText.text = "FPS: " + (int)(1f / Time.deltaTime);
        }
    }

    public void OnToleranceValueChanged()
    {
        tolerance = toleranceSlider.value;
    }

    public void OnHueValueChanged()
    {
        hedefhue = hedefHueSlider.value;
    }

    public void OnLowerSaturationValueChanged()
    {
        lowersaturation = lowerSaturationSlider.value;
    }

    public void OnUpperSaturationValueChanged()
    {
        uppersaturation = upperSaturationSlider.value;
    }

    public void OnLowerValueValueChanged()
    {
        lowervalue = lowerValueSlider.value;
    }

    public void OnUpperValueValueChanged()
    {
        uppervalue = upperValueSlider.value;
    }

    public void OnErodeValueChanged()
    {
        erodesize = erodeSlider.value;
    }

    public void OnDilateValueChanged()
    {
        dilatesize = dilateSlider.value;
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
//rgbText.text = "R=" + Mathf.RoundToInt(rgbData[153280].x * 255f).ToString("D3") +
//    ", G=" + Mathf.RoundToInt(rgbData[153280].y * 255f).ToString("D3") +
//    ", B=" + Mathf.RoundToInt(rgbData[153280].z * 255f).ToString("D3");

//hsvText.text = "H=" + Mathf.RoundToInt(hsvData[153280].h * 360f).ToString("D3") +
//               ", S=" + Mathf.RoundToInt(hsvData[153280].s * 100f).ToString("D3") +
//               ", V=" + Mathf.RoundToInt(hsvData[153280].v * 100f).ToString("D3") + " FPS: " + (int)(1f / Time.deltaTime);