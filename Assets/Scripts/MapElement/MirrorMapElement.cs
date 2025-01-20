using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMapElement : BaseMapElement {
    public EDirectionDiagnal4 direction;

    public MirrorMapElement(): base(EMapElementType.Mirror) {
    }
}