using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EmptyActorState
{
    public virtual void restore(GameObject actor, EmptyActorState previousState)
    {
        
    }

    public virtual void save(GameObject actor, EmptyActorState previousState)
    {
        
    }
}

public class EmptySimulation : MonoBehaviour
{
    public GameObject prefab;
    protected PlayerActorState lastPlayerState;

    public virtual EmptyActorState CreateNewState()
    {
        return new EmptyActorState();
    }

    public virtual void Proceed(EmptyActorState state, ControlInput? input, PlayerActorState playerState)
    {
        lastPlayerState = playerState;
    }

    public virtual GameObject CreateTimeFrame(GameObject actor, GameObject activeTimeFrames, int simPos, int maxTimeSteps, ControlInput? input)
    {
        GameObject block;
        float zStep = 0.25f;
        if (activeTimeFrames.transform.childCount <= simPos)
        {
            block = GameObject.Instantiate(prefab, actor.transform.position - new Vector3(0, 0, (simPos + 1) * zStep), actor.transform.rotation, activeTimeFrames.transform);
            block.name = simPos.ToString();
        }
        else
        {
            block = activeTimeFrames.transform.GetChild(simPos).gameObject;
            block.GetComponent<TimeFrameController>().TransitToNewPosition(actor.transform.position - new Vector3(0, 0, (simPos + 1) * zStep), 0.5f + simPos / (maxTimeSteps * 2));
            block.transform.rotation = actor.transform.rotation;
        }
        block.SetActive(actor.activeSelf);

        for (int c = 0; c < block.transform.childCount; c++)
            GameObject.Destroy(block.transform.GetChild(c).gameObject);

        return block;
    }
}
