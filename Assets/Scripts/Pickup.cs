using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Pickup : NetworkBehaviour
{

    public GameObject player;
    public Transform holdPos;
    public float throwForce = 500f; //force at which the object is thrown at
    public float pickUpRange = 5f; //how far the player can pickup the object from
    private float rotationSensitivity = 1f; //how fast/slow the object is rotated in relation to mouse movement
    private GameObject heldObj; //object which we pick up
    private Rigidbody heldObjRb; //rigidbody of object we pick up
    private bool canDrop = true; //this is needed so we don't throw/drop object when rotating the object
    private int LayerNumber; //layer index

    //MouseLookScript mouseLookScript;
    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("Pickup");

        //mouseLookScript = player.GetComponent<MouseLookScript>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObj == null) //if currently not holding anything
            {
                int Layer = LayerMask.NameToLayer("Pickup");
                //perform raycast to check if player is looking at object within pickuprange
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    //make sure pickup layer is attached
                    if (hit.transform.gameObject.layer == Layer)
                    {
                        //pass in object hit into the PickUpObject function
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    //StopClipping(); //prevents object from clipping through walls
                    DropObject();
                }
            }
        }
        if (heldObj != null) //if player is holding object
        {
            MoveObject(); //keep object position at holdPos
            RotateObject();
            if (Input.GetMouseButtonDown(1) && canDrop == true) //Throw
            {
                //StopClipping();
                ThrowObject();
            }

        }
    }
    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {

            if (!IsOwner)
            {
                Debug.Log("NOT OWNER");
                return;
            }
            else
            {
                Debug.Log("OWNER");
            }


            heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody

            heldObjRb.isKinematic = true;
            //heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
            heldObj.layer = 3; //change the object layer to the Pickup Layer

            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }

        /*if(pickUpObj.GetComponent<RequestIndex>().tag == "Mandrake Potion Recipe")
        {
            //Here write string that
        }
        else if (pickUpObj.GetComponent<RequestIndex>().tag == "Mushroom Potion Recipe")
        {
            //Here write string that
        }
        else if (pickUpObj.GetComponent<RequestIndex>().tag == "Pretty flower Potion Recipe")
        {
            //Here write string that
        }*/
    }
    void DropObject()
    {
        //re-enable collision with player
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 3; //object assigned back to default layer
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; //unparent object
        heldObj = null; //undefine game object
    }

    void MoveObject()
    {
        //keep object position the same as the holdPosition position
        //Debug.Log("Attempt Position Change");
        heldObj.transform.position = holdPos.transform.position;
    }
    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))//hold R key to rotate, change this to whatever key you want
        {
            canDrop = false; //make sure throwing can't occur during rotating

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
            //rotate the object depending on mouse X-Y Axis
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            //re-enable player being able to look around
            //mouseLookScript.verticalSensitivity = originalvalue;
            //mouseLookScript.lateralSensitivity = originalvalue;
            canDrop = true;
        }
    }
    void ThrowObject()
    {
        //same as drop function, but add force to object before undefining it
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 3;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
    }
}
