using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleMapElement : BaseMapElement {
    
    public uint bubbleSize;
    public uint bubbleThickness;

    public BubbleMapElement(): base(EMapElementType.Bubble) {
    }
}
