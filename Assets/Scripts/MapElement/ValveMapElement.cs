using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ValveMapElement : BaseMapElement {
    public EDirection2 valveDirection;
    public EDirectionDiagnal4 bubbleDirection;
    public bool isOpen;
    public uint maxSizeCouldPass;
    public uint sizeNeedToOpen;

    public void OpenValve() {
        // TODO: Implement this method
        Debug.LogError("ValveMapElement.OpenValve() not implemented");
    }

    public void CloseValve() {
        // TODO: Implement this method
        Debug.LogError("ValveMapElement.CloseValve() not implemented");
    }

    public ValveMapElement(): base(EMapElementType.Valve) {
    }
}