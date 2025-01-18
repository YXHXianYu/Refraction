using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExtractorMapElement : BaseMapElement {
    public EDirection4 direction;

    public ExtractorMapElement(): base(EMapElementType.Extractor) {
    }

    public void ExtractBubble(BubbleMapElement bubble) {
        // TODO: Implement this method
        Debug.LogError("InflatorMapElement.InflatBubble() not implemented");
    }
}
