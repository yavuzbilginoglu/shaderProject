using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class shaderControl : MonoBehaviour
{
    public ComputeShader shader;
    public Slider toleranceSlider;
    public Slider hedefHueSlider;
    public Slider lowerSaturationSlider;
    public Slider upperSaturationSlider;
    public Slider lowerValueSlider;
    public Slider upperValueSlider;

    public TextMeshProUGUI toleranceValueText;
    public TextMeshProUGUI hedefHueText;
    public TextMeshProUGUI lowerSaturationText;
    public TextMeshProUGUI upperSaturationText;
    public TextMeshProUGUI lowerValueText;
    public TextMeshProUGUI upperValueText;

    private float tolerance;
    private float hedefhue;
    private float lowersaturation;
    private float uppersaturation;
    private float lowervalue;
    private float uppervalue;

    void Update()
    {
        tolerance = toleranceSlider.value;
        hedefhue = hedefHueSlider.value;
        lowersaturation = lowerSaturationSlider.value;
        uppersaturation = upperSaturationSlider.value;
        lowervalue = lowerValueSlider.value;
        uppervalue = upperValueSlider.value;


        shader.SetFloat("tolerance", tolerance);
        shader.SetFloat("hedefHue", hedefhue);
        shader.SetFloat("lowerSaturation", lowersaturation);
        shader.SetFloat("upperSaturation", uppersaturation);
        shader.SetFloat("lowerValue", lowervalue);
        shader.SetFloat("upperValue", uppervalue);



        toleranceValueText.text = tolerance.ToString();
        hedefHueText.text = hedefhue.ToString();
        lowerSaturationText.text = lowersaturation.ToString();
        upperSaturationText.text = uppersaturation.ToString();
        lowerValueText.text = lowervalue.ToString();
        upperValueText.text = uppervalue.ToString();
    }
}
