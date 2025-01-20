using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseLevelController : MonoBehaviour {

    public const uint MAX_RAY_TRACING_COUNT = 1000;

    public const uint MAX_RAY_LEVEL = 7;

    // MARK: Public

    public uint gasCount;

    // Audio
    // When is null, will use the levelClearSoundData field in the SceneAudioManager by default
    // TODO: fix bug for override: "Object reference not set to an instance of an object
    // TODO: see method UpdateIsLevelPassed
    // public SoundData overrideLevelClearSoundData;

    [SerializeField]
    private GameObject mapElementsParent;

    private bool isLevelPassed;

    [Header("Ray Element Prefab")]
    // ray prefab
    public GameObject rayPrefab;

    [Header("Level Choose")]
    public string currentLevelSceneName = null;
    public string nextLevelSceneName = null;
    public string levelChooseSceneName = "Level00_ChooseLevel";

    // MARK: Start Funcs

    protected virtual void Start() {
        InitialMapElements();
        InitialRayDS();
    }

    protected virtual void Update() {
        UpdateValves();
        UpdateRays();
        UpdateIsLevelPassed();

        if (Input.GetKeyDown(KeyCode.R)) {
            StartCoroutine(ResetCurrentLevel());
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            StartCoroutine(LoadLevelChooseScene());
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            QuitGame();
        }
    }

    public IEnumerator LoadLevelChooseScene() {

        yield return DisableGameLogicInAllMapElements();

        SceneManager.UnloadSceneAsync(currentLevelSceneName).completed += (asyncOperation) => {
            SceneManager.LoadSceneAsync(levelChooseSceneName, LoadSceneMode.Additive).completed += (asyncOperation2) => {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelChooseSceneName));
            };
        };

        Debug.Log("Level choose scene is loaded: " + levelChooseSceneName);
    }
    
    public IEnumerator ResetCurrentLevel() {

        yield return DisableGameLogicInAllMapElements();

        SceneManager.UnloadSceneAsync(currentLevelSceneName).completed += (asyncOperation) => {
            SceneManager.LoadSceneAsync(currentLevelSceneName, LoadSceneMode.Additive).completed += (asyncOperation2) => {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentLevelSceneName));
            };
        };

        Debug.Log("Current level is reset: " + currentLevelSceneName);
    }

    public IEnumerator LoadNextLevel() {
        if (nextLevelSceneName == null || nextLevelSceneName == "") {
            Debug.LogWarning("Next level is not set.");
            yield break;
        }
        
        yield return DisableGameLogicInAllMapElements();

        SceneManager.UnloadSceneAsync(currentLevelSceneName).completed += (asyncOperation) => {
            SceneManager.LoadSceneAsync(nextLevelSceneName, LoadSceneMode.Additive).completed += (asyncOperation2) => {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextLevelSceneName));
            };
        };

        Debug.Log("Next level is loaded: " + nextLevelSceneName);
    }

    public bool GetIsLevelPassed() {
        return isLevelPassed;
    }

    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private Dictionary<Vector2Int, BaseMapElement> mapElements;
    private void InitialMapElements() {
        var elements = mapElementsParent.GetComponentsInChildren<BaseMapElement>();
        mapElements = new Dictionary<Vector2Int, BaseMapElement>();
        // var elementsStr = "";
        foreach (var element in elements) {
            var pos_x = (int)(element.transform.position.x + 0.5);
            var pos_y = (int)(element.transform.position.y + 0.5);
            var pos = new Vector2Int(pos_x, pos_y);

            element.position = pos;
            element.SetLevelController(this);

            if (mapElements.ContainsKey(pos)) {
                if (mapElements[pos] is WallMapElement) {
                    Debug.LogWarning("Duplicated map element at " + pos + ". The old one is WallMapElement and remove the old one.");
                    mapElements.Remove(pos);
                    mapElements.Add(pos, element);
                } else if (element is WallMapElement) {
                    Debug.LogWarning("Duplicated map element at " + pos + ". The new one is WallMapElement and remove the new one.");
                } else {
                    Debug.LogError("Duplicated map element at " + pos + ". The old one is " + mapElements[pos].GetMapElementType() + " and the new one is " + element.GetMapElementType() + ". Please fix it!");
                }
            } else {
                mapElements.Add(pos, element);
            }
            // elementsStr += " [" + pos + ": " + element.GetMapElementType() + "]";
        }
        // Debug.Log("Level is initialized with " + elements.Length + " elements. Elements: " + elementsStr);

        GetMapSize();
    }


    private Vector2Int levelSizeBL;
    private Vector2Int levelSizeTR;

    private void GetMapSize() {
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

        levelSizeBL = new Vector2Int(min_x, min_y);
        levelSizeTR = new Vector2Int(max_x, max_y);
    }

    #region Level Controller

    protected IEnumerator DisableGameLogicInAllMapElements() {
        if (mapElementsParent == null || mapElements.Count == 0) {
            yield break;
        }

        var stopDuration = 0.5f;
        var fadeDuration = 1.5f;
        foreach (var element in mapElements.Values) {
            element.DisableGameLogic();
        }
        yield return StartCoroutine(FadeMapElement(mapElementsParent, stopDuration, fadeDuration));

        // yield return WaitForAllCoroutines(coroutines);
    }

    private float FadeTimeMapper(float x) { // x in [0, 1]
        var t = x * Mathf.PI - Mathf.PI / 2; // [-pi/2, pi/2]
        return (Mathf.Sin(t) + 1) / 2; // [0, 1]
    }

    private float Lerp(float x, float y, float t) {
        return x + (y - x) * t;
    }

    protected IEnumerator FadeMapElement(GameObject mapElement, float stopDuration, float fadeDuration) {
        var spriteRenderers = mapElement.GetComponentsInChildren<SpriteRenderer>();
        var textMeshPros = mapElement.GetComponentsInChildren<TextMeshPro>();
        var images = FindObjectsOfType<Image>();
        var texts = FindObjectsOfType<TextMeshProUGUI>();

        for (float time = 0; time < stopDuration; time += Time.deltaTime) {
            yield return null;
        }

        for (float time = 0; time < fadeDuration; time += Time.deltaTime) {
            for (var i = 0; i < spriteRenderers.Length; i++) {
                if (!spriteRenderers[i]) continue;
                var c = spriteRenderers[i].color;
                spriteRenderers[i].color = new Color(c.r, c.g, c.b, Lerp(1, 0, FadeTimeMapper(time / fadeDuration)));
            }
            for (var i = 0; i < textMeshPros.Length; i++) {
                if (!textMeshPros[i]) continue;
                var c = textMeshPros[i].color;
                textMeshPros[i].color = new Color(c.r, c.g, c.b, Lerp(1, 0, FadeTimeMapper(time / fadeDuration)));
            }
            for (var i = 0; i < images.Length; i++) {
                if (!images[i]) continue;
                var c = images[i].color;
                images[i].color = new Color(c.r, c.g, c.b, Lerp(1, 0, FadeTimeMapper(time / fadeDuration)));
            }
            for (var i = 0; i < texts.Length; i++) {
                if (!texts[i]) continue;
                var c = texts[i].color;
                texts[i].color = new Color(c.r, c.g, c.b, Lerp(1, 0, FadeTimeMapper(time / fadeDuration)));
            }
            yield return null;
        }

        for (var i = 0; i < spriteRenderers.Length; i++) {
            if (!spriteRenderers[i]) continue;
            spriteRenderers[i].color = new Color(1, 1, 1, 0);
        }
        for (var i = 0; i < textMeshPros.Length; i++) {
            if (!textMeshPros[i]) continue;
            textMeshPros[i].color = new Color(1, 1, 1, 0);
        }
        for (var i = 0; i < images.Length; i++) {
            if (!images[i]) continue;
            images[i].color = new Color(1, 1, 1, 0);
        }
        for (var i = 0; i < texts.Length; i++) {
            if (!texts[i]) continue;
            texts[i].color = new Color(1, 1, 1, 0);
        }
    }

    public IEnumerator WaitForAllCoroutines(List<IEnumerator> coroutines) {
        var finishedCount = 0;
        var total = coroutines.Count;

        foreach (var routine in coroutines) {
            StartCoroutine(RunRoutine(routine, () => finishedCount++));
        }

        while (finishedCount < total) {
            yield return null;
        }
    }

    private IEnumerator RunRoutine(IEnumerator routine, System.Action onComplete) {
        yield return StartCoroutine(routine);
        onComplete?.Invoke();
    }

    #endregion

    // MARK: Update Funcs

    private bool IsInsideLevel(Vector2Int pos) {
        return pos.x >= levelSizeBL.x && pos.x <= levelSizeTR.x && pos.y >= levelSizeBL.y && pos.y <= levelSizeTR.y;
    }

    private bool IsInsideLevel(int x, int y) {
        return x >= levelSizeBL.x && x <= levelSizeTR.x && y >= levelSizeBL.y && y <= levelSizeTR.y;
    }

    private void UpdateIsLevelPassed() {
        if (isLevelPassed) return;

        foreach (var element in mapElements.Values) {
            if (element is RayReceiverMapElement receiver && !receiver.isRayReceived) {
                return;
            }
        }

        isLevelPassed = true;
        Debug.Log("Level is passed!");
        // PLAY AUDIO: level clear
        // TODO: fix bug for override: "Object reference not set to an instance of an object
        // TODO: see field overrideLevelClearSoundData
        // if (overrideLevelClearSoundData && overrideLevelClearSoundData.clips.Count > 0) {
        //     SoundManager.Instance.CreateSound()
        //         .WithSoundData(overrideLevelClearSoundData)
        //         .Play();
        // } else {
            SoundManager.Instance.CreateSound()
                .WithSoundData(SceneAudioManager.Instance.levelClearSoundData)
                .Play();
        // }

        StartCoroutine(LoadNextLevel());
    }

    #region Scene Controller

    public Result MoveBubble(Vector2Int source, Vector2Int target) {
        var res = IsBubbleMovable(source, target);
        if (!res.isSuc) return res;

        var bubble = mapElements[source] as BubbleMapElement;
        if (!bubble) Assert.IsTrue(false, "Bubble is not found at " + source);

        mapElements.Remove(source);
        mapElements.Add(target, bubble);

        bubble.position = target;
        // bubble.transform.position = new Vector3(target.x, target.y, 0);

        isMapChanged = true;
        return new Result(true, "");
    }

    public Result SplitBubble(Vector2Int source) {
        if (!mapElements.ContainsKey(source)) return new Result(false, "在起始位置找不到气泡哦。");

        var bubble = mapElements[source] as BubbleMapElement;
        if (!bubble) return new Result(false, "在起始位置找不到气泡哦。");

        if (bubble.BubbleSize <= 1) return new Result(false, "气泡大小不足，无法分裂。");

        var dx = new int[] {1, -1, 0, 0};
        var dy = new int[] {0, 0, -1, 1};

        var target = new Vector2Int(-1, -1);
        for (uint i = 0; i < 4; i++) {
            var tmp = new Vector2Int(source.x + dx[i], source.y + dy[i]);
            if (!IsInsideLevel(tmp)) continue;
            if (!mapElements.ContainsKey(tmp)) {
                target = tmp;
                break;
            }
        }

        if (target.x == -1) return new Result(false, "周围没有空位，无法分裂。");

        // 修改旧泡泡
        bubble.BubbleSize -= 1;

        // 增加并初始化新泡泡
        var newBubble = Instantiate(
            bubble.gameObject, new Vector3(target.x, target.y, 0), Quaternion.identity
        ).GetComponent<BubbleMapElement>();

        newBubble.transform.SetParent(mapElementsParent.transform);
        newBubble.position = target;
        newBubble.SetLevelController(this);

        mapElements.Add(target, newBubble);

        isMapChanged = true;
        return new Result(true, "");
    }

    public Result ApplyInflator(Vector2Int inflatorPos) {
        if (!mapElements.ContainsKey(inflatorPos)) return new Result(false, "在起始位置找不到充气机哦。");
        
        var inflator = mapElements[inflatorPos] as InflatorMapElement;
        if (!inflator) return new Result(false, "在起始位置找不到充气机哦。");

        var bubblePos = new Vector2Int(-1, -1);
        if (inflator.direction == EDirection4.Top) {
            bubblePos = new Vector2Int(inflatorPos.x, inflatorPos.y + 1);
        } else if (inflator.direction == EDirection4.Bottom) {
            bubblePos = new Vector2Int(inflatorPos.x, inflatorPos.y - 1);
        } else if (inflator.direction == EDirection4.Left) {
            bubblePos = new Vector2Int(inflatorPos.x - 1, inflatorPos.y);
        } else if (inflator.direction == EDirection4.Right) {
            bubblePos = new Vector2Int(inflatorPos.x + 1, inflatorPos.y);
        } else {
            Assert.IsTrue(false, "Invalid inflator direction: " + inflator.direction);
        }

        if (!mapElements.ContainsKey(bubblePos)) return new Result(false, "在充气机方向找不到气泡哦。");

        var bubble = mapElements[bubblePos] as BubbleMapElement;
        if (!bubble) return new Result(false, "在充气机方向找不到气泡哦。");

        if (gasCount <= 0) return new Result(false, "气体数量不足，无法充气。");
        if (bubble.BubbleThickness <= 1) return new Result(false, "气泡厚度不足，无法充气。");

        gasCount -= 1;

        bubble.BubbleSize += 1;
        bubble.BubbleThickness -= 1;

        isMapChanged = true;
        return new Result(true, "");
    }

    public Result ApplyExtractor(Vector2Int extractorPos) {
        if (!mapElements.ContainsKey(extractorPos)) return new Result(false, "在起始位置找不到抽气机哦。");

        var extractor = mapElements[extractorPos] as ExtractorMapElement;
        if (!extractor) return new Result(false, "在起始位置找不到抽气机哦。");

        var bubblePos = new Vector2Int(-1, -1);
        if (extractor.direction == EDirection4.Top) {
            bubblePos = new Vector2Int(extractorPos.x, extractorPos.y + 1);
        } else if (extractor.direction == EDirection4.Bottom) {
            bubblePos = new Vector2Int(extractorPos.x, extractorPos.y - 1);
        } else if (extractor.direction == EDirection4.Left) {
            bubblePos = new Vector2Int(extractorPos.x - 1, extractorPos.y);
        } else if (extractor.direction == EDirection4.Right) {
            bubblePos = new Vector2Int(extractorPos.x + 1, extractorPos.y);
        } else {
            Assert.IsTrue(false, "Invalid extractor direction: " + extractor.direction);
        }

        if (!mapElements.ContainsKey(bubblePos)) return new Result(false, "在抽气机方向找不到气泡哦。");

        var bubble = mapElements[bubblePos] as BubbleMapElement;
        if (!bubble) return new Result(false, "在抽气机方向找不到气泡哦。");

        if (bubble.BubbleSize <= 1) return new Result(false, "气泡大小不足，无法抽气。");

        gasCount += 1;

        bubble.BubbleSize -= 1;
        bubble.BubbleThickness += 1;

        isMapChanged = true;
        return new Result(true, "");
    }

    private Result IsBubbleMovable(Vector2Int source, Vector2Int target) {
        if (source == target) return new Result(false, "起始位置和目标位置相同。");

        if (mapElements.ContainsKey(target)) return new Result(false, "目标位置已经有其他东西了咯。");
        if (!mapElements.ContainsKey(source)) return new Result(false, "在起始位置找不到气泡哦。");
        
        var bubble = mapElements[source] as BubbleMapElement;
        if (!bubble) return new Result(false, "在起始位置找不到气泡哦。");

        // bfs

        var queue = new Queue<Vector2Int>();
        var visited = new HashSet<Vector2Int>();

        queue.Enqueue(source);
        visited.Add(source);

        var dx = new int[] {0, 0, 1, -1};
        var dy = new int[] {1, -1, 0, 0};

        while(queue.Count > 0) {
            var cur = queue.Dequeue();

            for (uint i = 0; i < 4; i++) {
                var cx = cur.x + dx[i];
                var cy = cur.y + dy[i];

                
                if (!IsInsideLevel(cx, cy)) continue;

                var next = new Vector2Int(cx, cy);
                if (visited.Contains(next)) continue;

                if (next == target) return new Result(true, "");

                if (mapElements.ContainsKey(next)) {
                    var element = mapElements[next];
                    if (element is WallMapElement) continue;
                    if (element is ValveMapElement valve) {
                        if (
                            !valve.isGameLogicActive
                            || !valve.isOpen
                            || bubble.BubbleSize > valve.maxSizeCouldPass
                        ) continue;
                    }
                }

                queue.Enqueue(next);
                visited.Add(next);
            }
        }

        return new Result(false, "气泡无法到达目标位置唔。");
    }

    private void UpdateValves() {
        foreach (var element in mapElements.Values) {
            if (element is ValveMapElement valve) {
                valve.isOpen = false;

                if (!valve.isGameLogicActive) continue;
                
                var pos = valve.position;
                var bubblePos = new Vector2Int(-1, -1);

                if (valve.bubbleDirection == EDirectionDiagnal4.TopLeft) {
                    bubblePos = new Vector2Int(pos.x - 1, pos.y + 1);
                } else if (valve.bubbleDirection == EDirectionDiagnal4.TopRight) {
                    bubblePos = new Vector2Int(pos.x + 1, pos.y + 1);
                } else if (valve.bubbleDirection == EDirectionDiagnal4.BottomLeft) {
                    bubblePos = new Vector2Int(pos.x - 1, pos.y - 1);
                } else if (valve.bubbleDirection == EDirectionDiagnal4.BottomRight) {
                    bubblePos = new Vector2Int(pos.x + 1, pos.y - 1);
                } else {
                    Assert.IsTrue(false, "Invalid valve direction: " + valve.bubbleDirection);
                }

                if (!mapElements.ContainsKey(bubblePos)) continue;

                var bubble = mapElements[bubblePos] as BubbleMapElement;
                if (!bubble) continue;

                valve.isOpen = bubble.BubbleSize >= valve.sizeNeedToOpen;
            }
        }
    }

    #endregion

    #region About Rays

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

    // 只在incident的时候添加 RayElement(实际的可见光线)，outgoing的时候只是逻辑判断
    private void UpdateRays() {
        if (!isMapChanged) {
            return;
        }
        isMapChanged = false;

        // reset bubbles, rayReceivers
        foreach (var element in mapElements.Values) {
            if (element is BubbleMapElement bubble) {
                bubble.BubbleXRay = -1;
                bubble.BubbleYRay = -1;
            } else if (element is RayReceiverMapElement receiver) {
                receiver.isRayReceived = false;
            }
        }

        // remove old rays
        foreach (Transform child in rayParent.transform) {
            Destroy(child.gameObject);
        }

        // init
        raySet = new HashSet<LogicalRayKey>();

        // bfs
        var queue = new List<LogicalRay>();
        var rayElementQ = new List<RayElement>();
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

                PushbackRay(ray, null, source.initialRayLevel, -1, queue, rayElementQ, rayLevelQ, father, ref tail);
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
                        IncidentToOutgoingAndRefract(ray, rayLevel, bubble.BubbleThickness, head, queue, rayElementQ, rayLevelQ, father, ref tail);

                        var nextRayLevel = Math.Min(rayLevel + bubble.BubbleThickness, MAX_RAY_LEVEL);

                        if (ray.rayForwardDirection == EDirection4.Top || ray.rayForwardDirection == EDirection4.Bottom) {
                            bubble.BubbleXRay = (int)nextRayLevel;
                            bubble.BubbleYRay = (int)rayLevel;
                        } else if (ray.rayForwardDirection == EDirection4.Left || ray.rayForwardDirection == EDirection4.Right) {
                            bubble.BubbleXRay = (int)rayLevel;
                            bubble.BubbleYRay = (int)nextRayLevel;
                        } else {
                            Assert.IsTrue(false, "Invalid ray forward direction: " + ray.rayForwardDirection);
                        }

                    } else if (element is ValveMapElement valve) {

                        if (valve.isOpen && valve.isGameLogicActive) { // can pass
                            IncidentToOutgoing(ray, rayLevel, head, queue, rayElementQ, rayLevelQ, father, ref tail);

                        } else { // cannot pass
                            // 被关闭的Valve阻挡，只生成当前incident的光线，而不生成outgoing的光线
                            InstantiateRayElement(ray, rayLevel);
                        }

                    } else if (element is MirrorMapElement mirror) {
                        IncidentToMirror(ray, rayLevel, mirror, head, queue, rayElementQ, rayLevelQ, father, ref tail);

                    } else if (element is RayReceiverMapElement rayReceiver) {

                        if (
                            rayLevel == rayReceiver.targetRayLevel
                            && ray.rayForwardDirection == RayTools.ReverseDirection(rayReceiver.direction)
                        ) {
                            rayReceiver.isRayReceived = true;

                            // trace back 回溯
                            var cur = head;
                            while (cur != -1) {
                                if (rayElementQ[cur] != null) {
                                    rayElementQ[cur].isMatched = true;
                                }
                                cur = father[cur];
                            }

                            // Debug.Log("RayReceiver at " + rayReceiver.position + " with targetLevel " + rayReceiver.targetRayLevel + " MATCHED!");
                        }
                        // InstantiateRayElement(ray, rayLevel);

                    } else {
                        // 被完整方块阻挡，只生成当前incident的光线，而不生成outgoing的光线
                        // TODO: 光线应该比较短或者直接不生成
                        // InstantiateRayElement(ray, rayLevel);

                    }

                } else { // no element
                    IncidentToOutgoing(ray, rayLevel, head, queue, rayElementQ, rayLevelQ, father, ref tail);
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

                    PushbackRay(new_ray, null, rayLevel, head, queue, rayElementQ, rayLevelQ, father, ref tail);
                } else {
                    // Debug.Log("Ray already exists: " + new_ray);
                }
            }
        }

    }

    private void IncidentToOutgoing(
        LogicalRay ray, uint rayLevel, int head, List<LogicalRay> queue, List<RayElement> rayElementQ,
        List<uint> rayLevelQ, List<int> father, ref int tail
    ) {
        var new_ray = new LogicalRay {
            position = ray.position,
            rayType = RayTools.NextRayType(ray.rayType),
            rayForwardDirection = ray.rayForwardDirection
        };

        var key = new LogicalRayKey(new_ray.position, new_ray.rayType);

        if (!raySet.Contains(key)) {
            raySet.Add(key);

            
            var rayE = InstantiateRayElement(ray, rayLevel);
            var rayFE = InstantiateRayElement(new_ray, rayLevel);

            rayElementQ[head] = rayE;
            PushbackRay(new_ray, rayFE, rayLevel, head, queue, rayElementQ, rayLevelQ, father, ref tail);
        } else {
            // Debug.Log("Ray already exists: " + new_ray);
        }
    }

    // IncidentToMirror(ray, rayLevel, mirror, head, queue, rayLevelQ, father, ref tail);
    private void IncidentToMirror(
        LogicalRay ray, uint rayLevel, MirrorMapElement mirror, int head, List<LogicalRay> queue,
        List<RayElement> rayElementQ, List<uint> rayLevelQ, List<int> father, ref int tail
    ) {

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

            var rayE = InstantiateRayElement(ray, rayLevel);
            var rayFE = InstantiateRayElement(new_ray, rayLevel);

            rayElementQ[head] = rayE;
            PushbackRay(new_ray, rayFE, rayLevel, head, queue, rayElementQ, rayLevelQ, father, ref tail);
        } else {
            // Debug.Log("Ray already exists: " + new_ray);
        }
    }

    private void IncidentToOutgoingAndRefract(
        LogicalRay ray, uint rayLevel, uint bubbleThickness, int head, List<LogicalRay> queue,
        List<RayElement> rayElementQ, List<uint> rayLevelQ, List<int> father, ref int tail
    ) {
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

            var nextRayLevel = Math.Min(rayLevel + bubbleThickness, MAX_RAY_LEVEL);

            var rayE = InstantiateRayElement(ray, rayLevel);
            var rayFE = InstantiateRayElement(new_forward_ray, rayLevel);
            var rayRE1 = InstantiateRayElement(new_refraction_ray_1, nextRayLevel);
            var rayRE2 = InstantiateRayElement(new_refraction_ray_2, nextRayLevel);

            rayElementQ[head] = rayE;
            PushbackRay(new_forward_ray, rayFE, rayLevel, head, queue, rayElementQ, rayLevelQ, father, ref tail);
            PushbackRay(new_refraction_ray_1, rayRE1, nextRayLevel, head, queue, rayElementQ, rayLevelQ, father, ref tail);
            PushbackRay(new_refraction_ray_2, rayRE2, nextRayLevel, head, queue, rayElementQ, rayLevelQ, father, ref tail);
        } else {
            // Debug.Log("Ray already exists: " + new_forward_ray);
        }
    }

    private void PushbackRay(
        LogicalRay ray, RayElement rayElement, uint rayLevel, int head, List<LogicalRay> queue, List<RayElement> rayElementQ,
        List<uint> rayLevelQ, List<int> father, ref int tail
    ) {
        queue.Add(ray);
        rayElementQ.Add(rayElement);
        rayLevelQ.Add(rayLevel);
        father.Add(head);
        tail += 1;
    }

    private RayElement InstantiateRayElement(LogicalRay ray, uint rayLevel) {
        var rayElement = Instantiate(
            rayPrefab,
            new Vector3(ray.position.x, ray.position.y, 0),
            Quaternion.identity
        );

        rayElement.transform.SetParent(rayParent.transform);

        var rayElementComponent = rayElement.GetComponent<RayElement>();
        rayElementComponent.position = ray.position;
        rayElementComponent.rayType = ray.rayType;
        rayElementComponent.rayForwardDirection = ray.rayForwardDirection;
        rayElementComponent.isMatched = false;
        rayElementComponent.rayLevel = rayLevel;

        rayElementComponent.UpdateRenderInfo();

        return rayElementComponent;
    }

    #endregion

}
