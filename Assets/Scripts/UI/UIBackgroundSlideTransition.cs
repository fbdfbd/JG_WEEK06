using System;
using System.Collections;
using UnityEngine;

public enum SlideDirection
{
    FromRightToLeft,
    FromLeftToRight,
}

public class UIBackgroundSlideTransition : MonoBehaviour
{
    [SerializeField] private RectTransform viewport; // 보통 BackgroundRoot
    [SerializeField, Min(0f)] private float duration = 0.5f;
    [SerializeField] private SlideDirection direction = SlideDirection.FromRightToLeft;
    [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine playingRoutine;

    public bool IsPlaying => playingRoutine != null;

    public void Play(
        RectTransform current,
        Vector2 currentBasePosition,
        RectTransform next,
        Vector2 nextBasePosition,
        Action onFinished)
    {
        if (current == null || next == null)
        {
            onFinished?.Invoke();
            return;
        }

        if (current.parent != next.parent)
        {
            Debug.LogWarning("배경 UI들은 반드시 같은 부모 아래에 있어야 합니다.");
            onFinished?.Invoke();
            return;
        }

        if (playingRoutine != null)
        {
            StopCoroutine(playingRoutine);
            playingRoutine = null;
        }

        playingRoutine = StartCoroutine(
            PlayRoutine(current, currentBasePosition, next, nextBasePosition, onFinished));
    }

    private IEnumerator PlayRoutine(
        RectTransform current,
        Vector2 currentBasePosition,
        RectTransform next,
        Vector2 nextBasePosition,
        Action onFinished)
    {
        RectTransform parentRect = current.parent as RectTransform;

        if (viewport != null)
            viewport.ForceUpdateRectTransforms();

        if (parentRect != null)
            parentRect.ForceUpdateRectTransforms();

        float width = GetSlideWidth(parentRect);
        Vector2 offset = GetOffset(width);

        current.gameObject.SetActive(true);
        next.gameObject.SetActive(true);

        // 중요: 새 배경을 제일 위로
        next.SetAsLastSibling();

        // 시작 위치 세팅
        current.anchoredPosition = currentBasePosition;
        next.anchoredPosition = nextBasePosition + offset;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            float easedT = easing.Evaluate(t);

            current.anchoredPosition = Vector2.LerpUnclamped(
                currentBasePosition,
                currentBasePosition - offset,
                easedT);

            next.anchoredPosition = Vector2.LerpUnclamped(
                nextBasePosition + offset,
                nextBasePosition,
                easedT);

            yield return null;
        }

        // 최종 정리
        current.anchoredPosition = currentBasePosition;
        next.anchoredPosition = nextBasePosition;

        current.gameObject.SetActive(false);
        next.gameObject.SetActive(true);

        // 끝난 뒤에도 현재 배경은 맨 위 유지
        next.SetAsLastSibling();

        playingRoutine = null;
        onFinished?.Invoke();
    }

    private float GetSlideWidth(RectTransform parentRect)
    {
        if (viewport != null)
            return viewport.rect.width;

        if (parentRect != null)
            return parentRect.rect.width;

        return Screen.width;
    }

    private Vector2 GetOffset(float width)
    {
        switch (direction)
        {
            case SlideDirection.FromLeftToRight:
                return new Vector2(-width, 0f);

            case SlideDirection.FromRightToLeft:
            default:
                return new Vector2(width, 0f);
        }
    }
}