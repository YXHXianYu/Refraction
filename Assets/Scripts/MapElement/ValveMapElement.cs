using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ValveMapElement : BaseMapElement {
    public EDirection2 valveDirection;
    public EDirectionDiagnal4 bubbleDirection;
    public bool isOpen;
    public uint maxSizeCouldPass;
    public uint sizeNeedToOpen;

    public TextMeshPro debugText;

    public ValveMapElement(): base(EMapElementType.Valve) {}

    private void Update() {
        UpdateRenderInfo();
        UpdateAnimation();
    }
    
    private void UpdateAnimation() {
        foreach (var animator in Animators) {
            if (isOpen) {
                animator.SetBool("isOpen", true);
            }
            else {
                animator.SetBool("isOpen", false);
            }
        }
    }

    private void UpdateRenderInfo() {
        debugText.text = "V|>=" + sizeNeedToOpen + "|<=" + maxSizeCouldPass + "|" + (isOpen ? "Op" : "Cl");
    }
}