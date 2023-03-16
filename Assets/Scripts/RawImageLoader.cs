using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class RawImageLoader : MonoBehaviour
{
    int textureWidth = 256;
    int textureHeight = 256;

    int rawImageWidth = 512;
    int rawImageHeight = 512;
    public RawImage rawImage;

    void Start()
    {
        Texture2D texture = (Texture2D)rawImage.texture;

        byte[] rawImageData = texture.GetRawTextureData();

        Texture2D texture2 = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

        Color[] colors = new Color[textureWidth * textureHeight];
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                int rawX = Mathf.Clamp(x * rawImageWidth / textureWidth, 0, rawImageWidth - 1);
                int rawY = Mathf.Clamp(y * rawImageHeight / textureHeight, 0, rawImageHeight - 1);
                int index = (rawY * rawImageWidth + rawX) * 4; // 4: RGBA32 formati icin
                byte r = rawImageData[index];
                byte g = rawImageData[index + 1];
                byte b = rawImageData[index + 2];
                byte a = rawImageData[index + 3];
                colors[y * textureWidth + x] = new Color32(r, g, b, a);
            }
        }
        texture2.SetPixels(colors);
        texture2.Apply();

        GetComponent<Renderer>().material.SetTexture("_MainTex", texture2);
    }

}
