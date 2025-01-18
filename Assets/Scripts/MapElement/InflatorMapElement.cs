using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class InflatorMapElement : BaseMapElement {
    public EDirection4 direction;

    public TextMeshPro debugText;

    public InflatorMapElement(): base(EMapElementType.Inflator) {}

    private void Update() {
        UpdateSelectionType();
        UpdateRenderInfo();
    }

    private void UpdateRenderInfo() {
        if (selectionType == EOnSelectionType.Selected) {
            debugText.text = "Inflt|Sel";
        } else if (selectionType == EOnSelectionType.Hover) {
            debugText.text = "Inflt|Hov";
        } else {
            debugText.text = "Inflt|Uns";
        }
    }

    private void UpdateSelectionType() {
        Assert.IsTrue(isSelectable, "BubbleMapElement must be selectable.");

        var mouseWorldPos = MouseTools.GetMouseWorldPosition();
        bool isInCurrentElement =
            mouseWorldPos.x < transform.position.x - 0.5f
            || mouseWorldPos.x > transform.position.x + 0.5f
            || mouseWorldPos.y < transform.position.y - 0.5f
            || mouseWorldPos.y > transform.position.y + 0.5f;

        if (isInCurrentElement) {
            selectionType = EOnSelectionType.Unselected;
        } else if (isDragging) {
            selectionType = EOnSelectionType.Selected;
        } else {
            selectionType = EOnSelectionType.Hover;
        }
    }

    private void InflatBubble() {
        levelController.ApplyInflator(position);
    }

    private bool isDragging = false;
    private float lastClickTime = -OurSettings.DOUBLE_CLICK_TIME;

    public void OnMouseDown() {
        isDragging = true;

        if (Time.time - lastClickTime < OurSettings.DOUBLE_CLICK_TIME) {
            lastClickTime = -OurSettings.DOUBLE_CLICK_TIME;
        } else {
            OnSingleClick();
            lastClickTime = Time.time;
        }
    }

    public void OnMouseUp() {
        isDragging = false;
    }

    private void OnSingleClick() {
        InflatBubble();
    }
}