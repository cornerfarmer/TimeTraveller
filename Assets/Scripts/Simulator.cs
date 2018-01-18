using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;


public enum ControlInput
{
    Jump,
    Forward,
    Backward,
    Stop
}

public class Simulator
{
    private GameObject activeTimeFrames;
    private GameObject player;
    private GameObject finish;
    private MovementController movementController;
    private Dictionary<int, ControlInput> controlInputs;
    private Vector3 startPos;
    private int maxTimeSteps;
    private float timePerTimeStep;
    private int simulationsPerTimeStep;
    public float runTime;
    private GameObject[] actors;
    private State[] states;
    private int simPos;
    
    class State
    {
        public Dictionary<GameObject, AbstractActorState> actorStates;

        public State(GameObject[] actors)
        {
            actorStates = new Dictionary<GameObject, AbstractActorState>();
            foreach (GameObject actor in actors)
            {
                actorStates.Add(actor, actor.GetComponent<AbstractSimulation>().CreateNewState());
            }
        }

        public void Save(GameObject[] actors, State previousState)
        {
            foreach (GameObject actor in actors)
            {
                actorStates[actor].save(actor, previousState == null ? null : previousState.actorStates[actor]);
            }
        }

        public void Restore(GameObject[] actors, State previousState)
        {
            foreach (GameObject actor in actors)
            {
                actorStates[actor].restore(actor, previousState == null ? null : previousState.actorStates[actor]);
            }
        }
    }

    public Simulator(GameObject activeTimeFrames, GameObject player, GameObject finish, int maxTimeSteps)
    {
        this.activeTimeFrames = activeTimeFrames;
        this.player = player;
        this.finish = finish;
        controlInputs = new Dictionary<int, ControlInput>();

        this.maxTimeSteps = maxTimeSteps;
        timePerTimeStep = 0.02f;
        simulationsPerTimeStep = 3;
        runTime = 0;
        actors = GameObject.FindGameObjectsWithTag("Actor");

        states = new State[maxTimeSteps];
        for (int i = 0; i < maxTimeSteps; i++)
            states[i] = new State(actors);

        states[0].Save(actors, null);
    }

    public void SetInput(int timePos, ControlInput input)
    {
        if (input == ControlInput.Backward || input == ControlInput.Forward)
        {
            bool isStop = false;
            for (int i = timePos - 1; i >= 0; i--)
            {
                if (controlInputs.ContainsKey(i))
                {
                    if (controlInputs[i] == input)
                    {
                        isStop = true;
                        break;
                    }
                    else if (controlInputs[i] == ControlInput.Backward || controlInputs[i] == ControlInput.Forward || controlInputs[i] == ControlInput.Stop)
                    {
                        break;
                    }
                }
            }
            if (isStop)
                input = ControlInput.Stop;
        }

        if (controlInputs.ContainsKey(timePos) && controlInputs[timePos] == input)
            controlInputs.Remove(timePos);
        else
            controlInputs[timePos] = input;
       
        Simulate(timePos);
    }

    public void Simulate(int start)
    {
        simPos = Mathf.Min(simPos, start);
    }

    public void SimulateNextBatch()
    {
        if (simPos < maxTimeSteps)
        {
            runTime = 0;
            GameObject.Find("Viewer").GetComponent<Rigidbody>().isKinematic = true;

            states[simPos].Restore(actors, simPos > 0 ? states[simPos - 1] : null);

            int endPos = Mathf.Min(maxTimeSteps, simPos + 10);
            Physics.autoSimulation = false;
            for (; simPos < endPos; simPos++)
            {
                State state = states[simPos];

                foreach (GameObject actor in actors)
                {
                    actor.GetComponent<AbstractSimulation>().Proceed(state.actorStates[actor], controlInputs.ContainsKey(simPos) ? controlInputs[simPos] : (ControlInput?) null);
                }

                for (int t = 0; t < simulationsPerTimeStep; t++)
                    Physics.Simulate(timePerTimeStep);

                if (runTime == 0 && player.transform.position.x > finish.transform.position.x && player.transform.position.y > finish.transform.position.y - 3)
                    runTime = simPos * simulationsPerTimeStep * timePerTimeStep;

                foreach (GameObject actor in actors)
                {
                    Transform pool = activeTimeFrames.transform.Find(actor.name + "Pool");
                    GameObject poolGameObject;
                    if (pool != null)
                        poolGameObject = pool.gameObject;
                    else
                    {
                        poolGameObject = new GameObject(actor.name + "Pool");
                        poolGameObject.transform.parent = activeTimeFrames.transform;
                    }
                    actor.GetComponent<AbstractSimulation>().CreateTimeFrame(actor, poolGameObject, simPos, maxTimeSteps, controlInputs.ContainsKey(simPos) ? controlInputs[simPos] : (ControlInput?)null);
                }
               
                
                if (simPos + 1 < maxTimeSteps)
                {
                    states[simPos + 1].Save(actors, state);
                }
            }
            Physics.autoSimulation = true;

            states[0].Restore(actors, null);

            GameObject.Find("Viewer").GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
