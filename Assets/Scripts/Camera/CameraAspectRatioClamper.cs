using UnityEngine;
using Cinemachine;

public class CameraAspectRatioClamper : MonoBehaviour {
    public CinemachineVirtualCamera virtualCamera;
    public float minAspectRatio = 1.6f; // E.g. 16:10

    private void Update() {
        var currentAspect = (float)Screen.width / Screen.height;
        if (currentAspect < minAspectRatio) {
            var ratio = minAspectRatio / currentAspect;
            var lens = virtualCamera.m_Lens;
            lens.Orthographic = true;
            lens.OrthographicSize *= ratio; 
            virtualCamera.m_Lens = lens;
            Debug.Log("Current aspect ratio: " + currentAspect + "; Clamped to: " + minAspectRatio);
        } else {
            Debug.Log("Current aspect ratio: " + currentAspect + "; No clamping needed.");
        }
    }
}
