using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicalRay {
    public Vector2Int position;
    public EDirection4 rayForwardDirection;
    public ERayType rayType;

    // MARK: For HashSet

    public override bool Equals(object other) {
        return Equals(other as LogicalRay);
    }

    public bool Equals(LogicalRay other) {
        if (other == null)
            return false;
        return position == other.position && rayForwardDirection == other.rayForwardDirection && rayType == other.rayType;
    }

    public override int GetHashCode() {
        return System.HashCode.Combine(position, rayForwardDirection, rayType);
    }

    public static bool operator ==(LogicalRay lhs, LogicalRay rhs) {
        if (ReferenceEquals(lhs, rhs))
            return true;
        if (lhs is null || rhs is null)
            return false;
        return lhs.Equals(rhs);
    }

    public static bool operator !=(LogicalRay lhs, LogicalRay rhs) {
        return !(lhs == rhs);
    }
}