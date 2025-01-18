using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayReceiverMapElement : BaseMapElement {
    public EDirection4 direction;
    public uint targetRayLevel;

    public RayReceiverMapElement(): base(EMapElementType.RayReceiver) {
    }
}