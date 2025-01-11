using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEventEmitter : Singleton<DefaultEventEmitter> {
    protected override void Awake() {
        base.Awake();
    }

    private EventEmitter eventEmitter = new();

    public void On(string eventName, EventEmitter.EventListener listener) {
        eventEmitter.On(eventName, listener);
    }

    public void Emit(string eventName, params object[] args) {
        eventEmitter.Emit(eventName, args);
    }

    public void Off(string eventName, EventEmitter.EventListener listener) {
        eventEmitter.Off(eventName, listener);
    }
}