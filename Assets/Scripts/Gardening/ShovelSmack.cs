using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelSmack : MonoBehaviour
{
    public GameObject shovel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            shovel.SetActive(true);
            StartCoroutine(Unshovel());
        }
    }

    IEnumerator Unshovel()
    {
        yield return new WaitForSeconds(0.7f);
        shovel.SetActive(false);
    }
}
