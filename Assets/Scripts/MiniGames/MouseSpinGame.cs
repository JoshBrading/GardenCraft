using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Color = UnityEngine.Color;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.Audio;
using UnityEngine.VFX;

public class PotionMixer : NetworkBehaviour
{
    [Header("Camera")]
    public Transform focusTransform;
    [Range(0.1f, 4f)] public float cameraTrackingSpeed = 1.5f;
    private Camera playerCamera;

    [Header("UI")]
    public SpriteRenderer leftTurnSprite;
    public SpriteRenderer rightTurnSprite;

    [Header("Spawning")]
    public Transform[] spawnPositions;
    public Transform potionSpawn;

    [Header("Stir")]
    public GameObject stirObject;
    [Range(0.1f, 10f)] public float stirTrackingSpeed = 1.5f;
    public float maxStirRadius = 1f;
    public float rayDistance = 1f;
    private bool controlStir = false;

    [Header("Tracking")]
    public Collider stirCollider;

    [Header("Shaders")]
    public float colorTransitionSpeed = 1f;

    public GameObject potion;

    public AudioSource audioSource;
    public AudioClip[] splashSFX;

    public VisualEffect particles;

    private int ingredientCount = 0;

    private int clockwiseCount;
    private int counterClockwiseCount;

    private float currentStirAngle;

    public PlayerInputActions playerControls;
    private InputAction press;
    private MeshRenderer couldronMeshRenderer;

    private Color targetColor;
    private Color targetColor2;

    public Color default1;
    public Color default2;

    private float angleSum;

    private Transform oldCameraTransform;

    private List<string> recipeTurnOrder;
    private int recipeTurnIndex = 0;
    private void Awake()
    {
        couldronMeshRenderer = this.transform.GetChild(0).GetComponent<MeshRenderer>();

        playerControls = new PlayerInputActions();

        targetColor = couldronMeshRenderer.material.GetColor("_Color");
        targetColor2 = couldronMeshRenderer.material.GetColor("_Color2");
    }

    void OnEnable()
    {
        press = playerControls.Player.Fire;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player") HandlePlayerEnter(other);
        if (other.tag == "Ingredient") HandleIngredientEnter(other);
        if (other.tag == "Recipe") HandleRecipeEnter(other);

    }

    private void HandlePlayerEnter(Collider other)
    {
        other.GetComponent<PlayerController>().toggleLook();
        oldCameraTransform = other.transform.GetChild(0).GetChild(2).transform;
        if (NetworkManager.Singleton.IsServer)
        {
            this.GetComponent<NetworkObject>().ChangeOwnership(other.GetComponent<NetworkObject>().OwnerClientId);
        }
        controlStir = false; // Initialize as false on enter
        stirCollider.enabled = true; // Enable collider for mouse tracking

        press.Enable();
        press.performed += ToggleStir;
        press.canceled += ToggleStir;
    }


    private void HandleIngredientEnter(Collider other)
    {
        if (ingredientCount == 0)
        {
            targetColor = other.GetComponent<MeshRenderer>().material.GetColor("_Color");
        }
        else
        {
            targetColor2 = other.GetComponent<MeshRenderer>().material.GetColor("_Color");
        }
        ingredientCount++;
        audioSource.clip = splashSFX[Random.Range(0, splashSFX.Length)];
        audioSource.Play();
        Destroy(other.gameObject);
    }

    private void HandleRecipeEnter(Collider other)
    {
        VariableDeclarations variables = Variables.Object(other.GetComponent<Variables>());
        Debug.Log(variables.Get("Recipe Rotation Order"));
        recipeTurnOrder = (List<string>)variables.Get("Recipe Rotation Order");
        if (recipeTurnOrder[recipeTurnIndex] == "Right")
        {
            rightTurnSprite.enabled = true;
        }
        else
        {
            rightTurnSprite.enabled = false;
        }
        Destroy(other.gameObject);
    }

