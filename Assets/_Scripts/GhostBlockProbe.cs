using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GhostBlockProbe : MonoBehaviour {

    private BoxCollider _boxCollider;

    private uint numContacts = 0;

    // Use this for initialization
    void Start ()
    {
        _boxCollider = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        numContacts = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        numContacts = 1;
        // numContacts++;
    }

    private void OnTriggerEnter(Collider other)
    {
           // numContacts++;
    }
    private void OnTriggerExit(Collider other)
    {
         //   numContacts--;
    }

    public bool IsColliding()
    {
        return numContacts > 0;
    }
}
