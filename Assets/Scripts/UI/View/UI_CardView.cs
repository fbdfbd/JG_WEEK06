using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

public class UI_CardView : MonoBehaviour
{
    [Header("Index Panel")]
    [SerializeField] private Button[] _indexButtons;
    [SerializeField] private TextMeshProUGUI[] _indexTexts;

    [Header("Content Panel - Title Panel")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _categoryText;

    [Header("Content Panel - Desc Panel")]
    [SerializeField] private TextMeshProUGUI _descText;

    [Header("Content Panel - Options")]   // 예정
    [SerializeField] private Button[] _optionButtons;
    [SerializeField] private TextMeshProUGUI[] _optionTexts;

    [Header("Content Panel - Navigation")]
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;

    private IReadOnlyList<WeekSelectionEntryPresentation> _currentDataList;
    private int _currentIndex = 0;

    private IReadOnlyList<WeekSelectionCategoryGroupPresentation> _currentGroups;
    private int _currentGroupIndex = 0;
    private int _currentCardIndexInGroup = 0;

    // 상위 뷰로 전달할 이벤트
    public event Action<SO_CardInfoDefinition, int> OnCardOptionClicked;

    private void Awake()
    {
        // 인덱스 버튼 바인딩
        for (int i = 0; i < _indexButtons.Length; i++)
        {
            int index = i;
            _indexButtons[i].onClick.AddListener(() => OnIndexButtonClicked(index));
        }

        // 옵션 버튼 바인딩 
        for (int i = 0; i < _optionButtons.Length; i++)
        {
            int optionIndex = i;
            _optionButtons[i].onClick.AddListener(() => RaiseCardOptionSelectedEvent(optionIndex));
        }

        if (_prevButton != null) _prevButton.onClick.AddListener(OnPrevButtonClicked);
        if (_nextButton != null) _nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    public void SetCardGroups(IReadOnlyList<WeekSelectionCategoryGroupPresentation> groups)
    {
        _currentGroups = groups;
        _currentGroupIndex = 0;
        _currentCardIndexInGroup = 0;

        for (int i = 0; i < _indexButtons.Length; i++)
        {
            if (groups != null && i < groups.Count)
            {
                _indexButtons[i].gameObject.SetActive(true);
                if (_indexTexts != null && i < _indexTexts.Length && _indexTexts[i] != null)
                {
                    _indexTexts[i].text = groups[i].TypeName; 
                }
            }
            else
            {
                // 남는 인덱스 버튼은 안 보이게 숨김 처리 
                _indexButtons[i].gameObject.SetActive(false);
            }
        }

        UpdateCurrentGroupedCard();
    }

    // [로컬 UI 상태 변경] 인덱스 버튼 클릭 시 Content 텍스트만 교체
    private void OnIndexButtonClicked(int index)
    {
        if (_currentGroups == null || index >= _currentGroups.Count) return;

        _currentGroupIndex = index;
        _currentCardIndexInGroup = 0; 

        UpdateCurrentGroupedCard();
    }

    private void OnPrevButtonClicked()
    {
        if (_currentCardIndexInGroup > 0)
        {
            _currentCardIndexInGroup--;
            UpdateCurrentGroupedCard();
        }
    }

    private void OnNextButtonClicked()
    {
        if (_currentGroups == null || _currentGroupIndex >= _currentGroups.Count) return;

        var currentGroup = _currentGroups[_currentGroupIndex];
        if (_currentCardIndexInGroup < currentGroup.Entries.Count - 1)
        {
            _currentCardIndexInGroup++;
            UpdateCurrentGroupedCard();
        }
    }

    private void UpdateCurrentGroupedCard()
    {
        // 방어 코드: 데이터가 비어있을 때의 처리
        if (_currentGroups == null || _currentGroups.Count == 0) return;
        if (_currentGroupIndex >= _currentGroups.Count) return;

        var currentGroup = _currentGroups[_currentGroupIndex];

        // 현재 그룹에 카드가 없다면 화면을 비우고 리턴
        if (currentGroup.Entries == null || currentGroup.Entries.Count == 0)
        {
            // (필요 시 UI를 끄거나 비어있다는 텍스트 표시)
            return;
        }

        // 현재 선택된 카드 정보 추출
        var currentCardData = currentGroup.Entries[_currentCardIndexInGroup];

        // 텍스트 렌더링
        _categoryText.text = currentGroup.TypeName; // 또는 currentCardData.TypeName
        _titleText.text = currentCardData.Title;
        _descText.text = currentCardData.OriginalText;

        // 옵션 버튼 렌더링 (예정)

        //for (int i = 0; i < _optionButtons.Length; i++)
        //{
        //    if (currentCardData.Options != null && i < currentCardData.Options.Count)
        //    {
        //        // _optionButtons[i].gameObject.SetActive(true);
        //        // _optionTexts[i].text = currentCardData.Options[i];
        //    }
        //    else
        //    {
        //        _optionButtons[i].gameObject.SetActive(false);
        //    }
        //}

        if (_prevButton != null)
        {
            // 첫 번째 카드면 이전 버튼 비활성화
            _prevButton.interactable = (_currentCardIndexInGroup > 0);
        }

        if (_nextButton != null)
        {
            // 마지막 카드면 다음 버튼 비활성화
            _nextButton.interactable = (_currentCardIndexInGroup < currentGroup.Entries.Count - 1);
        }
    }

    // 유저가 누른 특정 옵션의 인덱스를 상위로 전달
    private void RaiseCardOptionSelectedEvent(int clickedOptionIndex)
    {
        if (_currentGroups == null || _currentGroupIndex >= _currentGroups.Count) return;

        var currentGroup = _currentGroups[_currentGroupIndex];
        if (_currentCardIndexInGroup >= currentGroup.Entries.Count) return;

        // 현재 보고 있는 정확한 카드의 Definition을 상위로 쏴준다.
        var currentCardData = currentGroup.Entries[_currentCardIndexInGroup];

        OnCardOptionClicked?.Invoke(currentCardData.CardDefinition, clickedOptionIndex);
    }


}
