using System.Collections;
using UnityEngine;

public class EventCutsceneDefault : WeekFlowCutscenePlayerBase
{
    [SerializeField] private string _targetEventId = string.Empty;
    [SerializeField] private BackgroundType _backgroundType;

    [Header("커스텀")]
    [SerializeField] private bool _playSpecialOnStart = true;
    [SerializeField] private float _duration;

    [SerializeField] private CutsceneCharacterType _leftCharacter;
    [SerializeField] private CutsceneCharacterType _rightCharacter;

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
        SetCharacter(true);
    }

    private void DeactivateRoots()
    {
        SetCharacter(false);
    }

    private void SetCharacter(bool active)
    {
        if (CutsceneCharacterManager.I != null)
        {
            if(active)
            {
                CutsceneCharacterManager.I.ShowLeft(_leftCharacter);
                CutsceneCharacterManager.I.ShowRight(_rightCharacter);
                BackgroundSet(active);
            }
            else
            {
                CutsceneCharacterManager.I.HideLeft();
                CutsceneCharacterManager.I.HideRight();
                BackgroundSet(active);
            }
        }
    }

    private void BackgroundSet(bool active)
    {
        if (BackgroundManager.I != null)
        {
            if (active)
                BackgroundManager.I.ShowBackground(_backgroundType);
            else
                BackgroundManager.I.HideBackground(_backgroundType);
        }
    }


    private IEnumerator PlaySpecialIfNeeded()
    {
        yield return new WaitForSeconds(_duration);
    }
}
