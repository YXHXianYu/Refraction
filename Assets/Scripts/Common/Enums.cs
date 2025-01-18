using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDirection4 {
    Top,
    Bottom,
    Left,
    Right
}

public enum EDirection2 {
    Vertical,
    Horizontal
}

public enum EDirectionDiagnal4 {
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

public enum ELightTransmittanceType {
    Opaque,
    Transparent,
    Bubble,
    Mirror
}

public enum EMapElementType {
    Bubble,
    Inflator,
    Extractor,
    Valve,
    Wall,
    RaySource,
    RayReceiver,
    Mirror,
    Ray
}

public enum EOnSelectionType {
    Unselected,
    Selected,
    Hover
}
public enum EWallType {
    TopBottom,
    LeftRight,
    TopLeftBottom,
    TopRightBottom,
    TopLeftRight,
    BottomLeftRight,
    TopBottomLeftRight
}
