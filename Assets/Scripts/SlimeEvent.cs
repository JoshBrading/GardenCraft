using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SlimeEvent : NetworkBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        //PlayerController slimeStates = new PlayerController();
        if (other.gameObject.CompareTag("SlimeFood"))
        {
            //PlayerController player = GetComponent<PlayerController>();
            DisableSlimeEventServerRPC();
            //PlayerController.SlimeState = 0;
            
            Destroy(other);
        }
    }

    [ServerRpc]
    private void DisableSlimeEventServerRPC()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {

            PlayerController player = client.Value.PlayerObject.GetComponent<PlayerController>();

            player.SlimeActiveClientRPC(false);
        }
    }
}
