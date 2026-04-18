using UnityEngine;
using UnityEngine.EventSystems;

public sealed class NemoInteract
{
    private readonly GameObject _interactPanel;

    public NemoInteract(GameObject interactPanel)
    {
        _interactPanel = interactPanel;
    }

    public void HandlePointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        Debug.Log("네모 클릭함");
        OpenPanel();
    }

    public void OpenPanel()
    {
        if (_interactPanel != null)
        {
            _interactPanel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (_interactPanel != null)
        {
            _interactPanel.SetActive(false);
        }
    }
}
