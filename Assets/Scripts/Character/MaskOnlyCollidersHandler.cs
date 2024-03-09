using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaskOnlyCollidersHandler : MonoBehaviour
{
    public GameObject[] maskOnlyColliders;
    // Start is called before the first frame update
    void Start()
    {
        maskOnlyColliders = GameObject.FindGameObjectsWithTag("MaskOnlyCollider");
        foreach (GameObject maskOnlyCollider in maskOnlyColliders)
        {
            maskOnlyCollider.SetActive(false);
        }
    }

    public void ShowMaskOnlyColliders(bool visible)
    {
        foreach (GameObject maskOnlyCollider in maskOnlyColliders)
        {
            maskOnlyCollider.SetActive(visible);
        }
    }
}
