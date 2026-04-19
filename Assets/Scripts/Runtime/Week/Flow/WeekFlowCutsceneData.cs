public enum EWeekFlowCutsceneMoment
{
    EventEnter,
    EventExit,
    ScreenEnter,
    LineEnter
}

public readonly struct WeekFlowCutsceneRequest
{
    public WeekFlowCutsceneRequest(
        EWeekFlowCutsceneMoment moment,
        EWeekFlowScreenType screenType,
        string weekId,
        int weekIndex,
        string eventId,
        string stepName,
        int dialogueIndex,
        string choiceLabel,
        RuntimeChildState childState,
        RuntimeWeekResult lastWeekResult)
    {
        Moment = moment;
        ScreenType = screenType;
        WeekId = weekId;
        WeekIndex = weekIndex;
        EventId = eventId;
        StepName = stepName;
        DialogueIndex = dialogueIndex;
        ChoiceLabel = choiceLabel;
        ChildState = childState;
        LastWeekResult = lastWeekResult;
    }

    public EWeekFlowCutsceneMoment Moment { get; }
    public EWeekFlowScreenType ScreenType { get; }
    public string WeekId { get; }
    public int WeekIndex { get; }
    public string EventId { get; }
    public string StepName { get; }
    public int DialogueIndex { get; }
    public string ChoiceLabel { get; }
    public RuntimeChildState ChildState { get; }
    public RuntimeWeekResult LastWeekResult { get; }

    public static WeekFlowCutsceneRequest CreateEventEnter(
        WeekFlowScreen screen,
        RuntimeChildState childState,
        RuntimeWeekResult lastWeekResult)
    {
        return CreateEventRequest(EWeekFlowCutsceneMoment.EventEnter, screen, childState, lastWeekResult);
    }

    public static WeekFlowCutsceneRequest CreateEventExit(
        WeekFlowScreen screen,
        RuntimeChildState childState,
        RuntimeWeekResult lastWeekResult)
    {
        return CreateEventRequest(EWeekFlowCutsceneMoment.EventExit, screen, childState, lastWeekResult);
    }

    public static WeekFlowCutsceneRequest CreateScreenEnter(
        WeekFlowScreen screen,
        RuntimeChildState childState,
        RuntimeWeekResult lastWeekResult)
    {
        if (screen == null)
        {
            return default;
        }

        return new WeekFlowCutsceneRequest(
            EWeekFlowCutsceneMoment.ScreenEnter,
            screen.ScreenType,
            ResolveWeekId(screen.WeekDefinition),
            screen.WeekDefinition != null ? screen.WeekDefinition.WeekIndex : 0,
            ResolveEventId(screen.EventDefinition),
            ResolveStepName(screen.StepDefinition),
            -1,
            ResolveChoiceLabel(screen.ChoiceData),
            childState,
            lastWeekResult);
    }

    public static WeekFlowCutsceneRequest CreateLineEnter(
        WeekFlowScreen screen,
        int dialogueIndex,
        RuntimeChildState childState,
        RuntimeWeekResult lastWeekResult)
    {
        if (screen == null)
        {
            return default;
        }

        return new WeekFlowCutsceneRequest(
            EWeekFlowCutsceneMoment.LineEnter,
            screen.ScreenType,
            ResolveWeekId(screen.WeekDefinition),
            screen.WeekDefinition != null ? screen.WeekDefinition.WeekIndex : 0,
            ResolveEventId(screen.EventDefinition),
            ResolveStepName(screen.StepDefinition),
            dialogueIndex,
            ResolveChoiceLabel(screen.ChoiceData),
            childState,
            lastWeekResult);
    }

    private static WeekFlowCutsceneRequest CreateEventRequest(
        EWeekFlowCutsceneMoment moment,
        WeekFlowScreen screen,
        RuntimeChildState childState,
        RuntimeWeekResult lastWeekResult)
    {
        if (screen == null)
        {
            return default;
        }

        return new WeekFlowCutsceneRequest(
            moment,
            screen.ScreenType,
            ResolveWeekId(screen.WeekDefinition),
            screen.WeekDefinition != null ? screen.WeekDefinition.WeekIndex : 0,
            ResolveEventId(screen.EventDefinition),
            ResolveStepName(screen.StepDefinition),
            -1,
            ResolveChoiceLabel(screen.ChoiceData),
            childState,
            lastWeekResult);
    }

    private static string ResolveWeekId(SO_WeekDefinition weekDefinition)
    {
        if (weekDefinition == null)
        {
            return string.Empty;
        }

        return !string.IsNullOrWhiteSpace(weekDefinition.Id)
            ? weekDefinition.Id
            : weekDefinition.name;
    }

    private static string ResolveEventId(SO_InteractiveEventDefinition eventDefinition)
    {
        if (eventDefinition == null)
        {
            return string.Empty;
        }

        return !string.IsNullOrWhiteSpace(eventDefinition.Id)
            ? eventDefinition.Id
            : eventDefinition.name;
    }

    private static string ResolveStepName(SO_InteractiveEventStepDefinition stepDefinition)
    {
        return stepDefinition != null ? stepDefinition.name : string.Empty;
    }

    private static string ResolveChoiceLabel(InteractiveEventChoiceData choiceData)
    {
        return choiceData != null ? choiceData.Label : string.Empty;
    }
}
