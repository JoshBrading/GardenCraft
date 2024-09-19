
using TMPro;
using UnityEngine;
using Unity.VisualScripting;
using Unity.Netcode;

public class UI_Interaction_Manager : NetworkBehaviour
{
    Camera _camera;
    public TMP_Text centerText;
    public TMP_Text recipe;
    public float interactReach;
    // Start is called before the first frame update
    void Start()
    {
        _camera = NetworkManager.Singleton.LocalClient.PlayerObject.transform.GetChild(0).transform.GetChild(2).GetComponent<Camera>();
        //_camera = this.GetComponent<Camera>();
        centerText.text = "+";
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        centerText.text = "+";
        // Perform the raycast
        if (Physics.Raycast(ray, out hit, interactReach))
        {
            // Check if the object has a tag
            if (hit.collider != null)
            {
                int layer = hit.collider.gameObject.layer;
                if (LayerMask.LayerToName(layer) == "Interact")
                {
                    var name = Variables.Object(hit.collider.gameObject).Get("Name");
                    centerText.text = "Press E to interact with " + name;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        var interactable = hit.collider.GetComponent<InteractableAnimal>();
                        if (interactable != null)
                        {
                            interactable.OnInteract();
                        }
                        else
                        {
                            Debug.LogWarning("InteractableAnimal component not found on the hit object.");
                        }
                    }
                }


                if (LayerMask.LayerToName(layer) == "Pickup")
                {
                    string name = "";
                    if (!NetworkManager.IsServer && hit.collider.CompareTag("Recipe"))
                    {
                        name = "Pickup to learn " + (string)Variables.Object(hit.collider.gameObject).Get("Name");
                        if (Input.GetMouseButtonDown(0))
                        {
                            recipe.text = $"Combine the following...\n{(string)Variables.Object(hit.collider.gameObject).Get("Ingredient_1")}\n{(string)Variables.Object(hit.collider.gameObject).Get("Ingredient_2")}";
                        }
                    }
                    else
                    {
                        name = (string)Variables.Object(hit.collider.gameObject).Get("Name");
                    }
                    centerText.text = name;
                }
            }
        }
    }

}
