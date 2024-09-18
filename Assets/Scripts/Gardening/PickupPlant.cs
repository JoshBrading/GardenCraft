using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupPlant : MonoBehaviour
{
    public Transform pickupPoint;
    public int plantIndex;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("I'M BEING TOUCHED");
        if (collision.gameObject.CompareTag("Crow"))
        {
            pickupPoint.position = transform.position;
            transform.parent = pickupPoint.transform;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<BoxCollider>().enabled = false;
            
        }

        if(collision.gameObject.CompareTag("Cauldron"))
        {
            Debug.Log("LET ME RUB YOUR CAULDRON");
            if(plantIndex == 0)
            {
                collision.gameObject.GetComponent<TempCaul>().TurnToPoint(0); //Mandrake potion
            }

            if (plantIndex == 1)
            {
                collision.gameObject.GetComponent<TempCaul>().TurnToPoint(1); ; //Mushroom potion
            }

            if (plantIndex == 2)
            {
                collision.gameObject.GetComponent<TempCaul>().TurnToPoint(2); //Pretty flower potion
            }
            Destroy(gameObject);
        }
        
    }
}
