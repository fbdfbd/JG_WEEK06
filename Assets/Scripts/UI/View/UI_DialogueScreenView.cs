using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DialogueScreenView : WeekFlowViewBase
{
    [Header("Sub Panel")]
    [SerializeField] private UI_ChoiceView _choicePanel;
    [SerializeField] private UI_DialogView _dialogPanel;

    [Header("Continue Action")]
    [SerializeField] private Button _continueButton;

    private void Awake()
    {
        // 하위 패널의 이벤트를 구독하여 내 이벤트(WeekFlowViewBase)로 토스 (릴레이)
        _choicePanel.OnChoiceSelected += (index) =>
        {
            RaiseInteractiveEventChoiceSelected(index);
        };

        if (_continueButton != null)
        {
            _continueButton.onClick.AddListener(() =>
            {
                RaiseInteractiveEventContinueRequested();
            });
        }
    }

    public override void ShowInteractiveEvent(InteractiveEventPresentation presentation)
    {
        gameObject.SetActive(true);

        if (presentation.DialogueLines != null && presentation.DialogueLines.Count > 0)
        {
            _dialogPanel.gameObject.SetActive(true);

            string speakerName = presentation.DialogueLines[0].SpeakerName;
            string content = presentation.DialogueLines[0].Text;

            _dialogPanel.SetDialogue(speakerName, content);
        }
        else
        {
            _dialogPanel.gameObject.SetActive(false);
        }

        // 선택지 패널에 데이터 꽂아주기 (임시로 Label 리스트를 추출한다고 가정)
        bool hasChoices = presentation.Choices != null && presentation.Choices.Count > 0;
        _choicePanel.gameObject.SetActive(hasChoices);

        if (hasChoices)
        {
            // presentation.Choices에서 라벨만 뽑아서 넘겨줌
            List<string> labels = new List<string>();
            // foreach (var choice in presentation.Choices) labels.Add(choice.Label);
            _choicePanel.SetChoices(labels);
        }

        // 선택지가 없으면 Continue 버튼 활성화
        _continueButton.gameObject.SetActive(!hasChoices);
    }

    public override void HideTransientViews()
    {
        gameObject.SetActive(false);
    }

}
