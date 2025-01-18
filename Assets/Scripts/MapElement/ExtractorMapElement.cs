using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class ExtractorMapElement : BaseMapElement {
    public EDirection4 direction;
    
    public TextMeshPro debugText;

    public ExtractorMapElement(): base(EMapElementType.Extractor) {}

    private void Update() {
        UpdateSelectionType();
        UpdateRenderInfo();
    }

    private void UpdateRenderInfo() {
        if (selectionType == EOnSelectionType.Selected) {
            debugText.text = "Extcr|Sel";
        } else if (selectionType == EOnSelectionType.Hover) {
            debugText.text = "Extcr|Hov";
        } else {
            debugText.text = "Extcr|Uns";
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

    private void ExtractBubble() {
        levelController.ApplyExtractor(position);
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
        ExtractBubble();
    }
}
