using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber : MonoBehaviour
{
    public float gravity = 45.0f;
    public float sensitivity = 45.0f;

    public Hand currentHand = null;
    //private CharacterController controller = null;

    private Rigidbody rb;

    private void Awake()
    {
        //controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        if (currentHand == null)
            Debug.Log("its null");

        Vector3 movement = Vector3.zero;

        if (currentHand)
        {
            movement += currentHand.Delta * sensitivity;
        }

        if (movement == Vector3.zero)
            movement.y -= gravity * Time.deltaTime;

        //transform.position += movement * Time.deltaTime;
        //rb.MovePosition
        // Moves the player.
        //controller.Move(movement * Time.deltaTime);
        //rb.MovePosition(new Vector3(0f, 0f, 1f));
        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f);
    }

    public void SetHand(Hand hand)
    {
        if (currentHand)
        {
            currentHand.ReleasePoint();
        }

        currentHand = hand;
    }

    public void ClearHand(Hand hand)
    {
        currentHand = null;
    }
}