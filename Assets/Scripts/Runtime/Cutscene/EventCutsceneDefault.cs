using System.Collections;
using UnityEngine;
using DG.Tweening;

public enum CharacterPos
{
    Both,
    Center,
    None,
}

public class EventCutsceneDefault : WeekFlowCutscenePlayerBase
{
    private enum PanelFadeMode
    {
        FadeIn,
        FadeOut,
        FadeInOut
    }

    [Header("기본 설정")]
    [SerializeField] private string _targetEventId = string.Empty;
    [SerializeField] private BackgroundType _backgroundType;

    [Header("연출 설정")]
    [SerializeField] private bool _playSpecialOnStart = true;
    [SerializeField] private float _duration = 1f;

    [Header("캐릭터 배치")]
    [SerializeField] private CharacterPos _characterPos = CharacterPos.Both;
    [SerializeField] private CutsceneCharacterType _leftCharacter;
    [SerializeField] private CutsceneCharacterType _rightCharacter;
    [SerializeField] private CutsceneCharacterType _centerCharacter;

    [Header("패널 페이드")]
    [SerializeField] private bool _usePanelFade = false;
    [SerializeField] private CanvasGroup _fadePanel;
    [SerializeField] private PanelFadeMode _panelFadeMode = PanelFadeMode.FadeIn;
    [SerializeField] private float _panelFadeDuration = 0.3f;

    private bool _isPlaying;
    private bool _hasStarted;

    public override bool PersistsForEvent => true;
    public override bool IsPlaying => _isPlaying;

    public override bool CanPlay(WeekFlowCutsceneRequest request)
    {
        return !string.IsNullOrWhiteSpace(_targetEventId)
            && string.Equals(request.EventId, _targetEventId, System.StringComparison.OrdinalIgnoreCase);
    }

    public override IEnumerator Play(WeekFlowCutsceneRequest request)
    {
        _isPlaying = true;

        if (request.Moment == EWeekFlowCutsceneMoment.EventExit)
        {
            HandleExit();
        }
        else if (!_hasStarted)
        {
            yield return PlayStartSequence();
        }

        _isPlaying = false;
    }

    public override bool TrySkip()
    {
        return false;
    }

    public override void StopImmediate()
    {
        _isPlaying = false;
        _hasStarted = false;

        KillFadeTween();
        HideAllCharacters(false);
        SetBackground(false);
    }

    private IEnumerator PlayStartSequence()
    {
        ShowCharacters();
        SetBackground(true);

        if (_usePanelFade)
        {
            yield return PlayPanelFade();
        }

        if (_playSpecialOnStart)
        {
            yield return PlaySpecial();
        }

        _hasStarted = true;
    }

    private void HandleExit()
    {
        KillFadeTween();
        HideAllCharacters(false);
        SetBackground(false);
        _hasStarted = false;
    }

    private void ShowCharacters()
    {
        if (CutsceneCharacterManager.I == null)
            return;

        HideAllCharacters(true);

        switch (_characterPos)
        {
            case CharacterPos.Both:
                CutsceneCharacterManager.I.ShowLeft(_leftCharacter);
                CutsceneCharacterManager.I.ShowRight(_rightCharacter);
                break;

            case CharacterPos.Center:
                CutsceneCharacterManager.I.ShowCenter(_centerCharacter);
                break;

            case CharacterPos.None:
                break;
        }
    }

    private void HideAllCharacters(bool immediate)
    {
        if (CutsceneCharacterManager.I == null)
            return;

        if (immediate)
        {
            CutsceneCharacterManager.I.HideAll();
            return;
        }

        CutsceneCharacterManager.I.HideAllDeferred();
    }

    private void SetBackground(bool visible)
    {
        if (BackgroundManager.I == null)
            return;

        if (visible)
            BackgroundManager.I.ShowBackground(_backgroundType);
        else
            BackgroundManager.I.HideBackground(_backgroundType);
    }

    private IEnumerator PlaySpecial()
    {
        yield return new WaitForSeconds(_duration);
    }

    private IEnumerator PlayPanelFade()
    {
        if (_fadePanel == null)
            yield break;

        KillFadeTween();
        _fadePanel.gameObject.SetActive(true);

        switch (_panelFadeMode)
        {
            case PanelFadeMode.FadeIn:
                _fadePanel.alpha = 0f;
                yield return _fadePanel
                    .DOFade(1f, _panelFadeDuration)
                    .WaitForCompletion();
                break;

            case PanelFadeMode.FadeOut:
                _fadePanel.alpha = 1f;
                yield return _fadePanel
                    .DOFade(0f, _panelFadeDuration)
                    .WaitForCompletion();
                break;

            case PanelFadeMode.FadeInOut:
                _fadePanel.alpha = 0f;
                yield return _fadePanel
                    .DOFade(1f, _panelFadeDuration)
                    .WaitForCompletion();

                yield return _fadePanel
                    .DOFade(0f, _panelFadeDuration)
                    .WaitForCompletion();
                break;
        }
    }

    private void KillFadeTween()
    {
        if (_fadePanel == null)
            return;

        _fadePanel.DOKill();
    }
}
