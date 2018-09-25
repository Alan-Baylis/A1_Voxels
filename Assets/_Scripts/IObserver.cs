using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IObserverPattern
{
    public enum OBSERVER_EVENT
    {
        DEFAULT = 0,
        ENTERED_VOID,
        PLACED_BLOCK,
        UNLOCKED_ACHIEVEMENT,
        NUM_OBSERVER_EVENTS
    }

    public abstract class IObserver : IObservable
    {
        public void SubscribeTo(IObservable toWatch)
        {
            toWatch.AddSubscriber(this);
            Debug.Log("subscriber added");
        }

        public abstract void OnNotify(GameObject entity, OBSERVER_EVENT observedEvent);
    }
}


