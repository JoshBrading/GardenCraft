using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SlimeEvent : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        PlayerController slimeStates = new PlayerController();

        if (other.gameObject.CompareTag("Ingredient"))
        {
            //PlayerController player = GetComponent<PlayerController>();
            
            PlayerController.SlimeState = 0;
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().SlimeActive();
            Destroy(other);
        }
    }
}
