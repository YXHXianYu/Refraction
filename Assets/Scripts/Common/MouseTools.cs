using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseTools {
    public static Vector3 GetMouseWorldPosition() {
        var mouse_pos = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.transform.position.z
        );
        return Camera.main.ScreenToWorldPoint(mouse_pos);
    }
}