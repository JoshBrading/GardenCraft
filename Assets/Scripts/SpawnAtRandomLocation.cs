using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpawnAtRandomLocation : MonoBehaviour
{
    public GameObject[] objectsToSpawn;
    public Transform[] spawnPoints;

    public List<GameObject> spawnedObjects;

    public int spawnCount; // How many objects should be spawned

    private int objectIndex; // Random objectsToSpawn index
    private int spawnIndex; // Random spawnPoints index

    private void Start()
    {
        // This for loop is to not hardcode the spawn count
        for (int i = 0; i < spawnCount; i++)
        {
            // For each iteration generate a random index; You could make an int array containing if an object already got spawned and change the index.
            objectIndex = Random.Range(0, objectsToSpawn.Length);
            spawnIndex = Random.Range(0, spawnPoints.Length);

            // Instantiate object
            GameObject go = Instantiate(objectsToSpawn[objectIndex], spawnPoints[spawnIndex].position, Quaternion.identity);

            // Add Object to spawnedObjects List
            spawnedObjects.Add(go);
        }
    }
}