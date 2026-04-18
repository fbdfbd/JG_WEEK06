using UnityEngine;

public class UI_InteractionScreenView : WeekFlowViewBase
{
    [Header("Interaction Components")]
    [SerializeField] private UI_NemoInteractionView _nemoInteractionView;
    [SerializeField] private GameObject _interactionPanel; // Canvas_Interaction 할당

    private void Awake()
    {
        if (_interactionPanel != null)
        {
            _interactionPanel.SetActive(false);
        }

        if (_nemoInteractionView != null)
        {
            _nemoInteractionView.OnNemoClicked += HandleNemoClicked;
        }
    }

    private void HandleNemoClicked()
    {
        if (_interactionPanel != null)
        {
            _interactionPanel.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (_nemoInteractionView != null)
        {
            _nemoInteractionView.OnNemoClicked -= HandleNemoClicked;
        }
    }

}