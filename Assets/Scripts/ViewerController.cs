using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerController : MonoBehaviour
{
    public GameObject gameController;
    private MovementController movementController;

    private TimeFrameController markedCube;

    private bool cubeIsLocked;
    private bool movementActive;

    private Simulator simulator;
    // Use this for initialization
    void Start ()
	{
	    movementController = new MovementController(GetComponent<Rigidbody>(), transform.GetChild(0));
	    markedCube = null;
	    cubeIsLocked = false;
	    movementActive = true;
        simulator = gameController.GetComponent<GameController>().simulator;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    Movement();

	    CubeSelection();
	    SetInput();
        SendRay();
	}

    void Movement()
    {
        if (!cubeIsLocked)
        {
            movementActive |= (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W));
            if (movementActive)
                movementController.Move(Input.GetAxis("Vertical") == 0 ? 0 :(int)Mathf.Sign(Input.GetAxis("Vertical")), Input.GetAxis("Horizontal") == 0 ? 0 : (int)Mathf.Sign(Input.GetAxis("Horizontal")));
           
            if (Input.GetButtonDown("Jump"))
                movementController.Jump();
        }
        else
            movementController.Move(0, 0);
    }

    void SetInput()
    {
        if (cubeIsLocked)
        {
            bool successful = false;

            if (Input.GetKeyDown(KeyCode.D))
            {
                simulator.SetInput(Int32.Parse(markedCube.name), ControlInput.Forward);
                successful = true;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                simulator.SetInput(Int32.Parse(markedCube.name), ControlInput.Backward);
                successful = true;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                simulator.SetInput(Int32.Parse(markedCube.name), ControlInput.Jump);
                successful = true;
            }

            if (successful)
            {
                markedCube.unhover();
                markedCube = null;
                cubeIsLocked = false;
            }
        }
    }

    void CubeSelection()
    {
        if (Input.GetMouseButtonDown(0) && markedCube != null)
        {
            if (cubeIsLocked)
            {
                markedCube.unhover();
                markedCube = null;
                cubeIsLocked = false;
            }
            else
            {
                markedCube.mark();
                cubeIsLocked = true;
                movementActive = false;
            }
        }
    }

    void SendRay()
    {
        if (!cubeIsLocked)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 2))
            {
                TimeFrameController controller = hit.collider.gameObject.GetComponent<TimeFrameController>();

                if (controller != null && !controller.locked)
                {
                    if (markedCube != controller)
                    {
                        if (markedCube != null)
                            markedCube.unhover();
                        controller.hover();
                        markedCube = controller;
                    }
                    return;
                }
            }

            if (markedCube != null)
            {
                markedCube.unhover();
                markedCube = null;
            }
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), "");
    }
}
