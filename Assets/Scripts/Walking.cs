using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Written by Dylan Madhani
public class Walking : MonoBehaviour
{

    private String decision;
    private Rigidbody pedestrian;
    private float speed = 4f;
    private char street;
    private Boolean active;
    private Boolean collide;
    private RaycastHit hit;
    // used to detect traffic signal state
    public SignalXFSM xSignal;
    public SignalZFSM zSignal;
    // used for animation (walking vs idle animation)
    public Animator animator;
    public RuntimeAnimatorController walk;
    public RuntimeAnimatorController idle;

    // sets default values and retrieves Rigidbody component
    void Start()
    {
        pedestrian = GetComponent<Rigidbody>();
        decision = "forward";
        active = false;
        collide = false;
        animator.runtimeAnimatorController = walk;
        street = 'x';
    }

    // detects decision and runs corresponding method
    private void Update()
    {

        DetectPedestrian();

        if (decision == "forward" && !collide)
        {
            DoForward();
        }
        if (decision == "left" && !collide)
        {
            TurnLeft();
        }
        if (decision == "cross" && !active && !collide)
        {
            StartCoroutine(Crosswalk());
        }


    }

    // decides state based on the tag of the surface the pedestrian is on
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.tag);

        // default forward movement
        if (collision.gameObject.tag == "walk")
        {
            decision = "forward";
        }
        if (collision.gameObject.tag == "crosswalk")
        {
            decision = "forward";
        }
        if (collision.gameObject.tag == "xSignal")
        {
            decision = "forward";
        }
        if (collision.gameObject.tag == "zSignal")
        {
            decision = "forward";
        }
        // turn left if at corner
        if (collision.gameObject.tag == "cornerWalk")
        {
            decision = "left";
        }
        // xWalk and zWalk are corners of crosswalk
        if (collision.gameObject.tag == "xWalk")
        {

            // walk across crosswalk if light is red and on correct crosswalk
            if (xSignal.signalState == "red" && street == 'x')
            {
                decision = "forward";
            }
            else if (xSignal.signalState == "green" && street == 'x')
            {
                decision = "cross";
                street = 'x';
            }
            street = 'x';
        } 
        if (collision.gameObject.tag == "zWalk")
        {

            // walk across crosswalk if light is red and on correct crosswalk
            if (zSignal.signalState == "red" && street == 'z')
            {
                decision = "forward";
            }
            else if (zSignal.signalState == "green" && street == 'z')
            {
                decision = "cross";
                street = 'z';
            }
            street = 'z';
        } 

    }

    // forward movement
    private void DoForward()
    {
        animator.runtimeAnimatorController = walk;
        pedestrian.freezeRotation = true;
        pedestrian.velocity = transform.forward * speed;
    }

    // left turn
    private void TurnLeft()
    {
        animator.runtimeAnimatorController = walk;
        pedestrian.freezeRotation = false;
        pedestrian.velocity = transform.forward * speed;
        Vector3 toRotate = new Vector3(0f, -90f, 0f);
        transform.Rotate(toRotate);
        decision = "forward";
    }

    // pedestrian stops movement at crosswalk
    IEnumerator Crosswalk()
    {
        active = true;
        pedestrian.velocity = Vector3.zero;
        animator.runtimeAnimatorController = idle;
        yield return new WaitForSeconds(1);
        // changes state and animation if light turns red
        if (street == 'x')
        {
            if (xSignal.signalState == "red")
            {
                decision = "forward";
                animator.runtimeAnimatorController = walk;
            }
        }
        if (street == 'z')
        {
            if (zSignal.signalState == "red")
            {
                decision = "forward";
                animator.runtimeAnimatorController = walk;
            }
        }

        active = false;
    }

    // raycasting implemented to detect other pedestrians
    private void DetectPedestrian()
    {

        Vector3 currentPosition = transform.position;
        currentPosition.y += 1;
        Vector3 ray1 = Quaternion.Euler(0, 0, 0) * transform.forward;
        Debug.DrawRay(currentPosition, ray1 * 0.5f, Color.red);

        if (Physics.Raycast(currentPosition, ray1, out hit, 0.5f))
        {
            // if ray hits pedestrian, stop movement, set animation to idle
            if (hit.collider.CompareTag("pedestrian"))
            {
                //Debug.Log("hit");
                collide = true;
                pedestrian.velocity = Vector3.zero;
                animator.runtimeAnimatorController = idle;
            }

        }
        else
        {
            collide = false;
        }

    }


}
