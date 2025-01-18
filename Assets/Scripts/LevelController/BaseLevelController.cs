using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLevelController : MonoBehaviour {
    // MARK: public

    public Vector2Int levelSize;
    public uint gasCount;

    // MARK: Serialized Private

    [SerializeField]
    private GameObject mapElementsParent;

    // MARK: private
    
    private Dictionary<Vector2Int, BaseMapElement> mapElements;

    private void Start() {
        var elements = mapElementsParent.GetComponentsInChildren<BaseMapElement>();
        mapElements = new Dictionary<Vector2Int, BaseMapElement>();
        var elementsStr = "";
        foreach (var element in elements) {
            var pos_x = (int)(element.transform.position.x + 0.5);
            var pos_y = (int)(element.transform.position.y + 0.5);
            var pos = new Vector2Int(pos_x, pos_y);
            mapElements.Add(pos, element);
            elementsStr += " [" + pos + ": " + element.mapElementType + "]";
        }
        Debug.Log("Level is initialized with " + elements.Length + " elements. Elements: " + elementsStr);
    }

    private void Update() {
        UpdateMouseControl();
    }

    private void UpdateMouseControl() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Mouse clicked");
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit)) {
                var pos = new Vector2Int((int)(hit.point.x + 0.5), (int)(hit.point.y + 0.5));
                if (mapElements.ContainsKey(pos)) {
                    var element = mapElements[pos];
                    Debug.Log("Clicked on " + pos + ": " + element.mapElementType);
                    if (element is ValveMapElement valve) {
                        if (valve.isOpen) {
                            valve.CloseValve();
                        } else {
                            valve.OpenValve();
                        }
                    }
                }
            }
        }
    }
}
