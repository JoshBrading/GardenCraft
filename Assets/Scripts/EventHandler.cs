using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EventHandler : MonoBehaviour
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
        
            
        if (eventIndex == 0) //Crow Outcome
        {
            spawnIndex = Random.Range(0, spawnPoints.Length);

            GameObject go = Instantiate(objectsToSpawn[0], spawnPoints[spawnIndex].position, Quaternion.identity);

            spawnedObjects.Add(go);

            Debug.Log("Crow");
        }
        else if (eventIndex == 1) //Slime Outcome
        {
            Debug.Log("Slime");

            PlayerController player = GetComponent<PlayerController>();

            PlayerController.SlimeState = 1;
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().SlimeActive();


        }
        else if(eventIndex == 2)
        {
            Debug.Log("Do nothing Event");
        }



        yield return null;
    }
}
