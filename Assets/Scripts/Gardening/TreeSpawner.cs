using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public float timeBeforeGrowth;
    public float growthTimeDefault;
    public float timeBeforeWater;
    private bool isWatered = false;
    public GameObject apple;


    // Update is called once per frame
    void Update()
    {
        timeBeforeWater -= Time.deltaTime;

        if (isWatered == true)
        {
            timeBeforeGrowth -= Time.deltaTime;
        }

        if (timeBeforeWater <= 0)
        {
            isWatered = false;
        }


        if (timeBeforeGrowth < 0)
        {
            Grow();
        }
    }

    public void WaterPlant()
    {
        timeBeforeWater = growthTimeDefault;
        isWatered = true;
    }

    public void Grow()
    {
        //Implement the growth logic here
        Debug.Log("The apple has appeared");
        Instantiate(apple, transform.position, Quaternion.identity);
        timeBeforeGrowth = growthTimeDefault;
    }

    /*public GameObject apple;
    public float spawnInterval = 10f;
    public float waterRequirement = 5f;

    //public GameObject waterInd;

    private void Start()
    {
        StartCoroutine(SpawnApple());
    }

    private IEnumerator SpawnApple()
    {
        while (true)
        {
            //Spawn a new plant
            GameObject newPlant = Instantiate(apple, transform.position, Quaternion.identity);
            Tree appleScript = newPlant.GetComponent<Tree>();
            if(appleScript != null )
            {
                appleScript.SetWaterRequirement(waterRequirement);
            }

            //Wait for the new spawn
            yield return new WaitForSeconds(spawnInterval);
        }    
    }*/
}
