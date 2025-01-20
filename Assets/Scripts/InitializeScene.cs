using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeScene : MonoBehaviour {

    public string firstSceneName;

    private void Awake() {
        if (SceneManager.sceneCount == 1) {
            SceneManager.LoadSceneAsync(firstSceneName, LoadSceneMode.Additive).completed += (asyncOperation) => {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(firstSceneName));
            };
        }
    }

}
