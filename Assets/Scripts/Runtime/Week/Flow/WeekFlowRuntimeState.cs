using System;
using System.Collections.Generic;

public sealed class WeekFlowRuntimeState
{
    private readonly List<SO_InteractiveEventDefinition> _pendingEvents = new();
    private int _nextEventIndex;
    public event Action<RuntimeChildState> ChildStateReplaced;

    public WeekFlowRuntimeState()
    {
        SetChildState(new RuntimeChildState(), false);
        ClearPendingEventState();
        StatusMessage = "Week flow is ready.";
    }

    public RuntimeChildState ChildState { get; private set; }
    public RuntimeWeekResult LastWeekResult { get; set; }
    public RuntimeInteractiveEventSession CurrentEventSession { get; private set; }
    public bool ShouldShowEndingAfterEvents { get; set; }
    public bool ShouldAdvanceToNextWeekAfterEvents { get; set; }
    public bool HasReachedEnding { get; set; }
    public string StatusMessage { get; private set; }

    public void ResetChildState()
    {
        SetChildState(new RuntimeChildState(), true);
        LastWeekResult = null;
        HasReachedEnding = false;
        ClearPendingEventState();
    }

    public void SetPendingEvents(IReadOnlyList<SO_InteractiveEventDefinition> pendingEvents)
    {
        _pendingEvents.Clear();
        _pendingEvents.AddRange(pendingEvents ?? Array.Empty<SO_InteractiveEventDefinition>());
        _nextEventIndex = 0;
        CurrentEventSession = null;
    }

    public bool TryStartNextEvent()
    {
        while (_nextEventIndex < _pendingEvents.Count)
        {
            SO_InteractiveEventDefinition nextEvent = _pendingEvents[_nextEventIndex++];
            if (nextEvent?.FirstStep == null)
            {
                continue;
            }

            CurrentEventSession = new RuntimeInteractiveEventSession(nextEvent);
            return true;
        }

        CurrentEventSession = null;
        return false;
    }

    public void ClearCurrentEventSession()
    {
        CurrentEventSession = null;
    }

    public void ClearPendingEventState()
    {
        _pendingEvents.Clear();
        _nextEventIndex = 0;
        CurrentEventSession = null;
        ShouldShowEndingAfterEvents = false;
        ShouldAdvanceToNextWeekAfterEvents = false;
    }

    public void SetStatusMessage(string statusMessage)
    {
        StatusMessage = statusMessage;
    }

    private void SetChildState(RuntimeChildState childState, bool notify)
    {
        ChildState = childState;
        if (notify)
        {
            ChildStateReplaced?.Invoke(ChildState);
        }
    }
}

public sealed class RuntimeInteractiveEventSession
{
    public RuntimeInteractiveEventSession(SO_InteractiveEventDefinition eventDefinition)
    {
        EventDefinition = eventDefinition;
        CurrentStep = eventDefinition != null ? eventDefinition.FirstStep : null;
    }

    public SO_InteractiveEventDefinition EventDefinition { get; }
    public SO_InteractiveEventStepDefinition CurrentStep { get; private set; }
    public InteractiveEventChoiceData SelectedChoice { get; private set; }
    public bool HasAppliedCurrentStepEffects { get; private set; }
    public bool HasAppliedCurrentStep => HasAppliedCurrentStepEffects;
    public bool HasPendingChoiceResult => SelectedChoice != null;

    public void MarkCurrentStepEffectsApplied()
    {
        HasAppliedCurrentStepEffects = true;
    }

    public void MarkCurrentStepApplied()
    {
        MarkCurrentStepEffectsApplied();
    }

    public void SelectChoice(InteractiveEventChoiceData choice)
    {
        SelectedChoice = choice;
    }

    public void ClearChoiceResult()
    {
        SelectedChoice = null;
    }

    public bool TryMoveToNextStep()
    {
        SO_InteractiveEventStepDefinition nextStep = ResolveNextStep();
        if (nextStep == null)
        {
            return false;
        }

        CurrentStep = nextStep;
        SelectedChoice = null;
        HasAppliedCurrentStepEffects = false;
        return true;
    }

    private SO_InteractiveEventStepDefinition ResolveNextStep()
    {
        if (SelectedChoice?.NextStep != null)
        {
            return SelectedChoice.NextStep;
        }

        return CurrentStep != null ? CurrentStep.NextStep : null;
    }
}
