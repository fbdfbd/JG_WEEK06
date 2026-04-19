using System;
using UnityEngine;

public abstract class UIBackgroundTransitionBase : MonoBehaviour
{
    public abstract bool IsPlaying { get; }

    public abstract void Play(
        RectTransform current,
        Vector2 currentBasePosition,
        RectTransform next,
        Vector2 nextBasePosition,
        Action onFinished);
}