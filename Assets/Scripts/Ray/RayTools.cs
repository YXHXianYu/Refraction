using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RayTools {

    public static ERayType RayForwardToRayType(EDirection4 direction) {
        switch (direction) {
            case EDirection4.Top:
                return ERayType.TopCenter;
            case EDirection4.Bottom:
                return ERayType.BottomCenter;
            case EDirection4.Left:
                return ERayType.LeftCenter;
            case EDirection4.Right:
                return ERayType.RightCenter;
            default:
                Assert.IsTrue(false, "Invalid direction: " + direction);
                return ERayType.TopCenter;
        }
    }

    public static bool IsRayIncident(LogicalRay ray) {
        switch (ray.rayType) {
            case ERayType.TopCenter:
                return ray.rayForwardDirection == EDirection4.Bottom;
            case ERayType.BottomCenter:
                return ray.rayForwardDirection == EDirection4.Top;
            case ERayType.LeftCenter:
                return ray.rayForwardDirection == EDirection4.Right;
            case ERayType.RightCenter:
                return ray.rayForwardDirection == EDirection4.Left;
            default:
                Assert.IsTrue(false, "Invalid ray type: " + ray.rayType);
                return false;
        }
    }

    public static bool IsRayOutgoing(LogicalRay ray) {
        return !IsRayIncident(ray);
    }

    public static ERayType NextRayType(ERayType rayType) {
        switch (rayType) {
            case ERayType.TopCenter:
                return ERayType.BottomCenter;
            case ERayType.BottomCenter:
                return ERayType.TopCenter;
            case ERayType.LeftCenter:
                return ERayType.RightCenter;
            case ERayType.RightCenter:
                return ERayType.LeftCenter;
            default:
                Assert.IsTrue(false, "Invalid ray type: " + rayType);
                return ERayType.TopCenter;
        }
    }

    // Direction4ToVector2Int
    public static Vector2Int Direction4ToVector2Int(EDirection4 direction) {
        switch (direction) {
            case EDirection4.Top:
                return new Vector2Int(0, 1);
            case EDirection4.Bottom:
                return new Vector2Int(0, -1);
            case EDirection4.Left:
                return new Vector2Int(-1, 0);
            case EDirection4.Right:
                return new Vector2Int(1, 0);
            default:
                Assert.IsTrue(false, "Invalid direction: " + direction);
                return new Vector2Int(0, 0);
        }
    }

    // Direction4ToDirection2
    public static EDirection2 Direction4ToDirection2(EDirection4 direction) {
        switch (direction) {
            case EDirection4.Top:
                return EDirection2.Vertical;
            case EDirection4.Bottom:
                return EDirection2.Vertical;
            case EDirection4.Left:
                return EDirection2.Horizontal;
            case EDirection4.Right:
                return EDirection2.Horizontal;
            default:
                Assert.IsTrue(false, "Invalid direction: " + direction);
                return EDirection2.Vertical;
        }
    }

    public static ERayType NextRefractedRayType1(ERayType rayType) {
        switch (rayType) {
            case ERayType.TopCenter:
                return ERayType.LeftCenter;
            case ERayType.LeftCenter:
                return ERayType.BottomCenter;
            case ERayType.BottomCenter:
                return ERayType.RightCenter;
            case ERayType.RightCenter:
                return ERayType.TopCenter;
            default:
                Assert.IsTrue(false, "Invalid ray type: " + rayType);
                return ERayType.TopCenter;
        }
    }

    public static ERayType NextRefractedRayType2(ERayType rayType) {
        switch (rayType) {
            case ERayType.TopCenter:
                return ERayType.RightCenter;
            case ERayType.RightCenter:
                return ERayType.BottomCenter;
            case ERayType.BottomCenter:
                return ERayType.LeftCenter;
            case ERayType.LeftCenter:
                return ERayType.TopCenter;
            default:
                Assert.IsTrue(false, "Invalid ray type: " + rayType);
                return ERayType.TopCenter;
        }
    }

    public static EDirection4 RayTypeToOutgoindDirection(ERayType rayType) {
        switch (rayType) {
            case ERayType.TopCenter:
                return EDirection4.Top;
            case ERayType.BottomCenter:
                return EDirection4.Bottom;
            case ERayType.LeftCenter:
                return EDirection4.Left;
            case ERayType.RightCenter:
                return EDirection4.Right;
            default:
                Assert.IsTrue(false, "Invalid ray type: " + rayType);
                return EDirection4.Top;
        }
    }

    public static EDirection4 ReverseDirection(EDirection4 direction) {
        switch (direction) {
            case EDirection4.Top:
                return EDirection4.Bottom;
            case EDirection4.Bottom:
                return EDirection4.Top;
            case EDirection4.Left:
                return EDirection4.Right;
            case EDirection4.Right:
                return EDirection4.Left;
            default:
                Assert.IsTrue(false, "Invalid direction: " + direction);
                return EDirection4.Top;
        }
    }
}
