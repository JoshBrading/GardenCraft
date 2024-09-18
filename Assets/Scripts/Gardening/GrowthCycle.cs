using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthCycle : MonoBehaviour
{
    public float growthStart;
    public float growthMid;
    public float growthMax;
    public float growFactor;
    public float waitTime;
    public static bool needsWater;
    public GameObject waterInd;
    void Start()
    {
        needsWater = true;
        waterInd.SetActive(true);
        //StartCoroutine(Scale());
    }

    private void Update()
    {
        if (needsWater == false)
        {
            Debug.Log("I am watered");
        }

        if (Input.GetMouseButtonDown(0))
        {
            needsWater = false;
        }
    }

    private void Growing()
    {
        for (int i = 0; i < growthMax; i++)
        {
            if (needsWater == false)
            {
                StartCoroutine(Scale());
            }
            i++;
        }
    }
    IEnumerator Scale()
    {
        float timer = 0;

        while (true)
        {
            waterInd.SetActive(false);
            // We scale axis, so they will have the same value,
            // so we can work with a float instand of comparing vectors
            while (growthStart > transform.localScale.x)
            {


                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;

            }
            needsWater = true;
            waterInd.SetActive(true);
            StartCoroutine(Scale2());
            StopCoroutine(Scale());
            yield return new WaitForSeconds(waitTime);

        }
    }

    IEnumerator Scale2()
    {
        float timer = 0;

        yield return new WaitForSeconds(1);

        while (needsWater == false)
        {
            waterInd.SetActive(false);
            // We scale axis, so they will have the same value,
            // so we can work with a float instand of comparing vectors
            while (growthMid > transform.localScale.x)
            {


                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;

            }
            needsWater = true;
            waterInd.SetActive(true);
            StartCoroutine(Scale3());
            StopCoroutine(Scale2());
            yield return null;


        }
    }

    IEnumerator Scale3()
    {
        float timer = 0;

        yield return new WaitForSeconds(1);

        while (needsWater == false)
        {
            waterInd.SetActive(false);
            // We scale axis, so they will have the same value,
            // so we can work with a float instand of comparing vectors
            while (growthMax > transform.localScale.x)
            {


                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                Debug.Log("I am fully grown");

            }
            StopCoroutine(Scale3());
            yield return null;

        }
    }
}
