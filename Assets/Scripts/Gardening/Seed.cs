using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Seed : NetworkBehaviour
{
    public GameObject Plant;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            var plantObject = Instantiate(Plant, transform.position, Quaternion.identity);
            var networkPlantObject = plantObject.GetComponent<NetworkObject>();
            networkPlantObject.Spawn();
            networkPlantObject.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y + 0.25f, collision.transform.position.z);
            Destroy(gameObject);
        }

    }
}
