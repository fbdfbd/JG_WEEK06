using System.Collections;
using UnityEngine;

public abstract class WeekFlowCutscenePlayerBase : MonoBehaviour
{
    [SerializeField] private string _cutsceneId = string.Empty;
    [SerializeField] private bool _isBlocking = true;

    public string CutsceneId => _cutsceneId;
    public bool IsBlocking => _isBlocking;

    // Event-wide background or stage players return true here so the bridge
    // can keep them alive while step-specific overrides play on top.
    public virtual bool PersistsForEvent => false;

    public abstract bool IsPlaying { get; }

    public virtual bool CanPlay(WeekFlowCutsceneRequest request)
    {
        return true;
    }

    public abstract IEnumerator Play(WeekFlowCutsceneRequest request);
    public abstract bool TrySkip();
    public abstract void StopImmediate();
}
