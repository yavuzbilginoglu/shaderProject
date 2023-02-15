using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    //kamera goruntusunu gosteren UI elemaný
    public RawImage display;
    //kamera goruntusunu tutan nesne
    private WebCamTexture webcamTexture;
    private bool cameraStarted;
    public TextMeshProUGUI resolutionText;
    public TextMeshProUGUI fpsText;

    public void StartCamera()
    {
        // Eger kamera zaten baslamissa hicbir þey yapma
        if (cameraStarted)
            return;

        cameraStarted = true;

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("Kamera bulunamadý");
            return;
        }
        
        //640x480 e en yakin resolusyonu alma
        int closestIndex = 0;
        //kameranýn destekledigi resolusyonlarýný tek tek aliyoruz
        for (int i = 1; i < devices[0].availableResolutions.Length; i++)
        {
            //abs ile mutlak deger aliyoruz
            //boylece 640 a veya 480 e en yakýn degeri bulmus oluyoruz
            int diff1 = Mathf.Abs(devices[0].availableResolutions[i].width - 640);
            int diff2 = Mathf.Abs(devices[0].availableResolutions[closestIndex].width - 640);
            int diff3 = Mathf.Abs(devices[0].availableResolutions[i].height - 480);
            int diff4 = Mathf.Abs(devices[0].availableResolutions[closestIndex].height - 480);
            //en yakin degeri bulma 
            if (diff1 + diff3 < diff2 + diff4)
            {
                closestIndex = i;
            }
        }
        //kamera nesnesini gelen degerlere gore alma
        webcamTexture = new WebCamTexture(devices[0].name, devices[0].availableResolutions[closestIndex].width, devices[0].availableResolutions[closestIndex].height);
        //UI elemanina atama
        display.texture = webcamTexture;
        
        //kamera calistirma
        webcamTexture.Play();

        
        string resolutions = "Current Resolution: " + webcamTexture.width + "x" + webcamTexture.height;
        resolutionText.text = resolutions;

    }
    private void Update()
    {
        fpsText.text = "FPS: " + (int)(1f / Time.deltaTime);
    }
}
