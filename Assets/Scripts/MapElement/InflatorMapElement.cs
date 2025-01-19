using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class InflatorMapElement : BaseMapElement {
    public EDirection4 direction;

    public TextMeshPro debugText;

    public InflatorMapElement(): base(EMapElementType.Inflator) {}

    // Audio
    public SoundData inflateSoundData;

    private void Update() {
        UpdateSelectionType();
        UpdateRenderInfo();
        UpdateAnimation();
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
    
    private void UpdateAnimation() {
        foreach (var animator in Animators) {
            switch (selectionType) {
                case EOnSelectionType.Hover:
                    animator.SetBool("onHover", true);
                    break;
                case EOnSelectionType.Selected:
                    animator.SetBool("onHover", false);
                    animator.SetBool("isSelected", true);
                    break;
                case EOnSelectionType.Unselected:
                    animator.SetBool("onHover", false);
                    animator.SetBool("isSelected", false);
                    break;
            }
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
        var result = levelController.ApplyInflator(position);
        if (result.isSuc) {
            // PLAY AUDIO: inflate
            SoundManager.Instance.CreateSound()
                .WithSoundData(inflateSoundData)
                .WithRandomPitch(true)
                .Play();
        }
    }

    private bool isDragging = false;
    private float lastClickTime = -OurSettings.DOUBLE_CLICK_TIME;

    public void OnMouseDown() {
        if (!isGameLogicActive) return;
        
        isDragging = true;

        if (Time.time - lastClickTime < OurSettings.DOUBLE_CLICK_TIME) {
            lastClickTime = -OurSettings.DOUBLE_CLICK_TIME;
        } else {
            OnSingleClick();
            lastClickTime = Time.time;
        }
    }

    public void OnMouseUp() {
        if (!isGameLogicActive) return;
        
        isDragging = false;
    }

    private void OnSingleClick() {
        if (!isGameLogicActive) return;
        
        InflatBubble();
    }
}