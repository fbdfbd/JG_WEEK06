using System;
using System.Collections;
using UnityEngine;

public enum SlideDirection
{
    FromRightToLeft,
    FromLeftToRight,
}

public class UIBackgroundSlideTransition : UIBackgroundTransitionBase
{
    [SerializeField] private RectTransform viewport;
    [SerializeField, Min(0f)] private float duration = 0.5f;
    [SerializeField] private SlideDirection direction = SlideDirection.FromRightToLeft;
    [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine routine;

    public override bool IsPlaying => routine != null;

    public override void Play(
        RectTransform current,
        Vector2 currentBasePosition,
        RectTransform next,
        Vector2 nextBasePosition,
        Action onFinished)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        routine = StartCoroutine(PlayRoutine(current, currentBasePosition, next, nextBasePosition, onFinished));
    }

    private IEnumerator PlayRoutine(
        RectTransform current,
        Vector2 currentBasePosition,
        RectTransform next,
        Vector2 nextBasePosition,
        Action onFinished)
    {
        if (current == null || next == null)
        {
            onFinished?.Invoke();
            routine = null;
            yield break;
        }

        float width = GetWidth(next);
        Vector2 offset = direction == SlideDirection.FromRightToLeft
            ? new Vector2(width, 0f)
            : new Vector2(-width, 0f);

        current.gameObject.SetActive(true);
        next.gameObject.SetActive(true);

        current.localScale = Vector3.one;
        next.localScale = Vector3.one;

        next.SetAsLastSibling();

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

        current.anchoredPosition = currentBasePosition;
        next.anchoredPosition = nextBasePosition;

        current.gameObject.SetActive(false);
        next.gameObject.SetActive(true);
        next.SetAsLastSibling();

        routine = null;
        onFinished?.Invoke();
    }

    private float GetWidth(RectTransform target)
    {
        if (viewport != null)
            return viewport.rect.width;

        RectTransform parent = target.parent as RectTransform;
        if (parent != null)
            return parent.rect.width;

        return Screen.width;
    }
}