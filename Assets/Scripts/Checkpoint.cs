using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject highlighter;
    
    // Start is called before the first frame update
    void Start()
    {
        highlighter.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            highlighter.SetActive(true);
            FindObjectOfType<RespawnHandler>().respawnPosition = this.transform.position;
        }
    }
}
