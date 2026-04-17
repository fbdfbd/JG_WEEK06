using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

public class UI_CardView : MonoBehaviour
{
    [Header("Index Panel")]
    [SerializeField] private Button[] _indexButtons;

    [Header("Content Panel - Title Panel")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _categoryText;

    [Header("Content Panel - Desc Panel")]
    [SerializeField] private TextMeshProUGUI _descText;

    [Header("Content Panel - Options")]   // 예정
    [SerializeField] private Button[] _optionButtons;
    [SerializeField] private TextMeshProUGUI[] _optionTexts;

    private IReadOnlyList<WeekSelectionEntryPresentation> _currentDataList;
    private int _currentIndex = 0;

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
    }

    // [데이터 수신] 상위 메인 뷰가 데이터를 꽂아주는 단일 진입점
    public void SetCardData(IReadOnlyList<WeekSelectionEntryPresentation> dataList)
    {
        _currentDataList = dataList;

        if (_currentDataList != null && _currentDataList.Count > 0)
        {
            UpdateContentPanel(0);
        }
    }

    // [로컬 UI 상태 변경] 인덱스 버튼 클릭 시 Content 텍스트만 교체
    private void OnIndexButtonClicked(int index)
    {
        if (_currentDataList == null || index >= _currentDataList.Count) return;
        UpdateContentPanel(index);
    }

    // [출력 처리] Presentation 데이터를 화면에 출력
    private void UpdateContentPanel(int index)
    {
        _currentIndex = index;
        var data = _currentDataList[index];

        _categoryText.text = data.TypeName;
        _titleText.text = data.Title;
        _descText.text = data.OriginalText;

        // 추가 예정

        //for (int i = 0; i < _optionButtons.Length; i++)
        //{
        //    if (data.Options != null && i < data.Options.Count)
        //    {
        //        // _optionButtons[i].gameObject.SetActive(true);
        //        // _optionTexts[i].text = data.Options[i]; // 선택지 텍스트 렌더링

        //        // (선택) 만약 이미 유저가 고른 선택지(data.SelectedOptionIndex)라면 시각적 하이라이트 처리
        //    }
        //    else
        //    {
        //        _optionButtons[i].gameObject.SetActive(false); 
        //    }
        //}
    }

    // 유저가 누른 특정 옵션의 인덱스를 상위로 전달
    private void RaiseCardOptionSelectedEvent(int clickedOptionIndex)
    {
        if (_currentDataList == null || _currentIndex >= _currentDataList.Count) return;

        var currentData = _currentDataList[_currentIndex];

        OnCardOptionClicked?.Invoke(currentData.CardDefinition, clickedOptionIndex);
    }
}
