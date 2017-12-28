using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Simulator
{
    private GameObject prefab;
    private GameObject arrowPrefab;
    private GameObject arrowStopPrefab;
    private GameObject activeTimeFrames;
    private GameObject player;
    private Rigidbody rigidbody;
    private MovementController movementController;
    private Dictionary<int, ControlInput> controlInputs;
    private Vector3 startPos;
    private int maxTimeSteps;
    private float timePerTimeStep;
    private int simulationsPerTimeStep;

    public enum ControlInput
    {
        Jump,
        Forward,
        Backward,
        Stop
    }

    public Simulator(GameObject prefab, GameObject arrowPrefab, GameObject arrowStopPrefab, GameObject activeTimeFrames, GameObject player, int maxTimeSteps)
    {
        this.prefab = prefab;
        this.arrowPrefab = arrowPrefab;
        this.arrowStopPrefab = arrowStopPrefab;
        this.activeTimeFrames = activeTimeFrames;
        this.player = player;
        rigidbody = player.GetComponent<Rigidbody>();
        movementController = new MovementController(rigidbody, player.transform);
        controlInputs = new Dictionary<int, ControlInput>();
        startPos = player.transform.position;

        this.maxTimeSteps = maxTimeSteps;
        timePerTimeStep = 0.02f;
        simulationsPerTimeStep = 3;
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
       
        Simulate(0);
    }

	// Use this for initialization
    public void Simulate(int start)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        GameObject.Find("Viewer").GetComponent<Rigidbody>().isKinematic = true;

        Physics.autoSimulation = false;
        ControlInput movementMode = ControlInput.Stop;
        for (int i = start; i < maxTimeSteps; i++)
        {
            if (controlInputs.ContainsKey(i))
            {
                if (controlInputs[i] == ControlInput.Jump)
                    movementController.Jump();
                else
                    movementMode = controlInputs[i];
            }

            movementController.Move(movementMode == ControlInput.Forward ? 1 : (movementMode == ControlInput.Backward ? -1 : 0), 0);
            
            for (int t = 0; t < simulationsPerTimeStep; t++)
                Physics.Simulate(timePerTimeStep);

            GameObject block;
            if (activeTimeFrames.transform.childCount <= i)
            {
                block = GameObject.Instantiate(prefab, player.transform.position - new Vector3(0, 0, (i + 1) * player.transform.localScale.x), player.transform.rotation, activeTimeFrames.transform);
                block.name = i.ToString();
            }
            else
            {
                block = activeTimeFrames.transform.GetChild(i).gameObject;
                block.GetComponent<PlayerController>().TransitToNewPosition(player.transform.position - new Vector3(0, 0, (i + 1) * player.transform.localScale.x), 0.5f + i / (maxTimeSteps * 2));
                block.transform.rotation = player.transform.rotation;
            }

            for (int c = 0; c < block.transform.childCount; c++)
                GameObject.Destroy(block.transform.GetChild(c).gameObject);
            if (controlInputs.ContainsKey(i))
            {
                block.GetComponent<PlayerController>().SetHasAction(true);

                GameObject arrow = GameObject.Instantiate(controlInputs[i] == ControlInput.Stop ? arrowStopPrefab : arrowPrefab, block.transform);
                arrow.transform.localScale = new Vector3(1, 0.25f, 0.25f);
                if (controlInputs[i] == ControlInput.Forward)
                    arrow.transform.localEulerAngles = new Vector3(90, 0, 0);
                else if (controlInputs[i] == ControlInput.Backward)
                    arrow.transform.localEulerAngles = new Vector3(-90, 0, 0);
            }
            else
            {
                block.GetComponent<PlayerController>().SetHasAction(false);
            }
        }
        Physics.autoSimulation = true;

        sw.Stop();
        Debug.Log(sw.Elapsed);

        player.transform.position = startPos;
        rigidbody.velocity = Vector3.zero;

        GameObject.Find("Viewer").GetComponent<Rigidbody>().isKinematic = false;
    }
}
