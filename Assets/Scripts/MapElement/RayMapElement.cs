using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayMapElement : BaseMapElement {
    public EDirection2 direction;
    public bool isMatch;
    public uint rayLevel;

    public RayMapElement(): base(EMapElementType.Ray) {
    }
}