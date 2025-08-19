using System.Collections.Generic;
using System;
using UnityEngine;


// This class is in charge of communicating small events inside a level, must be instanced in the level manager
// Every event has an ID, the object which activates the event must call Invoke() and the listeners must Suscribe() or Unsuscribe()
public class EventManager
{ 
    private Dictionary<string, Action> events = new();
   
    public void Subscribe(string eventID, Action listener)
    {
        if (!events.ContainsKey(eventID)) // if not already created, it creates the event
        {
            events.Add(eventID, delegate { });
        }
        events[eventID] += listener;
    }

    public void Unsubscribe(string eventID, Action listener)
    {
        if (events.ContainsKey(eventID))
            events[eventID] -= listener;
    }

    public void Invoke(string eventID)
    {
        if (events.ContainsKey(eventID))
            events[eventID].Invoke();
    }
}
