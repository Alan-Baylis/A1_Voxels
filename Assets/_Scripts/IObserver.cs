using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class IObserver
{
    public abstract void OnNotify();
}

public abstract class IObservable
{
    LinkedList<IObserver> observers;

    public void subscribe(IObserver subscriber)
    {
        // if observer is not already subscribed
        if(observers.Find(subscriber) == null)
        {
            // add it to the list
            observers.AddLast(subscriber);
        }
    }

    public void unsubscribe(IObserver subscriber)
    {
        observers.Remove(subscriber);
    }

    public virtual void NotifyAllObservers()
    {
        foreach (IObserver item in observers)
        {
            item.OnNotify();
        }
    }
}

/*
public class NoiseCritterSwarm : IObserver
{
    public Vector3 m_seekTarget;
    public float m_seekStrength;
    public float m_

    public bool 

    // ref to player
    public IObservable player;

    List<NoiseCritter> swarmMembers;

    public override void OnNotify()
    {

    }
}*/

public class NoiseCritter : MonoBehaviour
{
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
