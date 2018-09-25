using UnityEngine;
using IObserverPattern;

/// <summary>
/// Attach this script to a player to watch for achievement unlocks and fire events
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerAchievementChecker : IObservable
{
    Vector3 lastPosition;
    private void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // if entered the void
        if (lastPosition.y >= 0.0f && transform.position.y < 0.0f)
        {
            NotifyAll(gameObject, OBSERVER_EVENT.ENTERED_VOID);
        }

        lastPosition = transform.position;
    }
}
