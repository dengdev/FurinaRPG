using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : MonoBehaviour {
    private CanvasGroup canvasGroup;

    [SerializeField] private float fadeInDuration = 2.0f;
    [SerializeField] private float fadeOutDuration = 1.5f;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(this.gameObject);
    }

    public IEnumerator FadeOutBeforeIn() {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }

    public IEnumerator FadeOut(float time) {
        while (canvasGroup.alpha < 1) {
            canvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }
    public IEnumerator FadeIn(float time) {
        while (canvasGroup.alpha > 0) {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
        canvasGroup.alpha = 0;
        Destroy(gameObject);
    }
}
