using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InLevelUI : MonoBehaviour {

    public BaseLevelController levelController;
    public TextMeshProUGUI gasCountText;
    public Button menuButton;
    public Button restartButton;

    public void ClickMenu() {
        Debug.LogWarning("ClickMenu() is not implemented yet.");
        // TODO: Implement ClickMenu()
    }

    public void ClickRestart() {
        restartButton.interactable = false;
        StartCoroutine(levelController.ResetCurrentLevel());
    }

    private void Update() {
        gasCountText.text = levelController.gasCount.ToString();
    }

}
