using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalOnlyCollidersHandler : MonoBehaviour
{
    public GameObject[] normalOnlyColliders;
    // Start is called before the first frame update
    void Start()
    {
        normalOnlyColliders = GameObject.FindGameObjectsWithTag("NormalOnlyCollider");
        foreach (GameObject normalOnlyCollider in normalOnlyColliders)
        {
            normalOnlyCollider.SetActive(true);
        }
    }

    public void ShowMaskOnlyColliders(bool visible)
    {
        foreach (GameObject normalOnlyCollider in normalOnlyColliders)
        {
            normalOnlyCollider.SetActive(visible);
        }
    }
}
