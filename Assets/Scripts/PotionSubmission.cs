using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSubmission : MonoBehaviour
{
    public static string potionName;

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == potionName)
        {
            Debug.Log("Successful Submission");
        }
    }
}
