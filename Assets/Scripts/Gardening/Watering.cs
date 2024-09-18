using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watering : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        //PlantGrowth plantGrowth = GetComponent<PlantGrowth>();
        
        if (other.GetComponent<PlantGrowth>() != null)
        {
            other.GetComponent<PlantGrowth>().WaterPlant();
        }

        if (other.GetComponent<TreeSpawner>() != null)
        {
            other.GetComponent<TreeSpawner>().WaterPlant();
        }
    }
}
