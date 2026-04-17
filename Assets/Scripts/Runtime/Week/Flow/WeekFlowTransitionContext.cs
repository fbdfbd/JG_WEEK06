public enum EWeekFlowTransitionPhase
{
    Enter,
    Exit
}

public sealed class WeekFlowTransitionContext
{
    public WeekFlowTransitionContext(
        EWeekFlowTransitionPhase phase,
        SO_WeekFlowCueDefinition cue,
        WeekFlowScreen screen = null,
        SO_WeekDefinition weekDefinition = null)
    {
        Phase = phase;
        Cue = cue;
        Screen = screen;
        WeekDefinition = weekDefinition;
    }

    public EWeekFlowTransitionPhase Phase { get; }
    public SO_WeekFlowCueDefinition Cue { get; }
    public WeekFlowScreen Screen { get; }
    public SO_WeekDefinition WeekDefinition { get; }
    public bool IsWeekChange => WeekDefinition != null && Screen == null;
}
