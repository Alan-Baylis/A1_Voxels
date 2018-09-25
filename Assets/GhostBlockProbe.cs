using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GhostBlockProbe : MonoBehaviour {

    private BoxCollider _boxCollider;
    public bool isColliding { get; private set; }

    // Use this for initialization
    void Start ()
    {
        isColliding = false;
        _boxCollider = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
	}
    
    private void OnTriggerStay(Collider other)
    {
        isColliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isColliding = false;
    }
}
