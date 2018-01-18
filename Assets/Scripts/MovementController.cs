using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController
{
    private Rigidbody rigidbody;
    private Transform transform;
    private float jumpSpeed = 300;
    private float walkSpeed = 3;
    private float maxSpeed = 5;

    public MovementController(Rigidbody rigidbody, Transform transform)
    {
        this.rigidbody = rigidbody;
        this.transform = transform;
    }

    public void Move(int forward, int left)
    {
        Vector3 movement = new Vector3(transform.right.x, 0, transform.right.z) * left + new Vector3(transform.forward.x, 0, transform.forward.z) * forward;
        if (left != 0 || forward != 0)
            movement.Normalize();
        movement *= walkSpeed;

        movement.y = rigidbody.velocity.y;
        rigidbody.velocity = movement;
    }

    public void Jump()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, 0.55f))
            rigidbody.AddForce(transform.up * jumpSpeed);
    }
}