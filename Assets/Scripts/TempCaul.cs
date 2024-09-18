using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCaul : MonoBehaviour
{
    public GameObject potion1;
    public GameObject potion2;
    public GameObject potion3;
    
    //This script doesn't separate by potion type
    public void TurnToPoint(int potind)
    {
        if (potind == 0)
        {
            Instantiate(potion1, transform.position, Quaternion.identity);
        }

        if (potind == 1)
        {
            Instantiate(potion2, transform.position, Quaternion.identity);
        }
        
        if (potind == 2)
        {
            Instantiate(potion3, transform.position, Quaternion.identity);
        }
    }
}
