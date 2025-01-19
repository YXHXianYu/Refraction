using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class RayReceiverMapElement : BaseMapElement {
    public EDirection4 direction;
    public uint targetRayLevel;
    public bool isRayReceived;

    public TextMeshPro debugText;

    [SerializeField]
    private GameObject rayActive;

    // Audio
    private bool prevIsRayReceived = false;
    public List<AudioClip> enableAudioClips = new();
    public List<AudioClip> disableAudioClips = new();
    public AudioMixerGroup mixerGroup;
    private List<SoundData> enableSoundDatas;
    private List<SoundData> disableSoundDatas;

    public RayReceiverMapElement(): base(EMapElementType.RayReceiver) {}

    private void Start() {
        enableSoundDatas = enableAudioClips.Select(x => new SoundData(x, mixerGroup)).ToList();
        disableSoundDatas = disableAudioClips.Select(x => new SoundData(x, mixerGroup)).ToList();
    }

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
                    .WithSoundData(enableSoundDatas[(int)targetRayLevel])
                    .Play();
                // Cover
                rayActive.SetActive(true);                
                rayActive.transform.SetParent(transform);

            } else {
                debugText.text = "Rcv|" + targetRayLevel + "|U";
                // PLAY AUDIO: disable
                SoundManager.Instance.CreateSound()
                    .WithSoundData(disableSoundDatas[(int)targetRayLevel])
                    .Play();
                // Cover
                rayActive.SetActive(false);
                rayActive.transform.SetParent(transform);
            }
        }
    }
}