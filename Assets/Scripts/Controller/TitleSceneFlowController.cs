using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneFlowController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private string _inGameSceneName = "InGame";

    [Header("UI Targets")]
    [SerializeField] private RectTransform _titleRect;
    [SerializeField] private RectTransform _buttonPanelRect;

    [Header("World Target")]
    [SerializeField] private Transform _nemoTransform;

    [Header("Animation")]
    [SerializeField] private float _duration = 0.8f;
    [SerializeField] private float _uiExitDistance = 1200f;
    [SerializeField] private float _nemoExitDistance = 15f;
    [SerializeField] private float _buttonPanelStartX = -1400f;

    private Vector2 _titleStartPos;
    private Vector2 _buttonPanelStartPos;
    private Vector3 _nemoStartPos;

    private bool _introPlayed = false;
    private bool _isAnimating = false;

    private void Start()
    {
        _startButton.onClick.AddListener(OnClickStartButton);
        _exitButton.onClick.AddListener(OnClickExitButton);

        _titleStartPos = _titleRect.anchoredPosition;
        _buttonPanelStartPos = _buttonPanelRect.anchoredPosition;
        _nemoStartPos = _nemoTransform.position;

        _buttonPanelRect.anchoredPosition = new Vector2(_buttonPanelStartX, _buttonPanelStartPos.y);
        SetButtonsInteractable(false);
    }

    private void Update()
    {
        if (_introPlayed || _isAnimating)
            return;

        if (WasPrimaryPointerPressedThisFrame())
        {
            StartCoroutine(PlayIntroSequence());
        }
    }

    private bool WasPrimaryPointerPressedThisFrame()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        return false;
    }

    private IEnumerator PlayIntroSequence()
    {
        _isAnimating = true;
        _introPlayed = true;

        Vector2 titleFrom = _titleStartPos;
        Vector2 titleTo = _titleStartPos + Vector2.left * _uiExitDistance;

        Vector2 panelFrom = new Vector2(_buttonPanelStartX, _buttonPanelStartPos.y);
        Vector2 panelTo = _buttonPanelStartPos;

        Vector3 nemoFrom = _nemoStartPos;
        Vector3 nemoTo = _nemoStartPos + Vector3.left * _nemoExitDistance;

        float time = 0f;

        while (time < _duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            _titleRect.anchoredPosition = Vector2.Lerp(titleFrom, titleTo, eased);
            _buttonPanelRect.anchoredPosition = Vector2.Lerp(panelFrom, panelTo, eased);
            _nemoTransform.position = Vector3.Lerp(nemoFrom, nemoTo, eased);

            yield return null;
        }

        _titleRect.anchoredPosition = titleTo;
        _buttonPanelRect.anchoredPosition = panelTo;
        _nemoTransform.position = nemoTo;

        SetButtonsInteractable(true);
        _isAnimating = false;
    }

    private void SetButtonsInteractable(bool value)
    {
        _startButton.interactable = value;
        _exitButton.interactable = value;
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(OnClickStartButton);
        _exitButton.onClick.RemoveListener(OnClickExitButton);
    }

    private void OnClickStartButton()
    {
        if (_isAnimating)
            return;

        SceneManager.LoadScene(_inGameSceneName);
    }

    private void OnClickExitButton()
    {
        if (_isAnimating)
            return;

        Application.Quit();
    }
}
