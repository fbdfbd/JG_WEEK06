using TMPro;
using UnityEngine;

public class UI_DialogView : MonoBehaviour
{
    [SerializeField] private GameObject _nameTagPanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _mainContentText;

    public void SetDialogue(string speakerName, string content)
    {
        _mainContentText.text = content;

        if(!string.IsNullOrEmpty(speakerName))
        {
            _nameTagPanel.SetActive(true);
            _nameText.text = speakerName;
        }
        else
        {
            _nameTagPanel.SetActive(false);
        }
    }
}
