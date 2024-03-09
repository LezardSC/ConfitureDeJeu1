using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMaskHandler : MonoBehaviour
{
    public LayerMask normalMask;
    public LayerMask alternateMask;
    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = this.GetComponent<Camera>();

        ChangeLayerMask(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeLayerMask(bool isNormal)
    {
        if (isNormal) camera.cullingMask = normalMask;
        else camera.cullingMask = alternateMask;
    }
}
