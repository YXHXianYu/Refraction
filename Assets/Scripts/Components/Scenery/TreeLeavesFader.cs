using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TreeLeavesFader : MonoBehaviour {

    private List<SpriteRenderer> spriteRenderers;
    private Coroutine currentCoroutine;

    private Color startColor = Color.white;
    private Color targetColor = new(1, 1, 1, Settings.treeLeavesFadeAlpha);

    void Awake() {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (currentCoroutine != null) {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(FadeToColor(startColor, targetColor, Settings.treeLeavesFadeDuration));
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (currentCoroutine != null) {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(FadeToColor(targetColor, startColor, Settings.treeLeavesFadeDuration));
        }
    }

    IEnumerator FadeToColor(Color startColor, Color targetColor, float duration) {
        float time = 0;
        while (time < duration) {
            spriteRenderers.ForEach(sr => sr.color = Color.Lerp(startColor, targetColor, time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        spriteRenderers.ForEach(sr => sr.color = targetColor);
    }
}
