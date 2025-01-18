using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaySourceMapElement : BaseMapElement {
    public EDirection4 direction;
    public uint targetRayLevel;

    public RaySourceMapElement(): base(EMapElementType.RaySource) {
    }
}
