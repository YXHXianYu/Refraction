using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayReceiverMapElement : BaseMapElement {
    public EDirection4 direction;
    public uint initialRayLevel;

    public RayReceiverMapElement(): base(EMapElementType.RayReceiver) {
    }
}