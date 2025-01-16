using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

/// An SoundManager uses an ObjectPool to manage SoundEmitters
/// An SoundEmitter is a wrapper on AudioSource to load SoundData
//
/// Usage:
/// 
/// ```c#
/// SoundManager.Instance.CreateSound().WithSoundData(soundData).Play();
/// ```
/// 
/// See SoundBuilder.cs for mor details
public class SoundManager : Singleton<SoundManager> {
    IObjectPool<SoundEmitter> soundEmitterPool;
    readonly List<SoundEmitter> activeSoundEmitters = new();
    // public readonly Dictionary<SoundData, int> Counts = new();
    public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();

    [SerializeField] SoundEmitter soundEmitterPrefab;
    [SerializeField] bool collectionCheck = true;
    [SerializeField] int defaultCapacity = 10;
    [SerializeField] int maxPoolSize = 100;
    [SerializeField] int maxSoundInstances = 30;

    void Start() {
        soundEmitterPool = new ObjectPool<SoundEmitter>(
            CreateSoundEmitter,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize
        );
    }

    public SoundBuilder CreateSound() => new SoundBuilder(this);

    public bool CanPlaySound(SoundData data) {
        // return !Counts.TryGetValue(data, out var count) || count >= maxSoundInstances;
        if (!data.frequentSound) return true;

        if (FrequentSoundEmitters.Count >= maxSoundInstances && FrequentSoundEmitters.TryDequeue(out var soundEmitter)) {
            try {
                soundEmitter.Stop();
                return true;
            }
            catch {
                Debug.Log("SoundEmitter is already released");
            }
            return false;
        }
        return true;
    }

    public SoundEmitter Get() {
        return soundEmitterPool.Get();
    }

    public void ReturnedToPool(SoundEmitter soundEmitter) {
        soundEmitterPool.Release(soundEmitter);
    }

    void OnDestroyPoolObject(SoundEmitter soundEmitter) {
        Destroy(soundEmitter.gameObject);
    }

    void OnReturnedToPool(SoundEmitter soundEmitter) {
        // if (Counts.TryGetValue(soundEmitter.Data, out var count)) {
        //     Counts[soundEmitter.Data] -= count > 0 ? 1 : 0;
        // }
        soundEmitter.gameObject.SetActive(false);
        activeSoundEmitters.Remove(soundEmitter);
    }

    void OnTakeFromPool(SoundEmitter soundEmitter) {
        soundEmitter.gameObject.SetActive(true);
        activeSoundEmitters.Add(soundEmitter);
    }

    SoundEmitter CreateSoundEmitter() {
        var soundEmitter = Instantiate(soundEmitterPrefab);
        soundEmitter.gameObject.SetActive(false);
        return soundEmitter;
    }
}

[Serializable]
public class SoundData {
    public List<AudioClip> clips = new();
    public AudioMixerGroup mixerGroup;
    public bool loop;
    public bool playOnAwake;
    public bool frequentSound;
}