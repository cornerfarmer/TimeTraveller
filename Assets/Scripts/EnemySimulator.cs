using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySimulator : RigidBodySimulation
{
    private Rigidbody rigidbody;
    public bool forward = false;
    private MovementController movementController;

    // Use this for initialization
    void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
        movementController = new MovementController(rigidbody, transform);
    }

    public override void Proceed(AbstractActorState state, ControlInput? input)
    {
        movementController.Move(forward ? 1 : -1, 0);
    }
}
