using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CrowSpawner : MonoBehaviour
{
    // Adjust the speed for the application.
    public float speed = 1.0f;

    // The target (cylinder) position.

    public Animation anim;

    void Update()
    {
        
        Transform target = GameObject.FindWithTag("Ingredient").transform;
        if (target)
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
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ingredient"))
        {
            Debug.Log("I'm touching your eggplant");
            this.anim.Play();
            StartCoroutine(CrowDrop());
        }

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
    }

    IEnumerator CrowDrop()
    {
        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
    }
}
