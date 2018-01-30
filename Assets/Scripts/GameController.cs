using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject activeTimeFrames;
    private GameObject player;
    public Simulator simulator;
    public GameObject scoreText;
    private GameObject finish;
    public int maxTimeSteps = 1000;
    private float buildTime;

    void Start ()
	{
	    finish = GameObject.FindWithTag("Finish");
	    player = GameObject.Find("Player");

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
