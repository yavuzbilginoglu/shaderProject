using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public RawImage display;
    public TextMeshProUGUI resolutionText;
    public TextMeshProUGUI resolutionsText;
    public TextMeshProUGUI fpsText;
    private string resolutions;
    private bool cameraStarted;
    private WebCamTexture webcamTexture;
    private Texture2D texture;


    public IEnumerator play()
    {
        yield return new WaitForSeconds(2.0f);

        //webcamTexture.Play();
    }

    private void Start()
    {
        
    }
    public void StartCamera_640_back()
    {
        if (cameraStarted)
            return;
        else cameraStarted = true;
        

        //int inputWidth = int.Parse(inputFieldWidth.text);
       // int inputHeight = int.Parse(inputFieldHeight.text);



        WebCamDevice[] devices = WebCamTexture.devices;
        WebCamDevice selectedDevice = devices[0];
        
        //if (inputFieldWidth.text == null || inputFieldHeight.text == null)
        //{
        //    resolutionText.text = "resolution girilmedi!!!";
        //    return;
        //}
        webcamTexture = new WebCamTexture(selectedDevice.name);

        display.texture = webcamTexture;


        //Coroutine coroutine = StartCoroutine(play());

        webcamTexture.Play();
        resolutionText.text = "Current Resolution: " + webcamTexture.width + "x" + webcamTexture.height;



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

        int cozunurluk = 480 * 640;

        int minDistance = int.MaxValue;
        
        //tum kameralarý dondurme
        for (int i = 0; i < devices.Length; i++)
        {
            WebCamDevice device = devices[i];
            for (int j = 0; j < device.availableResolutions.Length; j++)
            {
                
                int resolution = device.availableResolutions[j].width * device.availableResolutions[j].height;

                //640x480 ile olan mesafeyi hesapla
                int distance = Mathf.Abs(cozunurluk - resolution);
                
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
    
    
    private void Update()
    {
        fpsText.text = "FPS: " + (int)(1f / Time.deltaTime);
    }
}
