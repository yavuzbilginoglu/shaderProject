using UnityEngine;

public class MaterialControl : MonoBehaviour
{
    private Material ShaderMat;

    void Start()
    {
        ShaderMat = GetComponent<Renderer>().material;
    }

}
