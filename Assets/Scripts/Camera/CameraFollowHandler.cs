using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollowHandler : MonoBehaviour
{
    public Transform playerTransform;
    private CharacterController characterController;
    private Vector3 defaultCameraPosition;
    private Vector3 targetDestination;
    public float lerpSpeed;

    private void Awake()
    {
        defaultCameraPosition = this.gameObject.transform.position;
        characterController = playerTransform.GetComponent<CharacterController>();
    }

    void Update()
    {
        targetDestination = playerTransform.position + defaultCameraPosition;

        transform.position = Vector3.Lerp(transform.position, new Vector3(targetDestination.x, transform.position.y, targetDestination.z), Time.deltaTime * lerpSpeed);

        if (characterController.afterJumpTimer == 0)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetDestination.y, transform.position.z), Time.deltaTime * lerpSpeed);
        }
    }
}
