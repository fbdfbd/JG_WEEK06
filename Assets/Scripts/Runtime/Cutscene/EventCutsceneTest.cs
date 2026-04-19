using System.Collections;
using UnityEngine;

public class EventCutsceneTest : WeekFlowCutscenePlayerBase
{
    [SerializeField] private string _targetEventId = string.Empty;
    [SerializeField] private BackgroundType _backgroundType;

    [Header("Roots")]
    [SerializeField] private GameObject _contentRoot;

    [Header("Optional Special")]
    [SerializeField] private bool _playSpecialOnStart = true;

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

        switch (request.Moment)
        {
            case EWeekFlowCutsceneMoment.EventExit:
                DeactivateRoots();
                _hasStarted = false;
                break;

            default:
                if (!_hasStarted)
                {
                    ActivateRoots();

                    if (_playSpecialOnStart)
                    {
                        yield return PlaySpecialIfNeeded();
                    }

                    _hasStarted = true;
                }
                break;
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
        DeactivateRoots();
    }

    private void ActivateRoots()
    {
        if (BackgroundManager.I != null)
        {
            BackgroundManager.I.ShowBackground(_backgroundType);
        }

        if (_contentRoot != null)
        {
            _contentRoot.SetActive(true);
        }
    }

    private void DeactivateRoots()
    {
        if (_contentRoot != null)
        {
            _contentRoot.SetActive(false);
        }

        if (BackgroundManager.I != null)
        {
            BackgroundManager.I.HideBackground(_backgroundType);
        }
    }

    private IEnumerator PlaySpecialIfNeeded()
    {
        yield return null;
    }
}
