using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class CrowSpawner : NetworkBehaviour
{
    // Adjust the speed for the application.
    public float speed = 1.0f;

    // The target (cylinder) position.

    public Animation anim;

    void Update()
    {
        
        Transform target = GameObject.FindWithTag("Ingredient").transform;
        if (target != null)
        {
            Debug.Log(target);
        }
        else
        {
            Debug.Log("No GameObject called example GameObject");
        }


        // Move our position a step closer to the target.
        var step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        // Check if the position of the cube and sphere are approximately equal.
        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            // Swap the position of the cylinder.
            target.position *= -1.0f;
        }
    }

    private void OnCollision(Collision collision)
    {
        
       // if (collision.gameObject.CompareTag("Ingredient"))
       // {
       //     Debug.Log("I'm touching your eggplant");
       //     this.anim.Play();
       //     //collision.gameObject.SetActive(false);
       //     Destroy(collision.gameObject);
       //     Destroy(gameObject);
       //     //StartCoroutine(CrowDrop());
       // }

        if (collision.gameObject.CompareTag("Shovel"))
        {
            Debug.Log("WHACK EM");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shovel"))
        {
            Debug.Log("WHACK EM");
            gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Ingredient"))
        {
            Debug.Log("I'm touching your eggplant");
            this.anim.Play();
            //collision.gameObject.SetActive(false);
            Destroy(other.gameObject);
            Destroy(gameObject);
            //StartCoroutine(CrowDrop());
        }
    }

    IEnumerator CrowDrop()
    {
        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
    }
}
