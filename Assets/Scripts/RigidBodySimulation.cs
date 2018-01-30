using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyActorState : EmptyActorState
{
    public Vector3 position = new Vector3();
    public Vector3 velocity = new Vector3();

    public override void restore(GameObject actor, EmptyActorState previousState)
    {
        actor.transform.position = position;
        actor.GetComponent<Rigidbody>().velocity = velocity;
    }

    public override void save(GameObject actor, EmptyActorState previousState)
    {
        position = actor.transform.position;
        velocity = actor.GetComponent<Rigidbody>().velocity;
    }
}

public class RigidBodySimulation : EmptySimulation {
    public override EmptyActorState CreateNewState()
    {
        return new RigidBodyActorState();
    }
}
