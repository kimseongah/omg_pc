using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public Transform aimTarget; // the target where we aim to land the shuttlecock
    float speed = 3f; // move speed
    float hitForce = 15f; // shuttlecock impact force
    float upForce = 10f; // upward force to create a parabolic trajectory

    bool isHitting; // boolean to know if we are hitting the shuttlecock or not 

    public Transform shuttlecock; // the shuttlecock 
    Animator animator;

    Vector3 aimTargetInitialPosition; // initial position of the aiming gameObject

    private void Start()
    {
        animator = GetComponent<Animator>(); // reference our animator
        aimTargetInitialPosition = aimTarget.position; // initialise the aim position
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal2"); // get the horizontal axis of the keyboard
        float v = Input.GetAxisRaw("Vertical2"); // get the vertical axis of the keyboard

        if (Input.GetKeyDown(KeyCode.F)) 
        {
            isHitting = true; // we are trying to hit the shuttlecock
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            isHitting = false;
        }

        if (isHitting)  // if we are trying to hit the shuttlecock
        {
            aimTarget.Translate(new Vector3(h, 0, 0) * speed * 2 * Time.deltaTime); //translate the aiming gameObject on the court horizontally
        }
        else if ((h != 0 || v != 0)) // if we want to move and we are not hitting the shuttlecock
        {
            transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime); // move on the court
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) // if we collide with the shuttlecock 
        {
            Vector3 dir = aimTarget.position - transform.position; // get the direction to where we want to send the shuttlecock
            Vector3 force = dir.normalized * hitForce + Vector3.up * upForce; // adding upward force for a parabolic trajectory

            other.GetComponent<Rigidbody>().velocity = force;

            Vector3 shuttlecockDir = shuttlecock.position - transform.position;
            if (shuttlecockDir.x >= 0)                                   
            {
                animator.Play("forehand");                        
            }
            else                                                  
            {
                animator.Play("backhand");
            }

            aimTarget.position = aimTargetInitialPosition; // reset the position of the aiming gameObject to its original position
        }
    }
}
