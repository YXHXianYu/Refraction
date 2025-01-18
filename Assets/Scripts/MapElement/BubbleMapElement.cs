using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class BubbleMapElement : BaseMapElement {
    
    public uint bubbleSize;
    public uint bubbleThickness;
    
    // -1 means no ray pass; 0~inf means where the ray pass
    [SerializeField] private int bubbleXRay = -1; // Left and Right
    [SerializeField] private int bubbleYRay = -1; // Top and Down
    
    // Shader Util
    private SpriteRenderer _spriteRenderer;
    private int _centerPosXiD;
    private int _centerPosYiD;
    private int _outlineColorXiD;
    private int _outlineColorYiD;

    public BubbleMapElement(): base(EMapElementType.Bubble) {
    }


    public void OnPointerClick(PointerEventData eventData) {
    }

    private bool isDragging = false;
    private Vector2Int startPosition;

    public void OnMouseDown() {
        isDragging = true;
        startPosition = position;
    }

    public void OnMouseDrag() {
        if (!isDragging) {
            return;
        }

        var world_pos = MouseTools.GetMouseWorldPosition();
        transform.position = new Vector3(world_pos.x, world_pos.y, 0);
    }

    public void OnMouseUp() {
        isDragging = false;

        var world_pos = MouseTools.GetMouseWorldPosition();
        transform.position = new Vector3(world_pos.x, world_pos.y, 0);

        var source = new Vector2Int((int)(startPosition.x + 0.5f), (int)(startPosition.y + 0.5f));
        var target = new Vector2Int((int)(world_pos.x + 0.5f), (int)(world_pos.y + 0.5f));

        if (source == target) {
            return;
        }

        var result = levelController.MoveBubble(source, target);

        if (!result.isSuc) {
            Debug.Log("泡泡移动的失败原因：" + result.errMsg);
            position = startPosition;
            transform.position = new Vector3((int)(startPosition.x + 0.5f), (int)(startPosition.y + 0.5f), 0);
        } else {
            position = target;
            transform.position = new Vector3((int)(target.x + 0.5f), (int)(target.y + 0.5f), 0);
        }
    }

    private void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) {
            Debug.LogError("SpriteRenderer not found on this GameObject.");
        }
        _centerPosXiD = Shader.PropertyToID("_CenterPosX");
        Debug.Log("Shader _CenterPosYid:" + _centerPosXiD);
        _centerPosYiD = Shader.PropertyToID("_CenterPosY");
        _outlineColorXiD = Shader.PropertyToID("_OutlineColorX");
        _outlineColorYiD = Shader.PropertyToID("_OutlineColorY");
    }

    private void Update() {
        UpdateMaterial();
    }

    private void UpdateMaterial() {
        var mat = _spriteRenderer.material;
        mat.SetFloat(_centerPosXiD, transform.position.x);
        mat.SetFloat(_centerPosYiD, transform.position.y);
        mat.SetColor(_outlineColorXiD, ColorTool.GetColor(bubbleXRay));
        mat.SetColor(_outlineColorYiD, ColorTool.GetColor(bubbleYRay));
        // mat.SetColor("_OutlineColorX", Color.clear);
        // mat.SetColor("_OutlineColorY", Color.clear);
    }

    private void UpdateAnimation() {
        
    }
}
