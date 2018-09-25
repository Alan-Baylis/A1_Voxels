using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IObserverPattern
{
    public abstract class IObservable : MonoBehaviour
    {
        LinkedList<IObserver> observers = new LinkedList<IObserver>();

        public void AddSubscriber(IObserver subscriber)
        {
            // if observer is not already subscribed
            if (observers.Find(subscriber) == null)
            {
                // add it to the list
                observers.AddLast(subscriber);
            }
        }

        public void RemoveSubscriber(IObserver subscriber)
        {
            observers.Remove(subscriber);
        }

        public virtual void NotifyAll(GameObject entity, OBSERVER_EVENT observedEvent)
        {
            foreach (IObserver item in observers)
            {
                item.OnNotify(entity, observedEvent);
            }
        }
    }
}