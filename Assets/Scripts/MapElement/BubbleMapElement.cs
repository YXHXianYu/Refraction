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
        set { bubbleSize = value; UpdateRenderInfo(); }
    }
    public uint BubbleThickness {
        get { return bubbleThickness; }
        set { bubbleThickness = value; UpdateRenderInfo(); }
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
    private int _outlineColorXiD;
    private int _outlineColorYiD;

    public TextMeshPro debugText;

    // Sound
    public SoundData selectSoundData;
    public SoundData movementSoundData;
    public SoundData splitSoundData;

    public BubbleMapElement(): base(EMapElementType.Bubble) {
    }

    private void Start() {
        InitializeShader();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void InitializeShader() {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer == null) {
            Debug.LogError("SpriteRenderer not found on this GameObject.");
        }
        _outlineColorXiD = Shader.PropertyToID("_OutlineColorX");
        _outlineColorYiD = Shader.PropertyToID("_OutlineColorY");
    }

    private void Update() {
        UpdateSelectionType();
        UpdateRenderInfo();
        // TODO: Fix Shader
        // UpdateMaterial();
        UpdateAnimation();
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
        mat.SetColor(_outlineColorXiD,  ColorTool.GetColor(bubbleXRay));
        mat.SetColor(_outlineColorYiD, ColorTool.GetColor(bubbleYRay));
    }

    private void UpdateAnimation() {
        foreach (var animator in Animators) {
            switch (selectionType) {
                case EOnSelectionType.Hover:
                    animator.SetBool("onHover", true);
                    break;
                case EOnSelectionType.Selected:
                    animator.SetBool("onHover", false);
                    animator.SetBool("isSelected", true);
                    break;
                case EOnSelectionType.Unselected:
                    animator.SetBool("onHover", false);
                    animator.SetBool("isSelected", false);
                    break;
            }
        }
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
        // PLAY SOUND: selectSoundData
        // OnSelect
        if (!isDragging) {
            SoundManager.Instance.CreateSound()
                .WithSoundData(selectSoundData)
                .WithRandomPitch(true)
                .Play();
        }

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

            // PLAY SOUND: selectSoundData
            SoundManager.Instance.CreateSound()
                .WithSoundData(movementSoundData)
                .Play();
        }
    }

    private void OnDoubleClick() {
        var result = levelController.SplitBubble(position);
        if (result.isSuc) {
            Debug.Log("泡泡分裂成功");
            // PLAY SOUND: splitSoundData
            SoundManager.Instance.CreateSound()
                .WithSoundData(splitSoundData)
                .Play();
        } else {
            Debug.Log("泡泡分裂失败：" + result.errMsg);
        }
    }

    #endregion

}
