using System.Collections;
using UnityEngine;

public abstract class WeekFlowCutsceneBridgeBase : MonoBehaviour
{
    public abstract bool IsPlaying { get; }
    public abstract bool IsBlocking { get; }

    public abstract IEnumerator Play(WeekFlowCutsceneRequest request);
    public abstract bool TrySkip();
    public abstract void StopImmediate();
}
