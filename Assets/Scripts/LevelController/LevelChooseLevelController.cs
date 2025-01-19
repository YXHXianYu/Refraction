using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class RayReceiverAndLevel {
    public RayReceiverMapElement rayReceiver;
    public string levelName;
}

public class LevelChooseLevelController : BaseLevelController {

    [Header("Level Choose")]
    public string levelChooseSceneName = "Level00_ChooseLevel";
    public List<RayReceiverAndLevel> levelList = new List<RayReceiverAndLevel>();

    protected override void Update() {
        base.Update();

        foreach (var rcvAndLevel in levelList) {
            var rayReceiver = rcvAndLevel.rayReceiver;
            var levelName = rcvAndLevel.levelName;
            if (rayReceiver.isRayReceived) {

                SceneManager.UnloadSceneAsync(levelChooseSceneName).completed += (asyncOperation) => {
                    SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
                };
                return;
            }
        }
    }

}
