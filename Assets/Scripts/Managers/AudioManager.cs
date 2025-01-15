using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// AudioManager manages the audio in the scene:
/// - bgm: background music
/// - ambient: ambient sound
/// 
/// Currently it just plays the configured audio when the scene is loaded,
/// and stops when the scene is unloaded.
/// 
/// Actually ambient can be moved into the corresponding scene itself as a single AudioSource.
/// But if the scene is very simple, we can think that the ambient is constant in the same scene.
/// 
/// TODO: If we need to change the bgm or ambient in the middle of the scene,
/// TODO: we can expose a method like `setBgm` or `setAmbient` here.
/// 
/// TODO: But then, we may need to use enum to define the clip for convenience.
/// TODO: Therefore, a separate class will be needed to store the relation
/// TODO: between the enum and the clip.
/// </summary>
public class AudioManager : Singleton<AudioManager> {
    public AudioSource bgmSource;
    public AudioSource ambientSource;

    public List<SceneAudioDetail> sceneAudioList;

    private void OnEnable() {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode _mode) {
        SceneAudioDetail sceneAudio = sceneAudioList.Find(x => x.sceneName == scene.name);

        if (sceneAudio != null) {
            Debug.Log("[AudioManager] OnSceneLoaded: " + scene.name + "; bgmName: " + sceneAudio.bgmClip.name + "]");
            if (bgmSource.isActiveAndEnabled) {
                // Maybe this is redundant
                if (bgmSource.isPlaying) {
                    bgmSource.Stop();
                }

                if (sceneAudio.bgmClip != null) {
                    bgmSource.clip = sceneAudio.bgmClip;
                    bgmSource.volume = sceneAudio.bgmVolume;
                    bgmSource.Play();
                }
            }

            if (ambientSource.isActiveAndEnabled) {
                // Maybe this is redundant
                if (ambientSource.isPlaying) {
                    ambientSource.Stop();
                }

                if (sceneAudio.ambientClip != null) {
                    ambientSource.clip = sceneAudio.ambientClip;
                    ambientSource.volume = sceneAudio.ambientVolume;
                    ambientSource.Play();
                }
            }
        }
    }

    void OnSceneUnLoaded(Scene scene) {
        SceneAudioDetail sceneAudio = sceneAudioList.Find(x => x.sceneName == scene.name);
        if (sceneAudio != null) {
            if (bgmSource.isActiveAndEnabled && bgmSource.isPlaying) {
                bgmSource.Stop();
                bgmSource.clip = null; // Maybe this is redundant
            }

            if (ambientSource.isActiveAndEnabled && ambientSource.isPlaying) {
                ambientSource.Stop();
                ambientSource.clip = null; // Maybe this is redundant
            }
        }
    }
}

[Serializable]
public class SceneAudioDetail {
    public string sceneName;

    // Bgm
    public AudioClip bgmClip;

    [Range(0.0f, 1.0f)] public float bgmVolume;

    // Ambient
    public AudioClip ambientClip;
    [Range(0.0f, 1.0f)] public float ambientVolume;
}