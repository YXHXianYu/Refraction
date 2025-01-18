using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeUI : MonoBehaviour {
    public RectTransform dayNightImage;
    public RectTransform[] clocks;
    public TextMeshProUGUI yearAndSeasonText;
    public TextMeshProUGUI dayText;
    public Image seasonImage;

    public Sprite[] seasonSprites;

    public const float clockUIFadeDuration = Settings.clockUIFadeDuration;
    public const float dayNightImageRotateDuration = Settings.dayNightImageRotateDuration;

    private int lstDayTimePosition = 0;
    private bool[] isClockUIActive = new bool[6]; // 这个数组是有延迟的，表示已经执行操作但不一定完成
    private EventEmitter.EventListener eventListener = null;

    private void OnEnable() {
        eventListener = (args) => { UpdateGameTimeUI(); };
        DefaultEventEmitter.Instance.On("Tick", eventListener);
    }

    private void Start() {
        for (var i = 0; i < clocks.Length; i++) clocks[i].gameObject.SetActive(false);
    }

    private void OnDisable() {
        DefaultEventEmitter.Instance?.Off("Tick", eventListener);
    }

    private void UpdateGameTimeUI() {
        yearAndSeasonText.text =
            "第" + (TimeManager.Instance.Year + 1) + "年 " + TimeManager.Instance.CurrentSeason;
        dayText.text = "第" + (TimeManager.Instance.Day + 1) + "天";

        seasonImage.sprite = seasonSprites[(int)TimeManager.Instance.CurrentSeason];

        var chunk = (int)(1.0f * TimeManager.Instance.TickInDay / (1.0f * TimeManager.tickPerDay / 24.0f));
        // chunk in [0, 23]

        // Debug.Log("chunk: " + chunk + "; TickInDay: " + TimeManager.Instance.TickInDay);

        UpdateClocks(chunk % 6 + 1);

        if (lstDayTimePosition != chunk / 6) {
            lstDayTimePosition = chunk / 6;
            StartCoroutine(
                RotateDayNightImage(dayNightImage, chunk / 6)
            );
        }
    }

    private void UpdateClocks(int clockCount) {
        for (var i = 0; i < clocks.Length; i++)
            if (isClockUIActive[i] != i < clockCount) {
                isClockUIActive[i] = i < clockCount;
                StartCoroutine(
                    FadeClockPart(clocks[i].gameObject, clocks[i].GetComponent<Image>(), i < clockCount)
                );
            }
    }

    private IEnumerator FadeClockPart(GameObject clockPart, Image clockPartImage, bool isToActive) {
        var timer = 0f;
        if (isToActive) {
            clockPart.SetActive(true);
            clockPartImage.color = new Color(1f, 1f, 1f, 0f);
        }

        while (timer < clockUIFadeDuration) {
            timer += Time.deltaTime;
            var alpha = isToActive
                ? Mathf.Lerp(0f, 1f, timer / clockUIFadeDuration)
                : Mathf.Lerp(1f, 0f, timer / clockUIFadeDuration);
            clockPartImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        if (isToActive)
            clockPartImage.color = new Color(1f, 1f, 1f, 1f);
        else
            clockPart.SetActive(false);
    }

    private IEnumerator RotateDayNightImage(RectTransform dayNightImage, int position) {
        var timer = 0f;
        var startAngle = (position - 1) * 90f;
        var endAngle = position * 90f;
        while (timer < dayNightImageRotateDuration) {
            timer += Time.deltaTime;
            var angle = Mathf.Lerp(startAngle, endAngle, timer / dayNightImageRotateDuration);
            dayNightImage.localEulerAngles = new Vector3(0f, 0f, angle);
            yield return null;
        }
    }
}