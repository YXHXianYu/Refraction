using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Seed, Commodity, Furniture,
    HoeTool, ChopTool, BreakTool, ReapTool, WaterTool, CollectTool,
    ReapableScenery
}

public enum SlotType {
    PlayerBackpack,
}

public enum AnimatorChangingItemType {
    None, Carry, 
}

public enum AnimatorChangingBodyType {
    Body, Hair, Arm, Tool,
}

public enum Season {
    春天, 夏天, 秋天, 冬天
}