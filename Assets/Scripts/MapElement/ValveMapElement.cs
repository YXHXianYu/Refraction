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
    public TextMeshPro hatchText;

    public ValveMapElement(): base(EMapElementType.Valve) {}

    // Audio
    private bool prevIsOpen = false;
    public SoundData openSoundData;
    public SoundData closeSoundData;

    private void Update() {
        if (prevIsOpen != isOpen) {
            prevIsOpen = isOpen;
            OnUpdateIsOpen();
        }
        UpdateRenderInfo();
    }

    private void OnUpdateIsOpen() {
        UpdateAnimation();

        // PLAY SOUND: open/close
        if (isOpen) {
            SoundManager.Instance.CreateSound()
                .WithSoundData(openSoundData)
                .WithRandomPitch(true)
                .Play();
        } else {
            SoundManager.Instance.CreateSound()
                .WithSoundData(closeSoundData)
                .WithRandomPitch(true)
                .Play();
        }
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
        debugText.text = "<=" + maxSizeCouldPass; // + "|" + (isOpen ? "Op" : "Cl");
        hatchText.text = ">=" + sizeNeedToOpen;
    }
}