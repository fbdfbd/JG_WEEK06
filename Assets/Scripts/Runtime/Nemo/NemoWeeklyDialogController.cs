using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;

public class NemoWeeklyDialogController : MonoBehaviour
{
    [SerializeField] private SO_WeeklyTalkCatalog _weeklyTalkCatalog;
    [SerializeField] private WeekFlowController _weekFlowController;

    [SerializeField] private UI_WeeklyDialogPanel _dialogPanel;
    [SerializeField] private GameObject _interactionPanel;
    [SerializeField] private TextMeshProUGUI _talkText; 
    [SerializeField] private float textDuration = 0.5f;

    [SerializeField] private List<ParticleSystem> _particle;

    private bool _weeklyDialogFinised = true;
    private string _previousWeekId = string.Empty;
    private string _weekId = string.Empty;
    private SO_WeeklyTalk _currentTalk;
    private Tween _typingTween;

    private void OnEnable()
    {
        if (_weekFlowController != null)
        {
            _weekFlowController.WeekChanged += HandleWeekChanged;
        }
    }

    private void OnDisable()
    {
        if (_weekFlowController != null)
        {
            _weekFlowController.WeekChanged -= HandleWeekChanged;
        }
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        StopTypingTween();
    }

    private void Refresh()
    {
        string nextWeekId = _weekFlowController != null
            ? _weekFlowController.CurrentWeekDefinition?.Id ?? string.Empty
            : string.Empty;

        bool hasWeekChanged = !string.Equals(_previousWeekId, nextWeekId, StringComparison.Ordinal);

        _weekId = nextWeekId;

        if (hasWeekChanged)
        {
            _weeklyDialogFinised = true;
        }

        _currentTalk = _weeklyTalkCatalog != null
            ? _weeklyTalkCatalog.GetByWeekId(_weekId)
            : null;

        _previousWeekId = _weekId;
    }

    private void TextRefresh()
    {
        if (_talkText == null)
        {
            return;
        }

        StopTypingTween();

        if (_currentTalk == null || string.IsNullOrWhiteSpace(_currentTalk.Context))
        {
            _talkText.text = string.Empty;
            return;
        }

        _talkText.text = _currentTalk.Context;
        _talkText.maxVisibleCharacters = 0;
        _talkText.ForceMeshUpdate();

        int characterCount = _talkText.textInfo.characterCount;
        if (characterCount <= 0)
        {
            _talkText.maxVisibleCharacters = int.MaxValue;
            return;
        }

        _typingTween = DOTween.To(
                GetVisibleCharacterCount,
                ApplyVisibleCharacterCount,
                characterCount,
                textDuration)
            .SetEase(Ease.Linear)
            .OnComplete(HandleTypingCompleted);
    }

    private void HandleWeekChanged(SO_WeekDefinition _)
    {
        Refresh();
    }

    private void PlayCurrentTalkParticle()
    {
        if (_currentTalk == null || _particle == null)
        {
            return;
        }

        int particleIndex = (int)_currentTalk.NemoState;
        if (particleIndex < 0 || particleIndex >= _particle.Count)
        {
            return;
        }

        ParticleSystem targetParticle = _particle[particleIndex];
        if (targetParticle == null)
        {
            return;
        }

        targetParticle.Play();
    }

    private void ApplyCurrentTalkStat()
    {
        if (_currentTalk?.StatDelta == null)
        {
            return;
        }

        RuntimeChildState childState = _weekFlowController != null ? _weekFlowController.CurrentChildState : null;
        if (childState == null)
        {
            return;
        }

        _currentTalk.StatDelta.Apply(childState);
    }

    private int GetVisibleCharacterCount()
    {
        return _talkText != null ? _talkText.maxVisibleCharacters : 0;
    }

    private void ApplyVisibleCharacterCount(int visibleCharacterCount)
    {
        if (_talkText == null)
        {
            return;
        }

        _talkText.maxVisibleCharacters = visibleCharacterCount;
    }

    private void HandleTypingCompleted()
    {
        ApplyVisibleCharacterCount(int.MaxValue);
        _typingTween = null;
    }

    private void StopTypingTween()
    {
        if (_typingTween == null)
        {
            return;
        }

        if (_typingTween.IsActive())
        {
            _typingTween.Kill();
        }

        _typingTween = null;
    }

    public void OnClickTalkButton()
    {
        if (_weeklyDialogFinised)
        {
            _dialogPanel.Show();
            PlayCurrentTalkParticle();
            TextRefresh();
            ApplyCurrentTalkStat();
            _weeklyDialogFinised = false;
        }

        _interactionPanel.SetActive(false);
    }

    public void OnClickeExitButton()
    {
        _dialogPanel.Hide();
    }
    public void OnClickFlagButton()
    {

    }
}
