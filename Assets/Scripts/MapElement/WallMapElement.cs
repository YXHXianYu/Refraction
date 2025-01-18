using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WallMapElement : BaseMapElement {
    public EDirection2 direction;

    public WallMapElement(): base(EMapElementType.Wall) {
    }
}