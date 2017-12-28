using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Renderer renderer;

    private bool hasAction;

    private float transitionDelta;

    private Vector3 transitionDestination;
    private Vector3 transitionSource;

    public float transitTime = 1;
    // Use this for initialization
    void Start ()
	{
	    renderer = GetComponent<Renderer>();
	    hasAction = false;
	    transitionDelta = 1;

	}
	
	// Update is called once per frame
	void Update () {
	    if (transitionDelta < 1)
	    {
	        transitionDelta += Time.deltaTime / transitTime;
            transform.position = Vector3.Lerp(transitionSource, transitionDestination, transitionDelta);
	    }
	}

    private void ResetColor()
    {
        if (hasAction)
            renderer.material.color = Color.blue;
        else
            renderer.material.color = Color.red;
    }

    public void SetHasAction(bool hasAction)
    {
        this.hasAction = hasAction;
    }

    public void hover()
    {
        renderer.material.color = Color.green;
    }

    public void unhover()
    {
        ResetColor();
    }

    public void mark()
    {
        renderer.material.color = Color.grey;
    }

    public void TransitToNewPosition(Vector3 newPosition, float transitTime)
    {
        transitionDelta = 0;
        this.transitTime = transitTime;
        transitionSource = transform.position;
        transitionDestination = newPosition;
    }
}
