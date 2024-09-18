using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class QuickHostClientAutoTransfer : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log(other.gameObject.tag);
            this.GetComponent<NetworkObject>().ChangeOwnership(other.GetComponent<NetworkObject>().OwnerClientId);
            Debug.Log($"Ownership Changed to: {this.GetComponent<NetworkObject>().OwnerClientId}");
        }
    }
}
