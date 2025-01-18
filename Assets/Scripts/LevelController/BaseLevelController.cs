using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements.Experimental;

public class BaseLevelController : MonoBehaviour {

    public const uint MAX_RAY_TRACING_COUNT = 1000;

    // MARK: Public

    public Vector2Int levelSize;
    public uint gasCount;

    [SerializeField]
    private GameObject mapElementsParent;

    // ray prefab
    public GameObject rayPrefab;

    // MARK: Start Funcs

    private void Start() {
        InitialMapElements();
        InitialRayDS();
    }

    private Dictionary<Vector2Int, BaseMapElement> mapElements;
    private void InitialMapElements() {
        var elements = mapElementsParent.GetComponentsInChildren<BaseMapElement>();
        mapElements = new Dictionary<Vector2Int, BaseMapElement>();
        var elementsStr = "";
        foreach (var element in elements) {
            var pos_x = (int)(element.transform.position.x + 0.5);
            var pos_y = (int)(element.transform.position.y + 0.5);
            var pos = new Vector2Int(pos_x, pos_y);
            element.position = pos;
            mapElements.Add(pos, element);
            elementsStr += " [" + pos + ": " + element.GetMapElementType() + "]";
        }
        Debug.Log("Level is initialized with " + elements.Length + " elements. Elements: " + elementsStr);

        CheckMapSize();
    }

    private void CheckMapSize() {
        if (mapElements == null) {
            Debug.LogWarning("MapElements is null, but CheckMapSize is called.");
        }

        // traverse mapElements
        var max_x = -1;
        var max_y = -1;
        var min_x = int.MaxValue;
        var min_y = int.MaxValue;
        foreach (var pos in mapElements.Keys) {
            max_x = Mathf.Max(max_x, pos.x);
            max_y = Mathf.Max(max_y, pos.y);
            min_x = Mathf.Min(min_x, pos.x);
            min_y = Mathf.Min(min_y, pos.y);
        }
        if (!(min_x == 0 && min_y == 0 && max_x == levelSize.x - 1 && max_y == levelSize.y - 1)) {
            Debug.LogError("Map size is not correct. levelSize: " + levelSize + "; max_x/y: " + max_x + "/" + max_y + "; min_x/y: " + min_x + "/" + min_y);
        }
    }


    private class LogicalRayKey {
        public Vector2Int position;
        public ERayType direction;

        public LogicalRayKey(Vector2Int position, ERayType direction) {
            this.position = position;
            this.direction = direction;
        }

        public override bool Equals(object other) {
            return Equals(other as LogicalRayKey);
        }

        public bool Equals(LogicalRayKey other) {
            if (other == null)
                return false;
            return position == other.position && direction == other.direction;
        }

        public override int GetHashCode() {
            return System.HashCode.Combine(position, direction);
        }

        public static bool operator ==(LogicalRayKey lhs, LogicalRayKey rhs) {
            if (ReferenceEquals(lhs, rhs))
                return true;
            if (lhs is null || rhs is null)
                return false;
            return lhs.Equals(rhs);
        }

        public static bool operator !=(LogicalRayKey lhs, LogicalRayKey rhs) {
            return !(lhs == rhs);
        }
    }

    private bool isMapChanged = true;
    private GameObject rayParent;
    private HashSet<LogicalRayKey> raySet;
    private void InitialRayDS() {
        rayParent = new GameObject("RayParent");
        rayParent.transform.SetParent(mapElementsParent.transform);

        raySet = new HashSet<LogicalRayKey>();
    }

    // MARK: Update Funcs

    private void Update() {
        UpdateMouseControl();
        UpdateRays();
    }

