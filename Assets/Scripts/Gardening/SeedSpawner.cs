using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Unity.Netcode;

public class SeedSpawner : NetworkBehaviour
{

    public GameObject seed;
    public float range;
    public Transform spawnPoint;

    public Vector3 hitBoxSize = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("SeedCheck", 2f, 1f);
        //Instantiate(seed, spawnPoint);
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && IsServer)
        {
            var seedObject = Instantiate(seed, spawnPoint);
            var networkSeedObject = seedObject.GetComponent<NetworkObject>();
            networkSeedObject.Spawn();
        }
    }

    /*private void SeedCheck()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, hitBoxSize);

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<Seed>() == null)
            {
                Instantiate(seed, spawnPoint);
                Debug.Log("I need your seed");
            }
            *//*else
            {
                Instantiate(seed, spawnPoint);
                Debug.Log("I'm Good Bro");
            }*//*
        }
    }*/

    /*private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("I feel your seed");
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Seed1") || collider.gameObject.CompareTag("Seed2") || collider.gameObject.CompareTag("Seed3"))
        {
            Debug.Log("I need your seed");
            Instantiate(seed, spawnPoint);
        }

    }*/

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(hitBoxSize.x * 2, hitBoxSize.y * 2, hitBoxSize.z * 2));
    }*/
}
