using System.Collections.Generic;
using UnityEngine;

public class UI_ChildStateToastManager : MonoBehaviour
{
    [SerializeField] private WeekFlowController _weekFlowController;
    [SerializeField] private SO_WeekUiTextCatalog _weekUiTextCatalog;
    [SerializeField] private UI_ChildStateToastItem[] _toastItems;

    private RuntimeChildState _childState;
    private int _nextToastIndex;
    private readonly Queue<string> _queuedToastMessages = new();
    private WeekUiTextProvider _weekUiText;

    private void Awake()
    {
        if (_weekFlowController == null)
        {
            _weekFlowController = FindAnyObjectByType<WeekFlowController>();
        }

        _weekUiText = new WeekUiTextProvider(_weekUiTextCatalog);
    }

    private void OnEnable()
    {
        if (_weekFlowController == null)
        {
            return;
        }

        _weekFlowController.ChildStateSourceChanged += HandleChildStateSourceChanged;
        _weekFlowController.FlowPresentationCompleted += HandleFlowPresentationCompleted;
        BindChildState(_weekFlowController.CurrentChildState);
    }

    private void OnDisable()
    {
        if (_weekFlowController != null)
        {
            _weekFlowController.ChildStateSourceChanged -= HandleChildStateSourceChanged;
            _weekFlowController.FlowPresentationCompleted -= HandleFlowPresentationCompleted;
        }

        BindChildState(null);
        _queuedToastMessages.Clear();
    }

    private void HandleChildStateSourceChanged(RuntimeChildState childState)
    {
        BindChildState(childState);
    }

    private void BindChildState(RuntimeChildState childState)
    {
        if (ReferenceEquals(_childState, childState))
        {
            return;
        }

        if (_childState != null)
        {
            _childState.StatChanged -= HandleStatChanged;
        }

        _childState = childState;

        if (_childState != null)
        {
            _childState.StatChanged += HandleStatChanged;
        }
    }

    private void HandleStatChanged(StatChangeInfo changeInfo)
    {
        if (changeInfo.Delta == 0 || _toastItems == null || _toastItems.Length == 0)
        {
            return;
        }

        _queuedToastMessages.Enqueue(BuildStatMessage(changeInfo));
    }

    private void HandleFlowPresentationCompleted()
    {
        while (_queuedToastMessages.Count > 0)
        {
            PlayToast(_queuedToastMessages.Dequeue());
        }
    }

    private void PlayToast(string message)
    {
        if (string.IsNullOrWhiteSpace(message) || _toastItems == null || _toastItems.Length == 0)
        {
            return;
        }

        UI_ChildStateToastItem toastItem = _toastItems[_nextToastIndex % _toastItems.Length];
        _nextToastIndex++;

        if (toastItem == null)
        {
            return;
        }

        toastItem.Play(message);
    }

    private string BuildStatMessage(StatChangeInfo changeInfo)
    {
        string label = _weekUiText != null
            ? _weekUiText.GetStatLabel(changeInfo.StatType)
            : changeInfo.StatType.ToString();
        string sign = changeInfo.Delta > 0 ? "+" : string.Empty;
        return $"{label} {sign}{changeInfo.Delta}";
    }
}
