using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour {

    public CanvasGroup canvasGroup;

    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    private void Awake() {
        if(canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void FadeIn() {
        StopCoroutine(FadeOutCoroutine());
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeOut() {
        StopCoroutine(FadeInCoroutine());
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine() {
        while (canvasGroup.alpha > 0) {
            canvasGroup.alpha -= Time.deltaTime / fadeOutTime;
            yield return null;
        }
    }

    private IEnumerator FadeInCoroutine() {
        while (canvasGroup.alpha < 255) {
            canvasGroup.alpha -= Time.deltaTime / fadeInTime;
            yield return null;
        }
    }
}
