using UnityEngine;
using UnityEngine.InputSystem;
using Color = UnityEngine.Color;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Unity.Netcode;

public class PotionMixer : NetworkBehaviour
{
    [Header("Camera")]
    public Transform focusTransform;
    [Range(0.1f, 4f)] public float cameraTrackingSpeed = 1.5f;
    private Camera playerCamera;

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

    }

    private void HandlePlayerEnter(Collider other)
    {
        other.GetComponent<PlayerController>().toggleLook();

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


    private void HandleIngredientEnter( Collider other)
    {
        if( ingredientCount == 0 )
        {
            targetColor = other.GetComponent<MeshRenderer>().material.GetColor("_Color");
        }
        else
        {
            targetColor2 = other.GetComponent<MeshRenderer>().material.GetColor("_Color");
        }
        ingredientCount++;
        Destroy(other.gameObject);
    }

    private void FixedUpdate()
    {
        Color newColor = Color.Lerp(couldronMeshRenderer.material.GetColor("_Color"), targetColor, colorTransitionSpeed * Time.deltaTime);
        couldronMeshRenderer.material.SetColor("_Color", newColor);

        newColor = Color.Lerp(couldronMeshRenderer.material.GetColor("_Color2"), targetColor2, colorTransitionSpeed * Time.deltaTime);
        couldronMeshRenderer.material.SetColor("_Color2", newColor);

        if (clockwiseCount >= 2 && counterClockwiseCount >= 1)
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
                
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPotionServerRPC()
    {
        var potionObj = Instantiate(potion);
        var networkPotionObj = potionObj.GetComponent<NetworkObject>();
        networkPotionObj.Spawn();
        networkPotionObj.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 2, this.transform.position.z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        

        playerCamera = other.transform.GetChild(0).GetComponent<Camera>();

        Vector3 lookDirection = focusTransform.position - playerCamera.transform.position;
        lookDirection.Normalize();

        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, Quaternion.LookRotation(lookDirection), cameraTrackingSpeed * Time.deltaTime);

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
                    float angleChange = Mathf.DeltaAngle(currentStirAngle, newStirAngle );
                    angleSum += angleChange;
                    currentStirAngle = newStirAngle;

                    if ( Vector3.Distance(origin, hitPoint) < maxStirRadius)
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

                    }
                    if (angleSum <= -360)
                    {
                        //Debug.Log("1 Counterclockwise Rotation");

                        Color temp = targetColor;
                        targetColor = targetColor2;
                        targetColor2 = temp;
                        counterClockwiseCount++;
                        angleSum = 0;
                        
                    }

                    if (clockwiseCount >= 2 && counterClockwiseCount >= 1)
                    {
                        Color sum = (targetColor + targetColor2) / 2;
                        targetColor = sum;
                        targetColor2 = sum;
                    }

                    // TODO: Write an update color function with a target color param
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
}
