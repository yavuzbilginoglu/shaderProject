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
    private string resolutions;

    private void Update()
    {
        fpsText.text = "FPS: " + (int)(1f / Time.deltaTime);
    }
    public void StartCamera()
    {
        // Eger kamera zaten baslamissa hicbir sey yapma
        if (cameraStarted)
            return;

        cameraStarted = true;

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            resolutions = "Kamera bulunamadi";
            return;
        }
        //secilen kamerayi tutacak degisken
        WebCamDevice selectedDevice = devices[0];

        int minDistance = int.MaxValue;
        //tum kameralarý dondurme
        for (int i = 0; i < devices.Length; i++)
        {
            WebCamDevice device = devices[i];
            for (int j = 0; j < device.availableResolutions.Length; j++)
            {
                //kameralarin destekledigi cozunurlukleri Vector2Int'e donustur
                Vector2Int resolution = new Vector2Int(device.availableResolutions[j].width, device.availableResolutions[j].height);

                //640x480 ile olan mesafeyi hesapla
                int distance = (resolution - new Vector2Int(640, 480)).sqrMagnitude;
                //mevcut kameranin 640x480'e olan mesafesi en kucuk olani kaydet
                if (distance < minDistance)
                {
                    minDistance = distance;
                    selectedDevice = device;
                }
            }
        }


        int selectedResolutionIndex = 0;
        WebCamTexture webcamTexture = null;


        //Secilen kameranin 640x480 cozunurlugune sahip olup olmadigini kontrol et
        for (int i = 0; i < selectedDevice.availableResolutions.Length; i++)
        {
            if (selectedDevice.availableResolutions[i].width == 640 && selectedDevice.availableResolutions[i].height == 480)
            {
                selectedResolutionIndex = i;
                webcamTexture = new WebCamTexture(selectedDevice.name, selectedDevice.availableResolutions[selectedResolutionIndex].width, selectedDevice.availableResolutions[selectedResolutionIndex].height);
                break;
            }
        }
        //secilen kameranin 640x480 cozunurlugu yoksa en yakin cozunurlugu kullan
        if (webcamTexture == null)
        {
            selectedResolutionIndex = 0;
            webcamTexture = new WebCamTexture(selectedDevice.name, selectedDevice.availableResolutions[selectedResolutionIndex].width, selectedDevice.availableResolutions[selectedResolutionIndex].height);
        }

        if (selectedResolutionIndex == -1)
        {
            resolutions = "640x480 res destekleyen kamera bulunamadi.";
            return;
        }
        
        //UI elemanina atama
        display.texture = webcamTexture;
        
        //kamera calistirma
        webcamTexture.Play();

        
        resolutions = "Current Resolution: " + webcamTexture.width + "x" + webcamTexture.height;
        resolutionText.text = resolutions;

    }
    
}
