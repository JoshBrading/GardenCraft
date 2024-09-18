using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCan : MonoBehaviour
{
    public GameObject waterHitBox;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            waterHitBox.SetActive(true);
            Debug.Log("I'm literally pressing R");
            StartCoroutine(Unwater());
        }
    }

    IEnumerator Unwater()
    {
        yield return new WaitForSeconds(0.5f);
        waterHitBox.SetActive(false);
    }
}
