using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumTool {
    public static Dictionary<EMapElementType, ELightTransmittanceType> MapElementTypeToLightTransmittanceType
            = new Dictionary<EMapElementType, ELightTransmittanceType> {
                { EMapElementType.Bubble, ELightTransmittanceType.Bubble },
                { EMapElementType.Inflator, ELightTransmittanceType.Opaque },
                { EMapElementType.Extractor, ELightTransmittanceType.Opaque },
                { EMapElementType.Valve, ELightTransmittanceType.Transparent },
                { EMapElementType.Wall, ELightTransmittanceType.Opaque },
                { EMapElementType.RaySource, ELightTransmittanceType.Opaque },
                { EMapElementType.RayReceiver, ELightTransmittanceType.Opaque },
                { EMapElementType.Mirror, ELightTransmittanceType.Mirror },
                { EMapElementType.Ray, ELightTransmittanceType.Transparent }
        };
    
    public static ELightTransmittanceType GetLightTransmittanceType(EMapElementType mapElementType) {
        return MapElementTypeToLightTransmittanceType[mapElementType];
    }
}
