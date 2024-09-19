using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class PlantGrowth : NetworkBehaviour
{
    private int currentProgression = 0;
    public int timeBetweenGrowths;
    public int maxGrowth;
    private float MustWaterTimer;
    public float MustWaterDefault;
    public GameObject waterInd;
    public bool isWatered;
    public GameObject ingredientPrefab;
    // Start is called before the first frame update
    void Start()
    {
        MustWaterTimer = MustWaterDefault;
        InvokeRepeating("Growth", timeBetweenGrowths, timeBetweenGrowths);
        //isWatered = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Growth();

        MustWaterTimer -= Time.deltaTime;
        if (isWatered == false)
        {
            waterInd.SetActive(true);
        }
        else
        {
            waterInd.SetActive(false);
        }

        if (MustWaterTimer <= 0)
        {
            isWatered = false;
        }

        if (currentProgression == maxGrowth)
        {
            //MustWaterTimer = MustWaterDefault;
            this.gameObject.tag = "FullGrown";
            SpawnIngredientServerRPC();
            Destroy(this.gameObject);
            //crow.SetActive(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnIngredientServerRPC()
    {
        var ingredient = Instantiate(ingredientPrefab);
        var networkIngredient = ingredient.GetComponent<NetworkObject>();
        networkIngredient.Spawn();
        networkIngredient.transform.position = new Vector3( this.transform.position.x, this.transform.position.y + 2, this.transform.position.z );
    }

    public void Growth()
    {
        if (isWatered == true && currentProgression != maxGrowth)
        {
            gameObject.transform.GetChild(currentProgression).gameObject.SetActive(true);
        }
        if (isWatered == true && currentProgression > 0 && currentProgression < maxGrowth)
        {
            gameObject.transform.GetChild(currentProgression - 1).gameObject.SetActive(false);
        }
        if (isWatered == true && currentProgression < maxGrowth)
        {
            currentProgression++;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                //wateringParticle.Play();
                MustWaterTimer = MustWaterDefault;
                isWatered = true;
                //Growth();
            }
        }
    }

    public void WaterPlant()
    {
        MustWaterTimer = MustWaterDefault;
        isWatered = true;
        Debug.Log("I'm so wet");
    }
}
