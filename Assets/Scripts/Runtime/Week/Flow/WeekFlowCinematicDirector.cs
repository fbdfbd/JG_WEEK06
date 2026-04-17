using System.Collections;

public sealed class WeekFlowCinematicDirector
{
    private readonly WeekFlowViewBase _view;
    private readonly WeekFlowCinematicResolver _resolver;

    public WeekFlowCinematicDirector(WeekFlowViewBase view, WeekFlowCinematicResolver resolver)
    {
        _view = view;
        _resolver = resolver;
    }

    public IEnumerator PlayScreenEnter(WeekFlowScreen screen)
    {
        yield return PlayTransition(new WeekFlowTransitionContext(EWeekFlowTransitionPhase.Enter, _resolver.ResolveEnterCue(screen), screen));
    }

    public IEnumerator PlayScreenExit(WeekFlowScreen screen)
    {
        yield return PlayTransition(new WeekFlowTransitionContext(EWeekFlowTransitionPhase.Exit, _resolver.ResolveExitCue(screen), screen));
    }

    public IEnumerator PlayWeekChangeOut(SO_WeekDefinition weekDefinition)
    {
        yield return PlayTransition(new WeekFlowTransitionContext(EWeekFlowTransitionPhase.Exit, _resolver.ResolveWeekChangeOutCue(weekDefinition), null, weekDefinition));
    }

    public IEnumerator PlayWeekChangeIn(SO_WeekDefinition weekDefinition)
    {
        yield return PlayTransition(new WeekFlowTransitionContext(EWeekFlowTransitionPhase.Enter, _resolver.ResolveWeekChangeInCue(weekDefinition), null, weekDefinition));
    }

    private IEnumerator PlayTransition(WeekFlowTransitionContext context)
    {
        if (_view == null || context.Cue == null)
        {
            yield break;
        }

        yield return _view.PlayFlowTransition(context);
    }
}
