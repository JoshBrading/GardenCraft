using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EventHandler : NetworkBehaviour
{
    private float timer;
    public float defaultTimer;

    private int eventIndex;

    public GameObject[] objectsToSpawn;
    public Transform[] spawnPoints;

    public List<GameObject> spawnedObjects;

    private int objectIndex; // Random objectsToSpawn index
    private int spawnIndex; // Random spawnPoints index

    private void Start()
    {
        timer = defaultTimer;
    }

    void Update()
    {
        if (!IsHost) return;
        timer -= Time.deltaTime;

        if (timer < 0)// When timer reaches 0 reset the timer
        {
            timer = 8;
            StartCoroutine(EventStart());
        }
    }
    IEnumerator EventStart()
    {
        eventIndex = Random.Range(0, 3);

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {

            PlayerController player = client.Value.PlayerObject.GetComponent<PlayerController>();

            if (eventIndex == 0) //Crow Outcome
            {
                spawnIndex = Random.Range(0, spawnPoints.Length);

                GameObject go = Instantiate(objectsToSpawn[0]);
                NetworkObject networkObject = go.GetComponent<NetworkObject>();
                networkObject.Spawn();
                networkObject.transform.position = spawnPoints[spawnIndex].position;

                spawnedObjects.Add(go);

                Debug.Log("Crow");
            }
            else if (eventIndex == 1) //Slime Outcome
            {
                Debug.Log("Slime");

                player.SlimeActiveClientRPC(true);


            }
            else if (eventIndex == 2)
            {
                Debug.Log("Do nothing Event");
            }


        }



        yield return null;
    }
}
