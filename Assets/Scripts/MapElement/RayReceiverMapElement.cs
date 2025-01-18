using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RayReceiverMapElement : BaseMapElement {
    public EDirection4 direction;
    public uint targetRayLevel;
    public bool isRayReceived;

    public TextMeshPro debugText;

    // Audio
    public bool prevIsRayReceived;
    public SoundData enableSoundData;
    public SoundData disableSoundData;

    public RayReceiverMapElement(): base(EMapElementType.RayReceiver) {}

    private void Update() {
        UpdateRenderInfo();
    }

    private void UpdateRenderInfo() {
        if (isRayReceived != prevIsRayReceived) {
            prevIsRayReceived = isRayReceived;
            if (isRayReceived) {
                debugText.text = "Rcv|" + targetRayLevel + "|R";
                // PLAY AUDIO: enable
                SoundManager.Instance.CreateSound()
                    .WithSoundData(enableSoundData)
                    .Play();
            } else {
                debugText.text = "Rcv|" + targetRayLevel + "|U";
                // PLAY AUDIO: disable
                SoundManager.Instance.CreateSound()
                    .WithSoundData(disableSoundData)
                    .Play();
            }
        }
    }
}