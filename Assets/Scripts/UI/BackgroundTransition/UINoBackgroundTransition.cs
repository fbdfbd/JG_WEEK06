using System;
using UnityEngine;

public class UINoBackgroundTransition : UIBackgroundTransitionBase
{
    public override bool IsPlaying => false;

    public override void Play(
        RectTransform current,
        Vector2 currentBasePosition,
        RectTransform next,
        Vector2 nextBasePosition,
        Action onFinished)
    {
        if (current != null)
        {
            current.anchoredPosition = currentBasePosition;
            current.localScale = Vector3.one;
            current.gameObject.SetActive(false);
        }

        if (next != null)
        {
            next.gameObject.SetActive(true);
            next.anchoredPosition = nextBasePosition;
            next.localScale = Vector3.one;
            next.SetAsLastSibling();
        }

        onFinished?.Invoke();
    }
}