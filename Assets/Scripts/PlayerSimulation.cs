using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorState : RigidBodyActorState
{
    public ControlInput movementMode = ControlInput.Stop;

    public override void restore(GameObject actor, AbstractActorState previousState)
    {
        base.restore(actor, previousState);
        movementMode = previousState != null ? (previousState as PlayerActorState).movementMode : ControlInput.Stop;
    }

    public override void save(GameObject actor, AbstractActorState previousState)
    {
        base.save(actor, previousState);
        movementMode = previousState != null ? (previousState as PlayerActorState).movementMode : ControlInput.Stop;
    }
}

public class PlayerSimulation : RigidBodySimulation {
    private Rigidbody rigidbody;
    private float jumpSpeed = 300;
    private float walkSpeed = 3;
    private float maxSpeed = 5;
    private MovementController movementController;
    public GameObject arrowPrefab;
    public GameObject arrowStopPrefab;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        movementController = new MovementController(rigidbody, transform);
    }

    public override AbstractActorState CreateNewState()
    {
        return new PlayerActorState();
    }

    public override void Proceed(AbstractActorState state, ControlInput? input)
    {
        PlayerActorState playerState = state as PlayerActorState;
        
        if (input != null)
        {
            if (input == ControlInput.Jump)
                movementController.Jump();
            else
                playerState.movementMode = input.Value;
        }

        movementController.Move(playerState.movementMode == ControlInput.Forward ? 1 : (playerState.movementMode == ControlInput.Backward ? -1 : 0), 0);
    }

    public override GameObject CreateTimeFrame(GameObject actor, GameObject activeTimeFrames, int simPos, int maxTimeSteps, ControlInput? input)
    {
        GameObject block = base.CreateTimeFrame(actor, activeTimeFrames, simPos, maxTimeSteps, input);

        if (input != null)
        {
            block.GetComponent<TimeFrameController>().SetHasAction(true);

            GameObject arrow = GameObject.Instantiate(input.Value == ControlInput.Stop ? arrowStopPrefab : arrowPrefab, block.transform);
            arrow.transform.localScale = new Vector3(1, 0.25f, 0.25f);
            if (input.Value == ControlInput.Forward)
                arrow.transform.localEulerAngles = new Vector3(90, 0, 0);
            else if (input.Value == ControlInput.Backward)
                arrow.transform.localEulerAngles = new Vector3(-90, 0, 0);
        }
        else
        {
            block.GetComponent<TimeFrameController>().SetHasAction(false);
        }
        return block;
    }
}
