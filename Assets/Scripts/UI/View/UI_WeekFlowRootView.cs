using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeekFlowRootView : WeekFlowViewBase
{
    private enum EDialogueContinueRoute
    {
        None,
        WeekFeedback,
        Narrative
    }

    [Header("Panels")]
    [SerializeField] private UI_WeekFlowScreenView _weekScreenView;
    [SerializeField] private UI_DialogueScreenView _dialogueScreenView;
    [SerializeField] private UI_DialogueLogPanel _dialogueLogPanel;
    [SerializeField] private UI_WeekFlowTransitionPlayer _transitionPlayer;
    [SerializeField] private WeekFlowCutsceneBridgeBase _cutsceneBridge;
    [SerializeField] private CanvasGroup _mainCanvasGroup;
    [SerializeField] private GameObject _nemo;

    [Header("Log")]
    [SerializeField] private Button _openLogButton;

    private EDialogueContinueRoute _dialogueContinueRoute;
    private readonly WeekFlowDialogueLogService _dialogueLogService = new();

    private void Awake()
    {
        InitializePanels();
        SetMainCanvasVisible(true);
        BindWeekScreenEvents();
        BindDialogueScreenEvents();
        BindLogEvents();
        HideTransientViews();
    }

    private void OnDestroy()
    {
        UnbindWeekScreenEvents();
        UnbindDialogueScreenEvents();
        UnbindLogEvents();
    }

    public override void RenderWeekHeader(WeekHeaderPresentation presentation)
    {
        if (_weekScreenView == null)
        {
            return;
        }

        _weekScreenView.RenderWeekHeader(presentation);
    }

    public override void RenderSelectionGroups(IReadOnlyList<WeekSelectionCategoryGroupPresentation> groups)
    {
        if (_weekScreenView == null)
        {
            return;
        }

        _weekScreenView.RenderSelectionGroups(groups);
    }

    public override void RenderChildState(ChildStatePresentation presentation)
    {
        if (_weekScreenView == null)
        {
            return;
        }

        _weekScreenView.RenderChildState(presentation);
    }

    public override void ShowWeekFeedback(WeekFeedbackPresentation presentation)
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueContinueRoute = EDialogueContinueRoute.WeekFeedback;
        _dialogueScreenView.ShowWeekFeedback(presentation);
    }

    public override void ShowInteractiveEvent(InteractiveEventPresentation presentation)
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueContinueRoute = EDialogueContinueRoute.Narrative;
        _dialogueScreenView.ShowInteractiveEvent(presentation);
    }

    public override void ShowInteractiveEventResult(InteractiveEventChoiceResultPresentation presentation)
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueContinueRoute = EDialogueContinueRoute.Narrative;
        _dialogueScreenView.ShowInteractiveEventResult(presentation);
    }

    public override void ShowEnding(EndingPresentation presentation)
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueContinueRoute = EDialogueContinueRoute.Narrative;
        _dialogueScreenView.ShowEnding(presentation);
    }

    public override void HideTransientViews()
    {
        _dialogueContinueRoute = EDialogueContinueRoute.None;

        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueScreenView.HideView();
    }

    public override void SetMainCanvasVisible(bool visible)
    {
        if (_mainCanvasGroup == null)
        {
            return;
        }

        if(_nemo == null)
        {
            return;
        }

        _nemo.SetActive(visible);
        _mainCanvasGroup.alpha = visible ? 1f : 0f;
        _mainCanvasGroup.interactable = visible;
        _mainCanvasGroup.blocksRaycasts = visible;
    }

    public override WeekFlowCutsceneBridgeBase GetCutsceneBridge()
    {
        return _cutsceneBridge;
    }

    public override void SetFlowScreenContext(WeekFlowScreen screen, RuntimeChildState childState, RuntimeWeekResult lastWeekResult)
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueScreenView.SetScreenContext(screen, childState, lastWeekResult);
    }

    public override System.Collections.IEnumerator PlayCurrentDialogueCutscene()
    {
        if (_dialogueScreenView == null)
        {
            yield break;
        }

        yield return _dialogueScreenView.PlayCurrentDialogueCutscene();
    }

    public override System.Collections.IEnumerator PlayFlowTransition(WeekFlowTransitionContext context)
    {
        if (_transitionPlayer == null)
        {
            yield break;
        }

        yield return _transitionPlayer.Play(context);
    }

    private void InitializePanels()
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueScreenView.SetDialogueLogService(_dialogueLogService);
        _dialogueScreenView.SetCutsceneBridge(_cutsceneBridge);

        if (_dialogueLogPanel != null)
        {
            _dialogueLogPanel.SetDialogueLogService(_dialogueLogService);
            _dialogueLogPanel.Hide();
        }
    }

    private void BindWeekScreenEvents()
    {
        if (_weekScreenView == null)
        {
            return;
        }

        _weekScreenView.RunWeekRequested += HandleRunWeekRequested;
        _weekScreenView.CardOptionSelected += HandleCardOptionSelected;
    }

    private void UnbindWeekScreenEvents()
    {
        if (_weekScreenView == null)
        {
            return;
        }

        _weekScreenView.RunWeekRequested -= HandleRunWeekRequested;
        _weekScreenView.CardOptionSelected -= HandleCardOptionSelected;
    }

    private void BindDialogueScreenEvents()
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueScreenView.ContinueRequested += HandleDialogueContinueRequested;
        _dialogueScreenView.ChoiceSelected += HandleDialogueChoiceSelected;
    }

    private void BindLogEvents()
    {
        if (_openLogButton == null)
        {
            return;
        }

        _openLogButton.onClick.AddListener(HandleOpenLogButtonClicked);
    }

    private void UnbindDialogueScreenEvents()
    {
        if (_dialogueScreenView == null)
        {
            return;
        }

        _dialogueScreenView.ContinueRequested -= HandleDialogueContinueRequested;
        _dialogueScreenView.ChoiceSelected -= HandleDialogueChoiceSelected;
    }

    private void UnbindLogEvents()
    {
        if (_openLogButton == null)
        {
            return;
        }

        _openLogButton.onClick.RemoveListener(HandleOpenLogButtonClicked);
    }

    private void HandleRunWeekRequested()
    {
        RaiseRunWeekRequested();
    }

    private void HandleOpenLogButtonClicked()
    {
        if (_dialogueLogPanel == null)
        {
            return;
        }

        _dialogueLogPanel.Show();
    }

    private void HandleCardOptionSelected(SO_CardInfoDefinition cardDefinition, int optionIndex)
    {
        RaiseCardOptionSelected(cardDefinition, optionIndex);
    }

    private void HandleDialogueContinueRequested()
    {
        if (TrySkipCurrentTransition())
        {
            return;
        }

        switch (_dialogueContinueRoute)
        {
            case EDialogueContinueRoute.WeekFeedback:
                RaiseWeekFeedbackClosed();
                break;

            case EDialogueContinueRoute.Narrative:
                RaiseInteractiveEventContinueRequested();
                break;
        }
    }

    private void HandleDialogueChoiceSelected(int choiceIndex)
    {
        if (TrySkipCurrentTransition())
        {
            return;
        }

        RaiseInteractiveEventChoiceSelected(choiceIndex);
    }

    private bool TrySkipCurrentTransition()
    {
        if (_transitionPlayer == null)
        {
            return false;
        }

        return _transitionPlayer.TrySkipCurrent();
    }
}

