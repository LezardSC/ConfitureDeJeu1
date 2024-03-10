using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyRelatedPlatformButton : MonoBehaviour
{
    public GameObject relatedPlatform;

    // Start is called before the first frame update
    void Start()
    {
        relatedPlatform.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(relatedPlatform);
            Destroy(this.gameObject);
        }
    }
}
