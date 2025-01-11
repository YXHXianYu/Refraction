using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Event Emitter
/// </summary>
/// 
/// <example>
///     eventListener = (args) => {
///         EventSelectPlayerBackpackSlot((ItemDetails)args[0], (bool)args[1]);
///     };
///     DefaultEventEmitter.Instance.On("xxx", eventListener);
///     DefaultEventEmitter.Instance.Off("xxx", eventListener);
///     DefaultEventEmitter.Instance.Emit("xxx", an_item_details, a_boolean);
/// </example>
public class EventEmitter {
    public delegate void EventListener(params object[] args);

    private readonly Dictionary<string, List<EventListener>> events = new();

    public void On(string eventName, EventListener listener) {
        if (!events.ContainsKey(eventName)) events[eventName] = new List<EventListener>();

        events[eventName].Add(listener);
    }

    public void Emit(string eventName, params object[] args) {
        if (events.TryGetValue(eventName, out var listeners))
            foreach (var listener in listeners)
                listener(args);
    }

    public void Off(string eventName, EventListener listener) {
        if (events.TryGetValue(eventName, out var listeners)) {
            listeners.Remove(listener);
            if (listeners.Count == 0) events.Remove(eventName);
        }
    }
}