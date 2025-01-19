using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenController : BaseLevelController {

    [Header("Title Screen Controller")]
    public GameObject TitleScreenBackgroundParent;

    protected override void Start() {
        // Don't call base.Start() in TitleScreenController
        // base.Start();
    }

    protected override void Update() {
        // Don't call base.Update() in TitleScreenController
        // base.Update();

        UpdateTitleScreen();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            QuitGame();
        }
    }

    private void UpdateTitleScreen() {
        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(FadeOutToLevelChooseScene());
        }
    }

    private IEnumerator FadeOutToLevelChooseScene() {
        var stopDuration = 0.5f;
        var fadeDuration = 1.5f;
        yield return StartCoroutine(FadeMapElement(TitleScreenBackgroundParent, stopDuration, fadeDuration));

        StartCoroutine(LoadLevelChooseScene());
    }

}
