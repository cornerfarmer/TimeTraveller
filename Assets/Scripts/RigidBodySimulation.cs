using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyActorState : AbstractActorState
{
    public Vector3 position = new Vector3();
    public Vector3 velocity = new Vector3();

    public override void restore(GameObject actor, AbstractActorState previousState)
    {
        actor.transform.position = position;
        actor.GetComponent<Rigidbody>().velocity = velocity;
    }

    public override void save(GameObject actor, AbstractActorState previousState)
    {
        position = actor.transform.position;
        velocity = actor.GetComponent<Rigidbody>().velocity;
    }
}

public class RigidBodySimulation : AbstractSimulation {
    public override AbstractActorState CreateNewState()
    {
        return new RigidBodyActorState();
    }

    public override void Proceed(AbstractActorState state, ControlInput? input)
    {
        
    }
}
