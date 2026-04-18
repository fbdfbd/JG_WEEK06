using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneFlowController : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private string _inGameSceneName = "InGame";

    private void Start()
    {
        _startButton.onClick.AddListener(OnClickStartButton);
        _exitButton.onClick.AddListener(OnClickExitButton);
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(OnClickStartButton);
        _exitButton.onClick.RemoveListener(OnClickExitButton);
    }

    private void OnClickStartButton()
    {
        SceneManager.LoadScene(_inGameSceneName);
    }

    private void OnClickExitButton()
    {
        Application.Quit();
    }
}