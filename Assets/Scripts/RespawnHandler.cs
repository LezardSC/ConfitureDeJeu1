using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnHandler : MonoBehaviour
{
    private GameObject characterObject;
    public Vector3 respawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        characterObject = FindObjectOfType<CharacterController>().gameObject;
        respawnPosition = characterObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RespawnCharacter()
    {
        StartCoroutine(RespawnCharacterCoroutine());
    }

    IEnumerator RespawnCharacterCoroutine()
    {
        characterObject.SetActive(false);
        yield return new WaitForSeconds(2);
        characterObject.transform.position = respawnPosition;
        characterObject.SetActive(true);
    }
}
