using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class test : MonoBehaviour
{
    private Action<int> onMask;

    // Start is called before the first frame update
    void Start()
    {
        onMask += HandleMask;
    }

    // Update is called once per frame
    void Update()
    {
        onMask?.Invoke(5);
    }

    void HandleMask(int n)
    {

    }
}
