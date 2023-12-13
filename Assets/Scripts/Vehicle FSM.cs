using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// Written by Dylan Madhani
public class VehicleFSM : MonoBehaviour
{

    private Rigidbody vehicle;
    private string decision;
    private string turn;
    private Boolean collide;
    private RaycastHit hit;
    float speed = 10f;
    float random;
    Boolean active;
    char street;
    // used for traffic signal state detection
    public SignalXFSM xSignal;
    public SignalZFSM zSignal;



    // gets Rigidbody component and sets initial values
    void Start()
    {
        random = UnityEngine.Random.Range(-1f, 1f);
        decision = "forward";
        collide = false;
        turn = "default";
        vehicle = GetComponent<Rigidbody>();
        active = false;
    }

    // decides which method to run depending on 'decision' value and if car is colliding with another car
    private void Update()
    {

        DetectVehicle();

        if (decision == "forward" && !collide)
        {
            DoForward();
        }
        if (decision == "left" && !collide)
        {
            TurnLeft();
        }
        if (decision == "right" && !collide)
        {
            TurnRight();
        }
        // 'active' prevents the coroutine from running every frame
        if (decision == "wait" && !collide && !active)
        {
            StartCoroutine(RedLight());
        }



    }

    // sets 'decision' variable based on street tag which the vehicle interacts with
    private void OnCollisionEnter(Collision collision)
    {

        // default forward movement
        if (collision.gameObject.tag == "street")
        {
            decision = "forward";
        }
        // random left/forward turn
        if (collision.gameObject.tag == "leftForward")
        {
            random = UnityEngine.Random.Range(-1f, 1f);

            if (random > 0)
            {
                decision = "left";
                turn = "left";
            }
            else
            {
                decision = "forward";
                turn = "default";
            }

        }
        // random right/forward turn
        if (collision.gameObject.tag == "rightForward")
        {
            random = UnityEngine.Random.Range(-1f, 1f);

            if (random > 0 && turn != "left")
            {
                decision = "right";
                turn = "right";
            }
            else
            {
                decision = "forward";
                turn = "default";
            }

        }
        // left turn
        if (collision.gameObject.tag == "left")
        {
            decision = "left";
        }
        // right turn
        if (collision.gameObject.tag == "right")
        {
            decision = "right";
        }
        // stop sign
        if (collision.gameObject.tag == "stop")
        {
            StartCoroutine(StopSign());
        }
        // x-axis traffic signal
        if (collision.gameObject.tag == "xSignal")
        {
            if (xSignal.signalState == "green")
            {
                decision = "forward";
            } else if (xSignal.signalState == "red")
            {
                decision = "wait";
                street = 'x';
            }

        }
        // z-axis traffic signal
        if (collision.gameObject.tag == "zSignal")
        {
            if (zSignal.signalState == "green")
            {
                decision = "forward";
            }
            else if (zSignal.signalState == "red")
            {
                decision = "wait";
                street = 'z';
            }

        }


    }

    // uses ray casting to detect vehicles and then stops if detected
    private void DetectVehicle()
    {

        Vector3 currentPosition = transform.position;
        currentPosition.y += 1f;
        Vector3 ray1 = Quaternion.Euler(0, -20, 0) * transform.forward;
        Debug.DrawRay(currentPosition, ray1 * 5f, Color.red);
        Vector3 ray2 = Quaternion.Euler(0, 0, 0) * transform.forward;
        Debug.DrawRay(currentPosition, ray2 * 7f, Color.red);
        Vector3 ray3 = Quaternion.Euler(0, 20, 0) * transform.forward;
        Debug.DrawRay(currentPosition, ray3 * 5f, Color.red);


        if (Physics.Raycast(currentPosition, ray1, out hit, 5f) || Physics.Raycast(currentPosition, ray2, out hit, 7f) || Physics.Raycast(currentPosition, ray3, out hit, 5f))
        {
            if (hit.collider.CompareTag("vehicle"))
            {
                collide = true;
                vehicle.velocity = Vector3.zero;
            }

        }
        else
        {
            collide = false;
        }

    }

    // forward movement
    private void DoForward()
    {
        vehicle.freezeRotation = true;
        vehicle.velocity = transform.forward * speed;
    }

    // rotate the car to the left
    private void TurnLeft()
    {
        vehicle.freezeRotation = false;
        Vector3 toRotate = new Vector3(0f, -90f, 0f);
        transform.Rotate(toRotate);
        decision = "forward";
    }

    // rotate the car to the right
    private void TurnRight()
    {
        vehicle.freezeRotation = false;
        Vector3 toRotate = new Vector3(0f, 90f, 0f);
        transform.Rotate(toRotate);
        decision = "forward";
    }

    // stops at a stop sign for 3 seconds
    IEnumerator StopSign()
    {
        decision = "stop";
        vehicle.velocity = Vector3.zero;
        yield return new WaitForSeconds(3);
        decision = "forward";
    }

    // stops at a red light for 1 second then reevaluates traffic signal state
    IEnumerator RedLight()
    {
        active = true;
        vehicle.velocity = Vector3.zero;
        yield return new WaitForSeconds(1);

        // evaluates if the light is green, and if so the vehicle state is set to forward
        if (street == 'x')
        {
            if (xSignal.signalState == "green")
            {
                decision = "forward";
            }
        }
        if (street == 'z')
        {
            if (zSignal.signalState == "green")
            {
                decision = "forward";
            }
        }

        active = false;
    }


}
