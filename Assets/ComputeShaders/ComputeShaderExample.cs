using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ComputeShaderExample : MonoBehaviour
{
    public ComputeShader shader;
    public RawImage rawImage;

    private RenderTexture renderTexture;

    private void Start()
    {
        string imagePath = "Assets/Textures/rawImage.jpg";

        Texture2D sourceTexture = new Texture2D(2, 2);
        byte[] imageData = File.ReadAllBytes(imagePath);
        sourceTexture.LoadImage(imageData);

        int kernel2 = shader.FindKernel("ChangeGreen");
        int kernel3 = shader.FindKernel("ChangeRed");

        renderTexture = new RenderTexture(sourceTexture.width, sourceTexture.height, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        RenderTexture.active = renderTexture;
        Graphics.Blit(sourceTexture, renderTexture);

        shader.SetTexture(kernel2, "_ResultTex", renderTexture);
        shader.Dispatch(kernel2, 512 / 8, 512 / 8, 1);

        shader.SetTexture(kernel3, "_ResultTex", renderTexture);
        shader.Dispatch(kernel3, 512 / 8, 512 / 8, 1);
        rawImage.texture = renderTexture;

    }
}
