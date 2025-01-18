using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RayReceiverMapElement : BaseMapElement {
    public EDirection4 direction;
    public uint targetRayLevel;
    public bool isRayReceived;

    public TextMeshPro debugText;

    public RayReceiverMapElement(): base(EMapElementType.RayReceiver) {}

    private void Update() {
        UpdateRenderInfo();
    }

    private void UpdateRenderInfo() {
        if (isRayReceived) {
            debugText.text = "Rcv|" + targetRayLevel + "|R";
        } else {
            debugText.text = "Rcv|" + targetRayLevel + "|U";
        }
    }
}