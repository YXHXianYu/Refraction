using UnityEngine;
using Cinemachine;

public class CameraAspectRatioClamper : MonoBehaviour {
    public CinemachineVirtualCamera virtualCamera;
    public float initialOrthographicSize = 5.3f;
    public float minAspectRatio = 1.6f; // E.g. 16:10
    public float ratioScaleCoefficient = 1.0f;

    private void Update() {
        var currentAspect = (float)Screen.width / Screen.height;
        var ratio = 1.0f;
        var lens = virtualCamera.m_Lens;
        if (currentAspect < minAspectRatio) {
            ratio = minAspectRatio / currentAspect;
        }
        lens.OrthographicSize = initialOrthographicSize * Mathf.Pow(ratio, ratioScaleCoefficient); 
        virtualCamera.m_Lens = lens;
    }
}
