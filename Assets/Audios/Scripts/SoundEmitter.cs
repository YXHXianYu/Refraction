using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// A wrapper on AudioSource to use SoundData
public class SoundEmitter : MonoBehaviour {
    public SoundData Data { get; private set; }
    private AudioSource audioSource;
    private Coroutine playingCoroutine;

    void Awake() {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Play() {
        if (playingCoroutine != null) {
            StopCoroutine(playingCoroutine);
        }

        audioSource.Play();
        playingCoroutine = StartCoroutine(WaitForSoundToEnd());
    }

    IEnumerator WaitForSoundToEnd() {
        yield return new WaitWhile(() => audioSource.isPlaying);
        SoundManager.Instance.ReturnedToPool(this);
    }

    public void Stop() {
        if (playingCoroutine != null) {
            StopCoroutine(playingCoroutine);
            playingCoroutine = null;
        }

        audioSource.Stop();
        SoundManager.Instance.ReturnedToPool(this);
    }

    public void Initialize(SoundData data) {
        Data = data;
        // audioSource.clip = data.clip;
        if (data.clips.Count == 0) {
            Debug.LogWarning("SoundData has no clips");
            audioSource.clip = null;
            return;
        }
        audioSource.clip = data.clips[Random.Range(0, data.clips.Count)];
        audioSource.outputAudioMixerGroup = data.mixerGroup;
        audioSource.loop = data.loop;
        audioSource.playOnAwake = data.playOnAwake;
        audioSource.pitch = 1;
    }

    public void ApplyRandomPitch(float min = -0.05f, float max = 0.05f) {
        audioSource.pitch += Random.Range(min, max);
    }
}
