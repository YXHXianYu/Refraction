using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseMapElement : MonoBehaviour {
    /// <summary>
    /// 该元素的位置
    /// </summary>
    public Vector2Int position;

    /// <summary>
    /// 游戏逻辑上是否激活（用于在特定时刻激活Valve等）
    /// </summary>
    public bool isGameLogicActive;
    
    /// <summary>
    /// 是否可被选中
    /// </summary>
    public bool isSelectable;
    
    /// <summary>
    /// 是否已被选中
    /// </summary>
    public EOnSelectionType selectionType = EOnSelectionType.Unselected;
    
    /// <summary>
    /// 光传输过该格子后的类型
    /// 
    /// LightTransmittanceType:
    /// * Opaque: 不透明
    /// * Transparent: 透明
    /// * Bubble: 泡泡
    /// * Mirror: 镜子
    /// </summary>
    public ELightTransmittanceType lightTransmittanceType;

    /// <summary>
    /// 该地图元素的类型
    /// </summary>
    public EMapElementType mapElementType;

    /// <summary>
    /// 绑定到该物体上的Animator
    /// </summary>
    public RuntimeAnimatorController animatorController;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public BaseMapElement(EMapElementType mapElementType) {
        this.mapElementType = mapElementType;
        lightTransmittanceType = EnumTool.GetLightTransmittanceType(mapElementType);
    }
}
