using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUpPickup : MonoBehaviour
{
    public string powerUpName;
    public TMPro.TextMeshProUGUI powerUpText;
    private GameObject powerUpTextGO;
    public int powerUpScriptVariableOrder;

    // Start is called before the first frame update
    void Start()
    {
        powerUpTextGO = powerUpText.gameObject;
        powerUpTextGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CharacterController characterController = other.GetComponent<CharacterController>();
            
            StartCoroutine(ShowPowerUpText());
            switch (powerUpScriptVariableOrder)
            {
                case 1:
                    characterController.canUseMask = true;
                    break;
                case 2:
                    characterController.canUseWallJump = true;
                    break;
                case 3:
                    characterController.canUseDoubleJump = true;
                    break;
            }
        }
    }

    IEnumerator ShowPowerUpText()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
        powerUpText.text = "Vous avez obtenu le " + powerUpName + " !";
        powerUpTextGO.SetActive(true);
        yield return new WaitForSeconds(3);
        powerUpTextGO.SetActive(false);
        Destroy(this.gameObject);
    }
}
