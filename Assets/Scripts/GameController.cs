using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject alphaWall;
    public GameObject activeTimeFrames;
    public GameObject player;
    public Simulator simulator;
    public GameObject map;
    public GameObject scoreText;
    public GameObject finish;
    private int maxTimeSteps;
    private int mapWidth;
    private float buildTime;

    void Start ()
	{
	    maxTimeSteps = 1000;
	    mapWidth = 23;

        Instantiate(alphaWall, new Vector3(0, 0, 0), Quaternion.identity).transform.localScale = new Vector3(mapWidth, 10, 1);
	    Instantiate(alphaWall, new Vector3(-0.5f, 0, 0), Quaternion.Euler(0, 90, 0)).transform.localScale = new Vector3(maxTimeSteps / 4.0f, 10, 1);
	    Instantiate(alphaWall, new Vector3(mapWidth, 0, 0), Quaternion.Euler(0, 90, 0)).transform.localScale = new Vector3(maxTimeSteps / 4.0f, 10, 1);
	    Instantiate(alphaWall, new Vector3(0, 0, -maxTimeSteps / 4.0f - 0.5f), Quaternion.identity).transform.localScale = new Vector3(mapWidth, 10, 1);

        map.transform.localScale = new Vector3(1, 1, maxTimeSteps / 4.0f);
	    map.transform.Find("Finish").GetComponent<Renderer>().material.mainTextureScale = new Vector2(maxTimeSteps / 4.0f, 1);

        simulator = new Simulator(activeTimeFrames, player, finish, maxTimeSteps);
	    simulator.Simulate(0);
    }
	
	void Update ()
	{
	    simulator.SimulateNextBatch();

        scoreText.GetComponent<Text>().text = "Build time: " + secondsToString((int)buildTime) + "  Run time: " + secondsToString((int)simulator.runTime) + "  Total time: " + secondsToString(simulator.runTime == 0 ? 0 : (int)buildTime + (int)simulator.runTime);
        
	    buildTime += Time.deltaTime;
    }

    private string secondsToString(int seconds)
    {
        if (seconds != 0)
        {
            int minutes = seconds / 60;
            seconds -= minutes * 60;
            return minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else
        {
            return "-:-";
        }
    }
}
