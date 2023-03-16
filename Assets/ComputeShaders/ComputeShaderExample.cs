using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ComputeShaderExample : MonoBehaviour
{
    public ComputeShader shader;
    public RawImage rawImage;

    private RenderTexture renderTextureKernel1;
    private RenderTexture renderTextureKernel2;
    public int width = 512;
    public int height = 512;
    public Texture2D sourceTexture;

    private void Start()
    {

        int kernel1 = shader.FindKernel("ChangeGreen");
        int kernel2 = shader.FindKernel("ChangeRed");

        renderTextureKernel1 = new RenderTexture(width, height, 0);
        renderTextureKernel1.enableRandomWrite = true;
        renderTextureKernel1.Create();

        renderTextureKernel2 = new RenderTexture(width, height, 0);
        renderTextureKernel2.enableRandomWrite = true;
        renderTextureKernel2.Create();

        //RenderTexture.active = renderTextureKernel1;

        //Graphics.Blit(sourceTexture, renderTextureKernel1);

        shader.SetTexture(kernel1, "SourceTex", sourceTexture);
        shader.SetTexture(kernel1, "_ResultTex", renderTextureKernel1);
        shader.Dispatch(kernel1, width / 8, height / 8, 1);


        shader.SetTexture(kernel2, "SourceTex", renderTextureKernel1);
        shader.SetTexture(kernel2, "_ResultTex", renderTextureKernel2);
        shader.Dispatch(kernel2, width / 8, height / 8, 1);
        
        rawImage.texture = renderTextureKernel2;

    }
}
