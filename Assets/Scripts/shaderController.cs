using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class shaderController : MonoBehaviour
{
    public Scrollbar toleranceScrollbar;
    public Scrollbar smoothnessScrollbar;
    public Scrollbar invertScrollbar;
    public Scrollbar matchScrollbar;
    public Scrollbar desaturateScrollbar;
    public Scrollbar mapAlphaScrollbar;
    public Scrollbar dilateScrollbar;


    public Material material;
    private float toleranceValue;
    private float smoothnessValue;
    private float invertValue;
    private float matchValue;
    private float desaturateValue;
    private float mapalphaValue;
    private float dilateValue;

    void Start()
    {
        Shader shader = Shader.Find("Custom/ColorExtractor");
    }

    void Update()
    {
        toleranceValue = toleranceScrollbar.value;
        smoothnessValue = smoothnessScrollbar.value;
        invertValue = invertScrollbar.value;
        matchValue = matchScrollbar.value;
        desaturateValue = desaturateScrollbar.value;
        mapalphaValue = mapAlphaScrollbar.value;
        dilateValue = dilateScrollbar.value;

        material.SetFloat("_Tolerance", toleranceValue);
        material.SetFloat("_Smoothness", smoothnessValue);
        material.SetFloat("_Invert", invertValue);
        material.SetFloat("_Match", matchValue);
        material.SetFloat("_Desaturate", desaturateValue);
        material.SetFloat("_MapAlpha", mapalphaValue);
        material.SetFloat("_Dilate", dilateValue);
    }
}
