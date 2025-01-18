using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflatorMapElement : BaseMapElement {
    public EDirection4 direction;

    public InflatorMapElement(): base(EMapElementType.Inflator) {
    }

    public void InflatBubble(BubbleMapElement bubble) {
        // TODO: Implement this method
        Debug.LogError("InflatorMapElement.InflatBubble() not implemented");
    }
}