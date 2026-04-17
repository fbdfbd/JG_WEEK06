using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_BottomView : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _infoButton;
    [SerializeField] private Button _nextDayButton;

    public event Action OnInfoButtonClicked;
    public event Action OnNextDayButtonClicked;

    private void Awake()
    {
        // 버튼 클릭 시 Action을 발생(Invoke)시킵니다.
        if (_infoButton != null)
        {
            _infoButton.onClick.AddListener(() => OnInfoButtonClicked?.Invoke());
        }

        if (_nextDayButton != null)
        {
            _nextDayButton.onClick.AddListener(() => OnNextDayButtonClicked?.Invoke());
        }
    }

    private void OnDestroy()
    {
        if (_infoButton != null)
        {
            _infoButton.onClick.RemoveAllListeners();
        }

        if (_nextDayButton != null)
        {
            _nextDayButton.onClick.RemoveAllListeners();
        }
    }
}
