using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpikesSimulation : EmptySimulation
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            other.gameObject.SetActive(false);
        }
    }
}
