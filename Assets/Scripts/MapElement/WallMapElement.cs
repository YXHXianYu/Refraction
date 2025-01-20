using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMapElement : BaseMapElement {
    public EWallType wallType;

    public WallMapElement(): base(EMapElementType.Wall) {
    }
}