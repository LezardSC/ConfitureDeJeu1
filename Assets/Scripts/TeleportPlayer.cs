using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public GameObject playerObject;
    public Transform destinationPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerObject.transform.position = destinationPoint.position;
        }
    }
}
