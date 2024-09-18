using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantHandler : MonoBehaviour
{
    public GameObject Plant1;
    public GameObject Plant2;
    public GameObject Plant3;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Seed1"))
        {
            Plant1.SetActive(true);
        }

        if (collision.gameObject.CompareTag("Seed2"))
        {
            Plant2.SetActive(true);
        }

        if (collision.gameObject.CompareTag("Seed3"))
        {
            Plant3.SetActive(true);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            //GrowthCycle.needsWater = false;
        }
    }
}
