using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BubbleMapElement : BaseMapElement {
    
    public uint bubbleSize;
    public uint bubbleThickness;

    // -1 means no ray pass; 0~inf means where the ray pass
    public int bubbleLeftRayLevel = -1;
    public int bubbleRightRayLevel = -1;
    public int bubbleTopRayLevel = -1;
    public int bubbleBottomRayLevel = -1;

    public BubbleMapElement(): base(EMapElementType.Bubble) {
    }

    public void OnPointerClick(PointerEventData eventData) {
    }

    private bool isDragging = false;
    private Vector2Int startPosition;

    public void OnMouseDown() {
        isDragging = true;
        startPosition = position;
    }

    public void OnMouseDrag() {
        if (!isDragging) {
            return;
        }

        var world_pos = MouseTools.GetMouseWorldPosition();
        transform.position = new Vector3(world_pos.x, world_pos.y, 0);
    }

    public void OnMouseUp() {
        isDragging = false;

        var world_pos = MouseTools.GetMouseWorldPosition();
        transform.position = new Vector3(world_pos.x, world_pos.y, 0);

        var source = new Vector2Int((int)(startPosition.x + 0.5f), (int)(startPosition.y + 0.5f));
        var target = new Vector2Int((int)(world_pos.x + 0.5f), (int)(world_pos.y + 0.5f));

        if (source == target) {
            return;
        }

        var result = levelController.MoveBubble(source, target);

        if (!result.isSuc) {
            Debug.Log("泡泡移动的失败原因：" + result.errMsg);
            position = startPosition;
            transform.position = new Vector3((int)(startPosition.x + 0.5f), (int)(startPosition.y + 0.5f), 0);
        } else {
            position = target;
            transform.position = new Vector3((int)(target.x + 0.5f), (int)(target.y + 0.5f), 0);
        }
    }
}
