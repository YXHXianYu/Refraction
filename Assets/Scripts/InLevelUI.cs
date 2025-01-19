using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InLevelUI : MonoBehaviour {

    public BaseLevelController levelController;
    public TextMeshProUGUI gasCountText;
    public TextMeshProUGUI levelNameText;
    public Button menuButton;
    public Button restartButton;

    private void Start() {
        var t = levelController.currentLevelSceneName.ToUpper();
        levelNameText.text = "#" + t[^1];
    }

    public void ClickMenu() {
        menuButton.interactable = false;
        StartCoroutine(levelController.LoadLevelChooseScene());
    }

    public void ClickRestart() {
        restartButton.interactable = false;
        StartCoroutine(levelController.ResetCurrentLevel());
    }

    private void Update() {
        gasCountText.text = levelController.gasCount.ToString();
    }

}
