using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class InteractableAnimal : NetworkBehaviour
{
    public Transform spawn;
    public GameObject[] recipes;
    private GameObject recipe;

    // Start is called before the first frame update
    void Start()
    {
        recipe = recipes[Random.Range(0, recipes.Length)];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if( collision.gameObject.CompareTag("Potion"))
        {
            Destroy(collision.gameObject);
        }
    }


    public void OnInteract()
    {
        OnInteractServerRPC();
    }

    [ServerRpc]
    public void OnInteractServerRPC()
    {
        var recipeObj = Instantiate(recipe);
        var networkRecipeObj = recipeObj.GetComponent<NetworkObject>();
        networkRecipeObj.SpawnWithOwnership(GetNonHostPlayerId());
        networkRecipeObj.transform.position = spawn.position;
    }

    ulong GetNonHostPlayerId()
    {
        ulong hostId = NetworkManager.Singleton.LocalClientId;

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key != hostId)
            {
                return client.Key;
            }
        }

        return 0;
    }
}
