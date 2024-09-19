using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class SpawnAtRandomLocation : NetworkBehaviour
{
    public GameObject[] objectsToSpawn;
    public Transform[] spawnPoints;

    public List<GameObject> spawnedObjects;

    public int spawnCount; // How many objects should be spawned

    private int objectIndex; // Random objectsToSpawn index
    private int spawnIndex; // Random spawnPoints index

    private void Start()
    {


    }

    public void OnSessionStart()
    {
        Debug.LogWarning("Session Start");
        for (int i = 0; i < spawnCount; i++)
        {
            Debug.LogWarning("Pick Spawn");
            // For each iteration generate a random index; You could make an int array containing if an object already got spawned and change the index.
            objectIndex = Random.Range(0, objectsToSpawn.Length);
            spawnIndex = Random.Range(0, spawnPoints.Length);

            // Instantiate object
            GameObject obj = Instantiate(objectsToSpawn[objectIndex], spawnPoints[spawnIndex].position, Quaternion.identity);
            var networkObj = obj.GetComponent<NetworkObject>();
            networkObj.transform.position = spawnPoints[spawnIndex].position;
            // Add Object to spawnedObjects List
            spawnedObjects.Add(networkObj.gameObject);
        }
    }

    public void OnClientJoin()
    {
        if (!IsServer) return;
        foreach (var obj in spawnedObjects)
        {
            var networkObj = obj.GetComponent<NetworkObject>();
            networkObj.ChangeOwnership(GetNonHostPlayerId());
        }
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