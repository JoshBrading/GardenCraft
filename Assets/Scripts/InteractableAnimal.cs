using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class InteractableAnimal : NetworkBehaviour
{
    public Transform spawn;
    public GameObject[] recipes;
    private GameObject recipe;

    //Time
    public float timeRemaining = 0;
    public bool timeIsRunning = true;

    public int potionCount;

    public AudioSource audioSource;
    public AudioClip sfx;

    public GameObject Canvas;
    public GameObject potion1;
    public GameObject potion2;
    public GameObject potion3;

    public Vector3 startPosition;
    public Vector3 idlePosition;
    public Vector3 endPosition;

    public GameObject nextBunny;

    private Vector3 targetPosition;

    public bool start = false;

    // Start is called before the first frame update
    void Start()
    {
        recipe = recipes[Random.Range(0, recipes.Length)];
        timeIsRunning = true;

        targetPosition = idlePosition;
        this.transform.position = startPosition;

        //audioSource.Play();
    }


    private void FixedUpdate()
    {
        if ((int)(this.transform.position.x) == (int)(endPosition.x) &&
               (int)(this.transform.position.y) == (int)(endPosition.y) &&
               (int)(this.transform.position.z) == (int)(endPosition.z))

        {
            nextBunny.GetComponent<InteractableAnimal>().start = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if( collision.gameObject.CompareTag("Potion"))
        {
            Destroy(collision.gameObject);
            potionCount ++;
            CheckIfDone();
        }
    }


    public void OnInteract()
    {
        OnInteractServerRPC();
    }

    [ServerRpc]
    public void OnInteractServerRPC()
    {
        var recipeObj = Instantiate(recipe);
        var networkRecipeObj = recipeObj.GetComponent<NetworkObject>();
        networkRecipeObj.SpawnWithOwnership(GetNonHostPlayerId());
        networkRecipeObj.transform.position = spawn.position;
    }

    ulong GetNonHostPlayerId()
    {
        ulong hostId = NetworkManager.Singleton.LocalClientId;

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key != hostId)
            {
                return client.Key;
            }
        }

        return 0;
    }

    private void Update()
    {
        if (timeIsRunning)
        {
            if (timeRemaining >= 0)
            {
                timeRemaining += Time.deltaTime;
                DisplayTime(timeRemaining);
            }
        }
        if (start)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, 0.5f * Time.deltaTime);
            audioSource.enabled = true;
        }


    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
    }

    void CheckIfDone()
    {
        if (potionCount == 3)
        {

            targetPosition = endPosition;
            Canvas.SetActive(true);
            if( NetworkManager.Singleton.IsHost)
                showEndClientRPC();
            if(timeRemaining <= 90)
            {
                potion1.SetActive(true);
                potion2.SetActive(true);
                potion3.SetActive(true);
            }
            else if (timeRemaining <= 180)
            {
                potion1.SetActive(true);
                potion2.SetActive(true);
            }
            else if(timeRemaining <= 240)
            {
                potion1.SetActive(true);
            }
            else
            {
                Debug.Log("Maybe");
            }
        }
    }

    [ClientRpc]
    private void showEndClientRPC()
    {
        Canvas.SetActive(true);
    }

    public void playSound()
    {
        //audioSource.Play();
    }
}