    private void FixedUpdate()
    {
        Color newColor = Color.Lerp(couldronMeshRenderer.material.GetColor("_Color"), targetColor, colorTransitionSpeed * Time.deltaTime);
        couldronMeshRenderer.material.SetColor("_Color", newColor);

        newColor = Color.Lerp(couldronMeshRenderer.material.GetColor("_Color2"), targetColor2, colorTransitionSpeed * Time.deltaTime);
        couldronMeshRenderer.material.SetColor("_Color2", newColor);

        if (particles.enabled == true)
        {
            Debug.Log($"{couldronMeshRenderer.material.GetColor("_Color")} : {targetColor}");
            var couldronColor = couldronMeshRenderer.material.GetColor("_Color");
            if ((int)(couldronColor.r * 10) == (int)(targetColor.r * 10) &&
                (int)(couldronColor.g * 10) == (int)(targetColor.g * 10) &&
                (int)(couldronColor.b * 10) == (int)(targetColor.b * 10))
            {
                Debug.Log("DONE");
                targetColor = default1;
                targetColor2 = default2;
                counterClockwiseCount = 0;
                clockwiseCount = 0;
                SpawnPotionServerRPC();
                particles.enabled = false;


            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPotionServerRPC()
    {
        var potionObj = Instantiate(potion);
        var networkPotionObj = potionObj.GetComponent<NetworkObject>();

        networkPotionObj.Spawn();
        networkPotionObj.transform.position = potionSpawn.position;

        Transform newTransform = spawnPositions[Random.Range(0, spawnPositions.Length)];
        Vector3 position = newTransform.position;
        this.GetComponent<NetworkObject>().transform.position = position;

        potionSpawn = newTransform.GetChild(0).transform;

        // Optionally, you can call a method to update clients
        UpdateCouldronPositionClientRPC(position);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;



        playerCamera = other.transform.GetChild(0).transform.GetChild(2).GetComponent<Camera>();

        Vector3 lookDirection = focusTransform.position - playerCamera.transform.position;

        Vector3 cameraLookDirection = new Vector3(lookDirection.x, lookDirection.y, lookDirection.z);
        Vector3 playerLookDirection = new Vector3(0f, lookDirection.y, 0f);

        cameraLookDirection.Normalize();
        //playerLookDirection.Normalize();

        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, Quaternion.LookRotation(cameraLookDirection), cameraTrackingSpeed * Time.deltaTime);
        //other.transform.rotation = Quaternion.Slerp(other.transform.rotation, Quaternion.LookRotation(playerLookDirection), cameraTrackingSpeed * Time.deltaTime);


        if (controlStir)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = rayDistance;

            Ray ray = playerCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                if (hit.transform.CompareTag("StirObj"))
                {
                    Vector3 hitPoint = hit.point;
                    Vector3 origin = new Vector3(this.transform.position.x, stirObject.transform.position.y, this.transform.position.z);
                    hitPoint.y = stirObject.transform.position.y;

                    // Get angle of stir position to origin from -180 to 180
                    float newStirAngle = Vector3.SignedAngle(this.transform.forward, hitPoint - origin, Vector3.up);

                    // DeltaAngle negates the transition from -180 to 180
                    float angleChange = Mathf.DeltaAngle(currentStirAngle, newStirAngle);
                    angleSum += angleChange;
                    currentStirAngle = newStirAngle;

                    if (Vector3.Distance(origin, hitPoint) < maxStirRadius)
                    {
                        stirObject.transform.position = Vector3.Lerp(stirObject.transform.position, hitPoint, stirTrackingSpeed * Time.deltaTime);
                    }
                    else
                    {
                        Vector3 direction = (hitPoint - origin).normalized; // Get direction from origin to hit point
                        Vector3 edgePosition = origin + direction * maxStirRadius; // get edge position
                        stirObject.transform.position = Vector3.Lerp(stirObject.transform.position, edgePosition, stirTrackingSpeed * Time.deltaTime);
                    }

                    if (angleSum >= 360)
                    {
                        //Debug.Log("1 Clockwise Rotation");
                        Color temp = targetColor;
                        targetColor = targetColor2;
                        targetColor2 = temp;
                        clockwiseCount++;
                        angleSum = 0;
                        if (recipeTurnOrder[recipeTurnIndex] == "Right")
                        {
                            recipeTurnIndex++;
                        }
                    }
                    if (angleSum <= -360)
                    {
                        //Debug.Log("1 Counterclockwise Rotation");

                        Color temp = targetColor;
                        targetColor = targetColor2;
                        targetColor2 = temp;
                        counterClockwiseCount++;
                        angleSum = 0;
                        if (recipeTurnOrder[recipeTurnIndex] == "Left")
                        {
                            recipeTurnIndex++;
                        }
                    }

                    if (recipeTurnOrder.Count - 1 < recipeTurnIndex)
                    {
                        Color sum = (targetColor + targetColor2) / 2;
                        targetColor = sum;
                        targetColor2 = sum;
                        recipeTurnIndex = 0;

                        particles.enabled = true;
                        return;
                    }
                    if (recipeTurnOrder[recipeTurnIndex] == "Right")
                    {

                        rightTurnSprite.enabled = true;
                        leftTurnSprite.enabled = false;
                    }
                    else
                    {
                        rightTurnSprite.enabled = false;
                        leftTurnSprite.enabled = true;
                    }
                    Debug.Log(recipeTurnOrder[recipeTurnIndex]);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;
        other.GetComponent<PlayerController>().toggleLook();

        controlStir = false; // Set false on exit
        stirCollider.enabled = false;
    }

    private void ToggleStir(InputAction.CallbackContext context)
    {
        controlStir = !controlStir;
    }

    [ServerRpc]
    private void MoveCouldronServerRPC()
    {
        Debug.Log("MOVE COULDRON");
        Vector3 newPosition = spawnPositions[Random.Range(0, spawnPositions.Length)].position;
        this.GetComponent<NetworkObject>().transform.position = newPosition;

        // Optionally, you can call a method to update clients
        UpdateCouldronPositionClientRPC(newPosition);
    }

    [ClientRpc]
    private void UpdateCouldronPositionClientRPC(Vector3 newPosition)
    {
        this.transform.position = newPosition;
    }

}
