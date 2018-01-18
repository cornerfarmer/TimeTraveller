using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody rigidbody;
    public bool forward = false;
    private MovementController movementController;

    // Use this for initialization
    void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
        movementController = new MovementController(rigidbody, transform);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Proceed()
    {
        movementController.Move(forward ? 1 : -1, 0);
    }
}