    private void UpdateMouseControl() {
        if (Input.GetMouseButtonDown(0)) {
            var mouse_pos = new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.transform.position.z
            );
            var world_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

            var pos = new Vector2Int((int)(world_pos.x + 0.5), (int)(world_pos.y + 0.5));
            if (mapElements.ContainsKey(pos)) {
                var element = mapElements[pos];
                Debug.Log("Mouse clicked, " + pos + ": " + element.GetMapElementType());
                MapElementIsClicked(element);
            } else {
                Debug.Log("Mouse clicked, but no element found at " + pos);
            }
        }
    }


    // MARK: Other Funcs

    private void MapElementIsClicked(BaseMapElement element) {

        // TODO: isMapChanged = true;

        Debug.Log("Map element is clicked: " + element.GetMapElementType());
        if (element is ValveMapElement valve) {
            // TODO: 
        }
    }


    #region Update Rays

    // 只在incident的时候添加 RayElement(实际的可见光线)，outgoing的时候只是逻辑判断
    private void UpdateRays() {
        if (!isMapChanged) {
            return;
        }
        isMapChanged = false;

        // init
        raySet = new HashSet<LogicalRayKey>();

        // bfs
        var queue = new List<LogicalRay>();
        var rayLevelQ = new List<uint>();
        var father = new List<int>(); // for 回溯
        var tail = 0;

        // init
        foreach (var element in mapElements.Values) {
            if (element is RaySourceMapElement source) {
                // var source = element as RaySourceMapElement;
                var ray = new LogicalRay {
                    position = element.position,
                    rayType = RayTools.RayForwardToRayType(source.direction),
                    rayForwardDirection = source.direction
                };

                PushbackRay(ray, source.initialRayLevel, -1, queue, rayLevelQ, father, ref tail);
            }
        }

        // traverse
        for (var head = 0; head < tail && head < MAX_RAY_TRACING_COUNT; head++) {
            var ray = queue[head];
            var rayLevel = rayLevelQ[head];

            // Debug.Log("Head: " + head + "; Tail: " + tail);

            if (RayTools.IsRayIncident(ray)) {
                var cur_pos = ray.position;
                if (mapElements.ContainsKey(cur_pos)) {
                    var element = mapElements[cur_pos];

                    if (element is WallMapElement wall) {
                        // 被墙阻挡，只生成当前incident的光线，而不生成outgoing的光线
                        InstantiateRayElement(ray, rayLevel);

                    } else if (element is BubbleMapElement bubble) {
                        IncidentToOutgoingAndRefract(ray, rayLevel, bubble.bubbleThickness, head, queue, rayLevelQ, father, ref tail);

                    } else if (element is ValveMapElement valve) {

                        if (valve.isOpen && valve.isGameLogicActive) { // can pass
                            IncidentToOutgoing(ray, rayLevel, head, queue, rayLevelQ, father, ref tail);

                        } else { // cannot pass
                            // 被关闭的Valve阻挡，只生成当前incident的光线，而不生成outgoing的光线
                            InstantiateRayElement(ray, rayLevel);
                        }

                    } else if (element is MirrorMapElement mirror) {
                        IncidentToMirror(ray, rayLevel, mirror, head, queue, rayLevelQ, father, ref tail);

                    } else {
                        // 被完整方块阻挡，只生成当前incident的光线，而不生成outgoing的光线
                        // TODO: 光线应该比较短或者直接不生成
                        InstantiateRayElement(ray, rayLevel);

                    }

                } else { // no element
                    IncidentToOutgoing(ray, rayLevel, head, queue, rayLevelQ, father, ref tail);
                }

            } else { // outgoing
                var new_ray = new LogicalRay {
                    position = ray.position + RayTools.Direction4ToVector2Int(ray.rayForwardDirection),
                    rayType = RayTools.NextRayType(ray.rayType),
                    rayForwardDirection = ray.rayForwardDirection
                };

                var key = new LogicalRayKey(new_ray.position, new_ray.rayType);

                if (!raySet.Contains(key)) {
                    raySet.Add(key);

                    PushbackRay(new_ray, rayLevel, head, queue, rayLevelQ, father, ref tail);
                } else {
                    Debug.Log("Ray already exists: " + new_ray);
                }
            }
        }

    }

    private void IncidentToOutgoing(LogicalRay ray, uint rayLevel, int head, List<LogicalRay> queue, List<uint> rayLevelQ, List<int> father, ref int tail) {
        var new_ray = new LogicalRay {
            position = ray.position,
            rayType = RayTools.NextRayType(ray.rayType),
            rayForwardDirection = ray.rayForwardDirection
        };

        var key = new LogicalRayKey(new_ray.position, new_ray.rayType);

        if (!raySet.Contains(key)) {
            raySet.Add(key);

            InstantiateRayElement(ray, rayLevel);
            InstantiateRayElement(new_ray, rayLevel);

            PushbackRay(new_ray, rayLevel, head, queue, rayLevelQ, father, ref tail);
        } else {
            Debug.Log("Ray already exists: " + new_ray);
        }
    }

    // IncidentToMirror(ray, rayLevel, mirror, head, queue, rayLevelQ, father, ref tail);
    private void IncidentToMirror(LogicalRay ray, uint rayLevel, MirrorMapElement mirror, int head, List<LogicalRay> queue, List<uint> rayLevelQ, List<int> father, ref int tail) {

        ERayType rayType;
        if (mirror.direction == EDirectionDiagnal4.TopLeft) {
            if (ray.rayType == ERayType.TopCenter) {
                rayType = ERayType.LeftCenter;
            } else if (ray.rayType == ERayType.LeftCenter) {
                rayType = ERayType.TopCenter;
            } else {
                return;
            }
        } else if (mirror.direction == EDirectionDiagnal4.TopRight) {
            if (ray.rayType == ERayType.TopCenter) {
                rayType = ERayType.RightCenter;
            } else if (ray.rayType == ERayType.RightCenter) {
                rayType = ERayType.TopCenter;
            } else {
                return;
            }
        } else if (mirror.direction == EDirectionDiagnal4.BottomLeft) {
            if (ray.rayType == ERayType.BottomCenter) {
                rayType = ERayType.LeftCenter;
            } else if (ray.rayType == ERayType.LeftCenter) {
                rayType = ERayType.BottomCenter;
            } else {
                return;
            }
        } else if (mirror.direction == EDirectionDiagnal4.BottomRight) {
            if (ray.rayType == ERayType.BottomCenter) {
                rayType = ERayType.RightCenter;
            } else if (ray.rayType == ERayType.RightCenter) {
                rayType = ERayType.BottomCenter;
            } else {
                return;
            }
        } else {
            Assert.IsTrue(false, "Invalid mirror direction: " + mirror.direction);
            return;
        }

        var new_ray = new LogicalRay {
            position = ray.position,
            rayType = rayType,
            rayForwardDirection = RayTools.RayTypeToOutgoindDirection(rayType)
        };

        var key = new LogicalRayKey(new_ray.position, new_ray.rayType);

        if (!raySet.Contains(key)) {
            raySet.Add(key);

            InstantiateRayElement(ray, rayLevel);
            InstantiateRayElement(new_ray, rayLevel);

            PushbackRay(new_ray, rayLevel, head, queue, rayLevelQ, father, ref tail);
        } else {
            Debug.Log("Ray already exists: " + new_ray);
        }
    }

    private void IncidentToOutgoingAndRefract(LogicalRay ray, uint rayLevel, uint bubbleThickness, int head, List<LogicalRay> queue, List<uint> rayLevelQ, List<int> father, ref int tail) {
        var new_forward_ray = new LogicalRay {
            position = ray.position,
            rayType = RayTools.NextRayType(ray.rayType),
            rayForwardDirection = ray.rayForwardDirection
        };
        var new_type_1 = RayTools.NextRefractedRayType1(ray.rayType);
        var new_refraction_ray_1 = new LogicalRay {
            position = ray.position,
            rayType = new_type_1,
            rayForwardDirection = RayTools.RayTypeToOutgoindDirection(new_type_1)
        };
        var new_type_2 = RayTools.NextRefractedRayType2(ray.rayType);
        var new_refraction_ray_2 = new LogicalRay {
            position = ray.position,
            rayType = new_type_2,
            rayForwardDirection = RayTools.RayTypeToOutgoindDirection(new_type_2)
        };

        var key_forward = new LogicalRayKey(new_forward_ray.position, new_forward_ray.rayType);
        var key_refraction = new LogicalRayKey(new_refraction_ray_1.position, new_refraction_ray_1.rayType);

        if (!raySet.Contains(key_forward) && !raySet.Contains(key_refraction)) {
            raySet.Add(key_forward);
            raySet.Add(key_refraction);

            InstantiateRayElement(ray, rayLevel);
            InstantiateRayElement(new_forward_ray, rayLevel);
            InstantiateRayElement(new_refraction_ray_1, rayLevel + bubbleThickness);
            InstantiateRayElement(new_refraction_ray_2, rayLevel + bubbleThickness);

            PushbackRay(new_forward_ray, rayLevel, head, queue, rayLevelQ, father, ref tail);
            PushbackRay(new_refraction_ray_1, rayLevel + bubbleThickness, head, queue, rayLevelQ, father, ref tail);
            PushbackRay(new_refraction_ray_2, rayLevel + bubbleThickness, head, queue, rayLevelQ, father, ref tail);
        } else {
            Debug.Log("Ray already exists: " + new_forward_ray);
        }
    }

    private void PushbackRay(LogicalRay ray, uint rayLevel, int head, List<LogicalRay> queue, List<uint> rayLevelQ, List<int> father, ref int tail) {
        queue.Add(ray);
        rayLevelQ.Add(rayLevel);
        father.Add(head);
        tail += 1;
    }

    private void InstantiateRayElement(LogicalRay ray, uint rayLevel) {
        var rayElement = Instantiate(
            rayPrefab,
            new Vector3(ray.position.x, ray.position.y, 0),
            Quaternion.identity
        );

        var rayElementComponent = rayElement.GetComponent<RayElement>();
        rayElementComponent.position = ray.position;
        rayElementComponent.rayType = ray.rayType;
        rayElementComponent.rayForwardDirection = ray.rayForwardDirection;
        rayElementComponent.isMatch = false;
        rayElementComponent.rayLevel = rayLevel;

        rayElementComponent.UpdateRenderInfo();
    }

    #endregion

}
