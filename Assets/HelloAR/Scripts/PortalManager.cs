using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalManager : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject Sponza;
    private Material[] SponzaMaterilas;

    // Start is called before the first frame update
    void Start()
    {
        SponzaMaterilas = Sponza.GetComponent<Renderer>().sharedMaterials;
    }

    // Update is called once per frame
    void OnTriggerStay(Collider collider)
    {
        Vector3 cameraPositionInPortalSpace = transform.InverseTransformPoint(MainCamera.transform.position);

        if (cameraPositionInPortalSpace.y < 0.5f)
        {
            // Disable Stencil test
            for (int i = 0; i < SponzaMaterilas.Length; i++)
            {
                SponzaMaterilas[i].SetInt("_StencilComp", (int)CompareFunction.Always);
            }
        }
        else
        {
            // Enable Stencil test
            for (int i = 0; i < SponzaMaterilas.Length; i++)
            {
                SponzaMaterilas[i].SetInt("_StencilComp", (int)CompareFunction.Equal);
            }
        }
    }
}
