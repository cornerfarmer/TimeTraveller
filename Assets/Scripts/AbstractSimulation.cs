using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbstractActorState
{
    public Vector3 position = new Vector3();
    public Vector3 velocity = new Vector3();

    public abstract void restore(GameObject actor, AbstractActorState previousState);
    public abstract void save(GameObject actor, AbstractActorState previousState);
}

public abstract class AbstractSimulation : MonoBehaviour
{
    public GameObject prefab;

    public abstract AbstractActorState CreateNewState();

    public abstract void Proceed(AbstractActorState state, ControlInput? input);

    public virtual GameObject CreateTimeFrame(GameObject actor, GameObject activeTimeFrames, int simPos, int maxTimeSteps, ControlInput? input)
    {
        GameObject block;
        if (activeTimeFrames.transform.childCount <= simPos)
        {
            block = GameObject.Instantiate(prefab, actor.transform.position - new Vector3(0, 0, (simPos + 1) * actor.transform.localScale.x), actor.transform.rotation, activeTimeFrames.transform);
            block.name = simPos.ToString();
        }
        else
        {
            block = activeTimeFrames.transform.GetChild(simPos).gameObject;
            block.GetComponent<TimeFrameController>().TransitToNewPosition(actor.transform.position - new Vector3(0, 0, (simPos + 1) * actor.transform.localScale.x), 0.5f + simPos / (maxTimeSteps * 2));
            block.transform.rotation = actor.transform.rotation;
        }

        for (int c = 0; c < block.transform.childCount; c++)
            GameObject.Destroy(block.transform.GetChild(c).gameObject);

        return block;
    }
}
