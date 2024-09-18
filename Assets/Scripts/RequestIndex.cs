using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestIndex : MonoBehaviour
{
    private int reqIndex;
    // Start is called before the first frame update
    void Start()
    {
        reqIndex = Random.Range(0, 2);

        if(reqIndex == 0 )
        {
            //Potion 1 = Mandrake
            Debug.Log("Mandrake Potion needed");
            PotionSubmission.potionName = "Mandrake Potion(Clone)";
        }

        if( reqIndex == 1 )
        {
            //Potion 2 = Mushroom
            Debug.Log("Mushroom Potion needed");
            PotionSubmission.potionName = "Mushroom Potion(Clone)";
        }

        if ( reqIndex == 2 )
        {
            //Potion 3 = Pretty flower
            Debug.Log("Pretty Flower Potion needed");
            PotionSubmission.potionName = "Pretty Flower Potion(Clone)";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
