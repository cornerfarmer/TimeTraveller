using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinActorState : EmptyActorState
{
    public Quaternion rotation = Quaternion.identity;
    public bool visible = true;

    public override void restore(GameObject actor, EmptyActorState previousState)
    {
        actor.transform.rotation = rotation;
        actor.SetActive(visible);
    }

    public override void save(GameObject actor, EmptyActorState previousState)
    {
        rotation = actor.transform.rotation;
        visible = actor.activeSelf;
    }
}

public class CoinSimulator : EmptySimulation
{
    public override EmptyActorState CreateNewState()
    {
        return new CoinActorState();
    }

    public override void Proceed(EmptyActorState state, ControlInput? input, PlayerActorState playerState)
    {
        base.Proceed(state, input, playerState);
        transform.Rotate(0, 10f, 0);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            gameObject.SetActive(false);
            lastPlayerState.coins++;
        }
    }
    
}
