using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitch : MonoBehaviour
{
    public RawImage display;
    public Button backCameraButton;
    public Button frontCameraButton;
    public TextMeshProUGUI resolutionText;
    public TextMeshProUGUI fpsText;

    private WebCamTexture webcamTexture;
    private bool cameraStarted;
    private bool isFrontFacing = false;
    private string resolutions;

    void Start()
    {
        frontCameraButton.onClick.AddListener(SwitchToFrontCamera);
        backCameraButton.onClick.AddListener(SwitchToBackCamera);
        //StartCamera();
    }

    void SwitchToFrontCamera()
    {
        if (isFrontFacing)
            return;

        display.transform.rotation = Quaternion.Euler(180, 0, -90);
        isFrontFacing = true;
        StartCamera();
    }

    void SwitchToBackCamera()
    {
        if (!isFrontFacing)
            return;

        isFrontFacing = false;
        display.transform.rotation = Quaternion.Euler(0, 0, -90);
        StartCamera();
    }

    public void StartCamera()
    {
        if (cameraStarted)
        {
            webcamTexture.Stop();
            cameraStarted = false;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            resolutions = "Kamera bulunamadi";
            resolutionText.text = resolutions;
            return;
        }

      
        WebCamDevice selectedDevice = devices[0];
        for (int i = 0; i < devices.Length; i++)
        {
            var device = devices[i];
            if ((isFrontFacing && device.isFrontFacing) || (!isFrontFacing && !device.isFrontFacing))
            {
                selectedDevice = device;
                break;
            }
        }

        int minDistance = int.MaxValue;
        int selectedResolutionIndex = 0;
        for (int i = 0; i < selectedDevice.availableResolutions.Length; i++)
        {
            Vector2Int resolution = new Vector2Int(selectedDevice.availableResolutions[i].width, selectedDevice.availableResolutions[i].height);
            int distance = (resolution - new Vector2Int(640, 480)).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                selectedResolutionIndex = i;
            }
        }

        webcamTexture = new WebCamTexture(selectedDevice.name, selectedDevice.availableResolutions[selectedResolutionIndex].width, selectedDevice.availableResolutions[selectedResolutionIndex].height);
        display.texture = webcamTexture;
        webcamTexture.Play();
        cameraStarted = true;
        resolutions = "Current Resolution: " + webcamTexture.width + "x" + webcamTexture.height;
        resolutionText.text = resolutions;

    }
}
