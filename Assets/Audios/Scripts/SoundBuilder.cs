using UnityEngine;

public class SoundBuilder {
    readonly SoundManager soundManager;
    SoundData soundData;
    Vector3 position = Vector3.zero;
    bool randomPitch;

    public SoundBuilder(SoundManager manager) {
        soundManager = manager;
    }

    public SoundBuilder WithSoundData(SoundData data) {
        soundData = data;
        return this;
    }

    public SoundBuilder WithPosition(Vector3 pos) {
        position = pos;
        return this;
    }

    public SoundBuilder WithRandomPitch(bool x) {
        randomPitch = x;
        return this;
    }

    public void Play() {
        if (!soundManager.CanPlaySound(soundData)) return;

        SoundEmitter soundEmitter = soundManager.Get();
        soundEmitter.Initialize(soundData);
        soundEmitter.transform.position = position;
        soundEmitter.transform.parent = SoundManager.Instance.transform;

        if (randomPitch) {
            soundEmitter.ApplyRandomPitch();
        }

        // soundManager.Counts[soundData] =
        //     soundManager.Counts.TryGetValue(soundData, out var count) ? count + 1 : 1;
        if (soundData.frequentSound) {
            soundManager.FrequentSoundEmitters.Enqueue(soundEmitter);
        }
        soundEmitter.Play();
    }
}