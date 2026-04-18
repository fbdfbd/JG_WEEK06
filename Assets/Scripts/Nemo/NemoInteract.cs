using UnityEngine;
using UnityEngine.EventSystems;

public sealed class NemoInteract
{
    private readonly GameObject _interactPanel;
    private readonly RectTransform _canvasRect;
    private readonly Vector3 _offset;

    public NemoInteract(
        GameObject interactPanel, 
        RectTransform canvasRect,
        Vector3 offset
        )
    {
        _interactPanel = interactPanel;
        _canvasRect = canvasRect;
        _offset = offset;
    }

    public void OpenPanel(Transform nemo)
    {
        if (_interactPanel != null)
        {
            ChangeUIPosition(nemo);
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

    public void ChangeUIPosition(Transform nemoTf)
    {
        Vector3 worldPosition = nemoTf.position + _offset;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            screenPos,
            Camera.main,
            out localPointerPosition
        );

        RectTransform panelRect = _interactPanel.transform as RectTransform;

        panelRect.localPosition = localPointerPosition;
    }
}
