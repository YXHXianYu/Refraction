using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class RayElement : MonoBehaviour {
    // data
    public Vector2Int position {
        set { logicalRay.position = value; }
        get { return logicalRay.position; }
    }
    public EDirection4 rayForwardDirection {
        get { return logicalRay.rayForwardDirection; }
        set { logicalRay.rayForwardDirection = value; }
    }
    public ERayType rayType {
        get { return logicalRay.rayType; }
        set { logicalRay.rayType = value; }
    }
    public bool isMatched;
    public uint rayLevel;

    // render
    
    public TextMeshPro debugTextLeft;
    public TextMeshPro debugTextRight;
    public TextMeshPro debugTextTop;
    public TextMeshPro debugTextBottom;
    
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private SpriteLibrary _spriteLibrary;
    [SerializeField]
    private GameObject _spriteObject;
    
    private string _currentCategory = "None"; // 默认类别
    
    void UpdateSprite(string category, string label) {
        _spriteRenderer.sprite = _spriteLibrary.GetSprite(category, label);
    }
    
    public void UpdateRenderInfo() {
        debugTextLeft.text = "";
        debugTextRight.text = "";
        debugTextTop.text = "";
        debugTextBottom.text = "";
        string content = "";
        if (rayForwardDirection == EDirection4.Top) {
            content = "↑";
        } else if (rayForwardDirection == EDirection4.Bottom) {
            content = "↓";
        } else if (rayForwardDirection == EDirection4.Left) {
            content = "←";
        } else if (rayForwardDirection == EDirection4.Right) {
            content = "→";
        }
        string spriteCategory = "";
        var curScale = _spriteObject.transform.localScale;
        if (rayType == ERayType.LeftCenter) {
            debugTextLeft.text = content;
            spriteCategory = "Left";
            _spriteObject.transform.localScale = curScale + new Vector3(0, -0.1f, 0);
        } else if (rayType == ERayType.RightCenter) {
            debugTextRight.text = content;
            spriteCategory = "Right";
            _spriteObject.transform.localScale = curScale + new Vector3(0, -0.1f, 0);
        } else if (rayType == ERayType.TopCenter) {
            debugTextTop.text = content;
            spriteCategory = "Up";
            _spriteObject.transform.localScale = curScale + new Vector3(-0.1f, 0, 0);
        } else if (rayType == ERayType.BottomCenter) {
            debugTextBottom.text = content;
            spriteCategory = "Down";
            _spriteObject.transform.localScale = curScale + new Vector3(-0.1f, 0, 0);
        }
        UpdateSprite(spriteCategory, $"L{rayLevel}");
    }

    // logical ray inner
    private readonly LogicalRay logicalRay = new LogicalRay();
}