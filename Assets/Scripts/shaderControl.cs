using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class shaderControl : MonoBehaviour
{
    public ComputeShader shader;
    public ComputeShaderExample computeShaderExample;

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

    void Start()
    {
        computeShaderExample = FindObjectOfType<ComputeShaderExample>();
    }
    
    void Update()
    {
        if (computeShaderExample.webcamTexture.isPlaying && computeShaderExample.webcamTexture.didUpdateThisFrame)
        {
            tolerance = toleranceSlider.value;
            hedefhue = hedefHueSlider.value;
            lowersaturation = lowerSaturationSlider.value;
            uppersaturation = upperSaturationSlider.value;
            lowervalue = lowerValueSlider.value;
            uppervalue = upperValueSlider.value;
            erodesize = erodeSlider.value;
            dilatesize = dilateSlider.value;
            

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
            else if(lowerhue < 0) // alttan tasma
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
            

            toleranceValueText.text = toleranceSlider.value.ToString();
            hedefHueText.text = hedefHueSlider.value.ToString();
            lowerSaturationText.text = lowerSaturationSlider.value.ToString();
            upperSaturationText.text = upperSaturationSlider.value.ToString();
            lowerValueText.text = lowerValueSlider.value.ToString();
            upperValueText.text = upperValueSlider.value.ToString();
            erodeText.text = erodeSlider.value.ToString();
            dilateText.text = dilateSlider.value.ToString();
            
        }
    }
}
