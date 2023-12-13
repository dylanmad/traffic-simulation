using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Dylan Madhani
// traffic light controller along the z-axis
public class SignalZFSM : MonoBehaviour
{
    public Material objectColor;
    public String signalState;
    // lock for Coroutine
    private Boolean active;

    // get material component, set initial values
    void Start()
    {
        objectColor = GetComponent<Renderer>().material;
        active = false;
        signalState = "red";
    }

    // continously run coroutine. 'active' prevents coroutine from running every frame
    void Update()
    {
        if (!active)
        {
            StartCoroutine(SignalChange());
        }
    }

    // controls traffic light color and state
    IEnumerator SignalChange()
    {
        active = true;

        // red light
        objectColor.color = Color.red;
        signalState = "red";
        yield return new WaitForSeconds(10);

        // green light
        objectColor.color = Color.green;
        signalState = "green";
        yield return new WaitForSeconds(8);

        // yellow light
        objectColor.color = Color.yellow;
        yield return new WaitForSeconds(2);

        active = false;

    }
}
