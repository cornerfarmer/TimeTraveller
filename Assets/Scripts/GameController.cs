using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject prefab;
    public GameObject alphaWall;
    public GameObject prefabArrow;
    public GameObject prefabStopArrow;
    public GameObject activeTimeFrames;
    public GameObject player;
    public Simulator simulator;
    public GameObject map;
    private int maxTimeSteps;
    private int mapWidth;

    void Start ()
	{
	    maxTimeSteps = 100;
	    mapWidth = 9;

        Instantiate(alphaWall, new Vector3(0, 0, 0), Quaternion.identity).transform.localScale = new Vector3(mapWidth, 10, 1);
	    Instantiate(alphaWall, new Vector3(-0.5f, 0, 0), Quaternion.Euler(0, 90, 0)).transform.localScale = new Vector3(maxTimeSteps / 4.0f, 10, 1);
	    Instantiate(alphaWall, new Vector3(mapWidth, 0, 0), Quaternion.Euler(0, 90, 0)).transform.localScale = new Vector3(maxTimeSteps / 4.0f, 10, 1);
	    Instantiate(alphaWall, new Vector3(0, 0, -maxTimeSteps / 4.0f - 0.5f), Quaternion.identity).transform.localScale = new Vector3(mapWidth, 10, 1);

        map.transform.localScale = new Vector3(1, 1, maxTimeSteps / 4.0f);

        simulator = new Simulator(prefab, prefabArrow, prefabStopArrow, activeTimeFrames, player, maxTimeSteps);
	    simulator.Simulate(0);
    }
	
	void Update () {
    }
}
