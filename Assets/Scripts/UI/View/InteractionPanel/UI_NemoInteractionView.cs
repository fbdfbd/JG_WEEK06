using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_NemoInteractionView : MonoBehaviour
{
    [SerializeField] private Button _nemoButton;

    public event Action OnNemoClicked;

    private void Awake()
    {
        if (_nemoButton != null)
        {
            _nemoButton.onClick.AddListener(() => OnNemoClicked?.Invoke());
        }
    }

    private void OnDestroy()
    {
        if (_nemoButton != null)
        {
            _nemoButton.onClick.RemoveAllListeners();
        }
    }
}