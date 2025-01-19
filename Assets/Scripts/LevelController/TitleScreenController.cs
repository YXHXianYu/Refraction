using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenController : BaseLevelController {

    [Header("Title Screen Controller")]
    public GameObject TitleScreenBackgroundParent;
    
    private Animator[] Animators;

    protected override void Start() {
        // Don't call base.Start() in TitleScreenController
        // base.Start();
        var bubbleObj = GameObject.Find("Background_Title_Bubble");
        Animators = bubbleObj.GetComponentsInChildren<Animator>();
    }
    
    private void UpdateAnimation() {
        foreach (var animator in Animators) {
            animator.SetBool("isBoom", true);
        }
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
            // UpdateAnimation();
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
