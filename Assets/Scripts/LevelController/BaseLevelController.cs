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
            var mouse_pos = new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.transform.position.z
            );
            var world_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

            var pos = new Vector2Int((int)(world_pos.x + 0.5), (int)(world_pos.y + 0.5));
            if (mapElements.ContainsKey(pos)) {
                var element = mapElements[pos];
                Debug.Log("Mouse clicked, " + pos + ": " + element.mapElementType);
                MapElementIsClicked(element);
            } else {
                Debug.Log("Mouse clicked, but no element found at " + pos);
            }
        }
    }

    private void MapElementIsClicked(BaseMapElement element) {
        Debug.Log("Map element is clicked: " + element.mapElementType);
        if (element is ValveMapElement valve) {
            // TODO: 
        }
    }
}
