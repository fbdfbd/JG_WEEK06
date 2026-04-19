using System;
using DG.Tweening;
using UnityEngine;

public class UIBackgroundPopTransition : UIBackgroundTransitionBase
{
    [SerializeField, Min(0f)] private float duration = 0.35f;
    [SerializeField] private float startScale = 0.88f;
    [SerializeField] private Ease ease = Ease.OutBack;

    private Sequence playingSequence;

    public override bool IsPlaying => playingSequence != null && playingSequence.IsActive();

    public override void Play(
        RectTransform current,
        Vector2 currentBasePosition,
        RectTransform next,
        Vector2 nextBasePosition,
        Action onFinished)
    {
        KillCurrentSequence();

        if (next == null)
        {
            onFinished?.Invoke();
            return;
        }

        // 이전 배경 정리
        if (current != null)
        {
            current.DOKill();
            current.anchoredPosition = currentBasePosition;
            current.localScale = Vector3.one;
            current.gameObject.SetActive(false);
        }

        // 다음 배경 준비
        next.DOKill();
        next.gameObject.SetActive(true);
        next.SetAsLastSibling();
        next.anchoredPosition = nextBasePosition;
        next.localScale = new Vector3(startScale, startScale, 1f);

        // 뿅
        playingSequence = DOTween.Sequence();
        playingSequence.Append(
            next.DOScale(Vector3.one, duration).SetEase(ease)
        );
        playingSequence.OnComplete(() =>
        {
            next.anchoredPosition = nextBasePosition;
            next.localScale = Vector3.one;
            next.SetAsLastSibling();
            playingSequence = null;
            onFinished?.Invoke();
        });
        playingSequence.OnKill(() =>
        {
            playingSequence = null;
        });
    }

    private void OnDisable()
    {
        KillCurrentSequence();
    }

    private void KillCurrentSequence()
    {
        if (playingSequence != null && playingSequence.IsActive())
        {
            playingSequence.Kill();
            playingSequence = null;
        }
    }
}