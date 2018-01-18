using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Simulator
{
    private GameObject prefab;
    private GameObject arrowPrefab;
    private GameObject arrowStopPrefab;
    private GameObject activeTimeFrames;
    private GameObject player;
    private GameObject finish;
    private Rigidbody rigidbody;
    private MovementController movementController;
    private Dictionary<int, ControlInput> controlInputs;
    private Vector3 startPos;
    private int maxTimeSteps;
    private float timePerTimeStep;
    private int simulationsPerTimeStep;
    public float runTime;
    private GameObject[] enemies;
    private State[] states;
    private int simPos;
    class State
    {
        public Vector3 position = new Vector3();
        public Vector3 velocity = new Vector3();
        public ControlInput movementMode = ControlInput.Stop;
    }

    public enum ControlInput
    {
        Jump,
        Forward,
        Backward,
        Stop
    }

    public Simulator(GameObject prefab, GameObject arrowPrefab, GameObject arrowStopPrefab, GameObject activeTimeFrames, GameObject player, GameObject finish, int maxTimeSteps)
    {
        this.prefab = prefab;
        this.arrowPrefab = arrowPrefab;
        this.arrowStopPrefab = arrowStopPrefab;
        this.activeTimeFrames = activeTimeFrames;
        this.player = player;
        this.finish = finish;
        rigidbody = player.GetComponent<Rigidbody>();
        movementController = new MovementController(rigidbody, player.transform);
        controlInputs = new Dictionary<int, ControlInput>();

        this.maxTimeSteps = maxTimeSteps;
        timePerTimeStep = 0.02f;
        simulationsPerTimeStep = 3;
        runTime = 0;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        states = new State[maxTimeSteps];
        for (int i = 0; i < maxTimeSteps; i++)
            states[i] = new State();

        states[0].position = player.transform.position;
        states[0].velocity = rigidbody.velocity;
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

            player.transform.position = states[simPos].position;
            rigidbody.velocity = states[simPos].velocity;
            states[simPos].movementMode = simPos > 0 ? states[simPos - 1].movementMode : ControlInput.Stop;

            int endPos = Mathf.Min(maxTimeSteps, simPos + 10);
            Physics.autoSimulation = false;
            for (; simPos < endPos; simPos++)
            {
                State state = states[simPos];

                if (runTime == 0)
                {
                    if (controlInputs.ContainsKey(simPos))
                    {
                        if (controlInputs[simPos] == ControlInput.Jump)
                            movementController.Jump();
                        else
                            state.movementMode = controlInputs[simPos];
                    }

                    movementController.Move(state.movementMode == ControlInput.Forward ? 1 : (state.movementMode == ControlInput.Backward ? -1 : 0), 0);
                }

                for (int t = 0; t < simulationsPerTimeStep; t++)
                    Physics.Simulate(timePerTimeStep);

                if (runTime == 0 && player.transform.position.x > finish.transform.position.x && player.transform.position.y > finish.transform.position.y - 2)
                    runTime = simPos * simulationsPerTimeStep * timePerTimeStep;

                GameObject block;
                if (activeTimeFrames.transform.childCount <= simPos)
                {
                    block = GameObject.Instantiate(prefab, player.transform.position - new Vector3(0, 0, (simPos + 1) * player.transform.localScale.x), player.transform.rotation, activeTimeFrames.transform);
                    block.name = simPos.ToString();
                }
                else
                {
                    block = activeTimeFrames.transform.GetChild(simPos).gameObject;
                    block.GetComponent<PlayerController>().TransitToNewPosition(player.transform.position - new Vector3(0, 0, (simPos + 1) * player.transform.localScale.x), 0.5f + simPos / (maxTimeSteps * 2));
                    block.transform.rotation = player.transform.rotation;
                }

                for (int c = 0; c < block.transform.childCount; c++)
                    GameObject.Destroy(block.transform.GetChild(c).gameObject);
                if (controlInputs.ContainsKey(simPos))
                {
                    block.GetComponent<PlayerController>().SetHasAction(true);

                    GameObject arrow = GameObject.Instantiate(controlInputs[simPos] == ControlInput.Stop ? arrowStopPrefab : arrowPrefab, block.transform);
                    arrow.transform.localScale = new Vector3(1, 0.25f, 0.25f);
                    if (controlInputs[simPos] == ControlInput.Forward)
                        arrow.transform.localEulerAngles = new Vector3(90, 0, 0);
                    else if (controlInputs[simPos] == ControlInput.Backward)
                        arrow.transform.localEulerAngles = new Vector3(-90, 0, 0);
                }
                else
                {
                    block.GetComponent<PlayerController>().SetHasAction(false);
                }

                for (int e = 0; e < enemies.Length; e++)
                {
                    enemies[e].GetComponent<EnemyController>().Proceed();
                }

                if (simPos + 1 < maxTimeSteps)
                {
                    states[simPos + 1].position = player.transform.position;
                    states[simPos + 1].velocity = rigidbody.velocity;
                    states[simPos + 1].movementMode = state.movementMode;
                }
            }
            Physics.autoSimulation = true;

            player.transform.position = states[0].position;
            rigidbody.velocity = states[0].velocity;

            GameObject.Find("Viewer").GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
