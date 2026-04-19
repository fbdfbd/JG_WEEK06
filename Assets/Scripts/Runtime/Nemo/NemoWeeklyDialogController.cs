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

        _talkText.DOKill();
        _talkText.text = string.Empty;

        if (_currentTalk == null || string.IsNullOrWhiteSpace(_currentTalk.Context))
        {
            return;
        }

        _talkText.DOText(_currentTalk.Context, textDuration).SetEase(Ease.Linear);
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

    public void OnClickTalkButton()
    {
        if (_weeklyDialogFinised)
        {
            _dialogPanel.Show();
            PlayCurrentTalkParticle();
            TextRefresh();
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
