using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleMapElement : BaseMapElement {
    
    public uint bubbleSize;
    public uint bubbleThickness;

    // -1 means no ray pass; 0~inf means where the ray pass
    public int bubbleLeftRayLevel = -1;
    public int bubbleRightRayLevel = -1;
    public int bubbleTopRayLevel = -1;
    public int bubbleBottomRayLevel = -1;

    public BubbleMapElement(): base(EMapElementType.Bubble) {
    }
}
