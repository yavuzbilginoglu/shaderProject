using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ComputeShaderExample : MonoBehaviour
{
    public ComputeShader shader;
    public RawImage rawImage;
    private int width = 640;
    private int height = 480;

    private WebCamTexture webcamTexture;
    private RenderTexture renderTextureKernel1;
    private RenderTexture renderTextureKernel2;

    private void Start()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();

        int kernel1 = shader.FindKernel("ChangeGreen");
        int kernel2 = shader.FindKernel("ChangeRed");

        renderTextureKernel1 = new RenderTexture(width, height, 0);
        renderTextureKernel1.enableRandomWrite = true;
        renderTextureKernel1.Create();

        renderTextureKernel2 = new RenderTexture(width, height, 0);
        renderTextureKernel2.enableRandomWrite = true;
        renderTextureKernel2.Create();

        StartCoroutine(ProcessWebcamTexture(kernel1, kernel2));
    }

    IEnumerator ProcessWebcamTexture(int kernel1, int kernel2)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame(); //her framede tekrar calismasi icin
            shader.SetTexture(kernel1, "SourceTex", webcamTexture);
            shader.SetTexture(kernel1, "_ResultTex", renderTextureKernel1);
            shader.Dispatch(kernel1, width / 8, height / 8, 1);

            shader.SetTexture(kernel2, "SourceTex", renderTextureKernel1);
            shader.SetTexture(kernel2, "_ResultTex", renderTextureKernel2);
            shader.Dispatch(kernel2, width / 8, height / 8, 1);
            
            rawImage.texture = renderTextureKernel2;
        }
    }
}
