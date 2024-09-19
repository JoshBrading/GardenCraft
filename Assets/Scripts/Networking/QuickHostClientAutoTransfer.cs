using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class QuickHostClientAutoTransfer : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log(other.gameObject.tag);
            if( other.gameObject.CompareTag("Player"))
            {
                this.GetComponent<NetworkObject>().ChangeOwnership(other.GetComponent<NetworkObject>().OwnerClientId);
                Debug.Log($"Ownership Changed to: {this.GetComponent<NetworkObject>().OwnerClientId}");
            }
        }
    }
}
