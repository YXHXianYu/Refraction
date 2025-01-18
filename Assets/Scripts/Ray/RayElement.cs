using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RayElement : MonoBehaviour {
    // data
    public Vector2Int position {
        set { logicalRay.position = value; }
        get { return logicalRay.position; }
    }
    public EDirection4 rayForwardDirection {
        get { return logicalRay.rayForwardDirection; }
        set { logicalRay.rayForwardDirection = value; }
    }
    public ERayType rayType {
        get { return logicalRay.rayType; }
        set { logicalRay.rayType = value; }
    }
    public bool isMatch;
    public uint rayLevel;

    // render
    
    public TextMeshPro debugTextLeft;
    public TextMeshPro debugTextRight;
    public TextMeshPro debugTextTop;
    public TextMeshPro debugTextBottom;


    public void UpdateRenderInfo() {
        debugTextLeft.text = "";
        debugTextRight.text = "";
        debugTextTop.text = "";
        debugTextBottom.text = "";

        string content = "";
        if (rayForwardDirection == EDirection4.Top) {
            content = "↑";
        } else if (rayForwardDirection == EDirection4.Bottom) {
            content = "↓";
        } else if (rayForwardDirection == EDirection4.Left) {
            content = "←";
        } else if (rayForwardDirection == EDirection4.Right) {
            content = "→";
        }

        if (rayType == ERayType.LeftCenter) {
            debugTextLeft.text = content;
        } else if (rayType == ERayType.RightCenter) {
            debugTextRight.text = content;
        } else if (rayType == ERayType.TopCenter) {
            debugTextTop.text = content;
        } else if (rayType == ERayType.BottomCenter) {
            debugTextBottom.text = content;
        }
    }

    // logical ray inner
    private readonly LogicalRay logicalRay = new LogicalRay();
}