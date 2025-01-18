using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RayReceiverMapElement : BaseMapElement {
    public EDirection4 direction;
    public uint targetRayLevel;
    public bool isRayReceived;

    public TextMeshPro debugText;
    
    [SerializeField] private GameObject rayActive;

    public RayReceiverMapElement(): base(EMapElementType.RayReceiver) {}

    private void Update() {
        UpdateRenderInfo();
    }

    private void UpdateRenderInfo() {
        if (isRayReceived) {
            debugText.text = "Rcv|" + targetRayLevel + "|R";
            rayActive.SetActive(true);
        } else {
            debugText.text = "Rcv|" + targetRayLevel + "|U";
            rayActive.SetActive(false);
        }
    }
}