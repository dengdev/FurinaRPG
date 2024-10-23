using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MessageAnimator : MonoBehaviour {
    [SerializeField] private float moveDistance = 150f; // 移动的距离
    [SerializeField] private float fadeOutTime = 0.5f; // 淡出时间
    [SerializeField] private float stayDuration = 2f; // 停留时间

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public System.Action OnMessageRemoved; // 消息移除时的事件
    private bool isAnimating = false;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void PlayMessageAnimation(float initialY) {
        isAnimating = true; // 动画开始时设置为true
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, initialY, 0);

        rectTransform.DOLocalMoveY(initialY + moveDistance, fadeOutTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                isAnimating = false; // 动画完成时设置为false
                StartCoroutine(StayAndFadeOut());
            });
    }

    private IEnumerator StayAndFadeOut() {
        yield return new WaitForSeconds(stayDuration);

        Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.Append(canvasGroup.DOFade(0, fadeOutTime).SetEase(Ease.InQuad));
        fadeOutSequence.Join(rectTransform.DOLocalMoveX(rectTransform.localPosition.x - 100f, fadeOutTime).SetEase(Ease.InQuad));

        // 等待淡出动画完成
        yield return fadeOutSequence.WaitForCompletion();

        // 调用事件以通知移除
        OnMessageRemoved?.Invoke();
    }
    public void StopAnimation() {
        if (isAnimating) {
            DOTween.Kill(this);
            isAnimating = false; // 动画停止时也设置为false
        }
    }
}
