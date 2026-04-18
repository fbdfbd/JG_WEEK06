using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ChoiceView : MonoBehaviour
{
    [SerializeField] private Button[] _choiceButtons;
    [SerializeField] private TextMeshProUGUI[] _choiceButtonTexts;

    public event Action<int> OnChoiceSelected;

    private void Awake()
    {
        for (int i = 0; i < _choiceButtons.Length; i++)
        {
            int index = i;
            _choiceButtons[i].onClick.AddListener(() => OnChoiceSelected?.Invoke(index));
        }
    }

    public void SetChoices(IReadOnlyList<string> choiceLabels)
    {
        for (int i = 0; i < _choiceButtons.Length; i++)
        {
            if (i < choiceLabels.Count)
            {
                _choiceButtons[i].gameObject.SetActive(true);
                _choiceButtonTexts[i].text = choiceLabels[i];
            }
            else
            {
                _choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
