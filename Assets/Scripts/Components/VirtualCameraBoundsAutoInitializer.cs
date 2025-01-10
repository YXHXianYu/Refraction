using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraBoundsAutoInitializer : MonoBehaviour {
    void Start() {
        UpdateConfiner();
    }

    // TODO: UpdateConfiner when switching scenes

    private void UpdateConfiner() {
        // 1. 获取对应Collider2D
        PolygonCollider2D collider = GameObject.FindGameObjectWithTag("VirtualCameraBounds").GetComponent<PolygonCollider2D>();
        // 2. 获取对应的CinemachineConfiner
        if (!TryGetComponent<CinemachineConfiner>(out var confiner)) {
            Debug.LogError("CinemachineConfiner is not found.");
        }
        // 3. 设置Collider2D的边界
        confiner.m_BoundingShape2D = collider;
        // 4. 刷新缓存
        confiner.InvalidatePathCache();
    }
}
