using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class BubbleMapElement : BaseMapElement {

    public uint BubbleSize {
        get { return bubbleSize; }
        set { bubbleSize = value; }
    }
    public uint BubbleThickness {
        get { return bubbleThickness; }
        set { bubbleThickness = value; }
    }
    public int BubbleXRay {
        get { return bubbleXRay; }
        set { bubbleXRay = value; }
    }
    public int BubbleYRay {
        get { return bubbleYRay; }
        set { bubbleYRay = value; }
    }
    
    [SerializeField]
    private uint bubbleSize = 1;
    [SerializeField]
    private uint bubbleThickness = 1;
    
    // -1 means no ray pass; 0~inf means where the ray pass
    [SerializeField]
    private int bubbleXRay = -1; // Left and Right
    [SerializeField]
    private int bubbleYRay = -1; // Top and Down
    
    // Shader Util
    private SpriteRenderer _spriteRenderer;
    private int _centerPosXiD;
    private int _centerPosYiD;
    private int _outlineColorXiD;
    private int _outlineColorYiD;

    public TextMeshPro debugText;

    public BubbleMapElement(): base(EMapElementType.Bubble) {
    }

    private void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) {
            Debug.LogError("SpriteRenderer not found on this GameObject.");
        }
        _centerPosXiD = Shader.PropertyToID("_CenterPosX");
        _centerPosYiD = Shader.PropertyToID("_CenterPosY");
        _outlineColorXiD = Shader.PropertyToID("_OutlineColorX");
        _outlineColorYiD = Shader.PropertyToID("_OutlineColorY");
    }

    private void Update() {
        UpdateSelectionType();
        UpdateRenderInfo();
        UpdateMaterial();
    }

    public void UpdateRenderInfo() {
        debugText.text = bubbleSize + "|" + bubbleThickness;

        if (selectionType == EOnSelectionType.Selected) {
            debugText.text += "|S";
        } else if (selectionType == EOnSelectionType.Hover) {
            debugText.text += "|H";
        } else {
            debugText.text += "|U";
        }
    }

    private void UpdateMaterial() {
        var mat = _spriteRenderer.material;
        mat.SetFloat(_centerPosXiD, transform.position.x);
        mat.SetFloat(_centerPosYiD, transform.position.y);
        mat.SetColor(_outlineColorXiD,  ColorTool.GetColor(bubbleXRay));
        // Debug.Log("OutlineColorX: " + mat.GetColor(_outlineColorXiD));
        mat.SetColor(_outlineColorYiD, ColorTool.GetColor(bubbleYRay));
        
        // mat.SetColor("_OutlineColorX", Color.clear);
        // mat.SetColor("_OutlineColorY", Color.clear);
    }

    private void UpdateAnimation() {
        
    }

    private void UpdateSelectionType() {
        Assert.IsTrue(isSelectable, "BubbleMapElement must be selectable.");

        var mouseWorldPos = MouseTools.GetMouseWorldPosition();
        bool isInCurrentElement =
            mouseWorldPos.x < transform.position.x - 0.5f
            || mouseWorldPos.x > transform.position.x + 0.5f
            || mouseWorldPos.y < transform.position.y - 0.5f
            || mouseWorldPos.y > transform.position.y + 0.5f;

        if (isInCurrentElement) {
            selectionType = EOnSelectionType.Unselected;
        } else if (isDragging) {
            selectionType = EOnSelectionType.Selected;
        } else {
            selectionType = EOnSelectionType.Hover;
        }
    }

    #region Event

    private float lastClickTime = -OurSettings.DOUBLE_CLICK_TIME;

    private bool isDragging = false;
    private Vector2Int startPosition;

    public void OnMouseDown() {
        isDragging = true;
        startPosition = position;

        if (Time.time - lastClickTime < OurSettings.DOUBLE_CLICK_TIME) {
            OnDoubleClick();
            lastClickTime = -OurSettings.DOUBLE_CLICK_TIME;
        } else {
            lastClickTime = Time.time;
        }
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
            transform.position = new Vector3((int)(startPosition.x + 0.5f), (int)(startPosition.y + 0.5f), 0);
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

    private void OnDoubleClick() {
        var result = levelController.SplitBubble(position);
        if (result.isSuc) {
            Debug.Log("泡泡分裂成功");
        } else {
            Debug.Log("泡泡分裂失败：" + result.errMsg);
        }
    }


    #endregion
}
