using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeekFileWindowView : MonoBehaviour
{
    public event Action OpenRequested;
    public event Action CloseRequested;
    public event Action PreviousRequested;
    public event Action NextRequested;

    private TMP_FontAsset _fontAsset;
    private RectTransform _root;
    private RectTransform _windowRoot;
    private RectTransform _cardContainerRoot;
    private CanvasGroup _windowCanvasGroup;
    private Button _openButton;
    private Button _closeButton;
    private Button _previousButton;
    private Button _nextButton;
    private TextMeshProUGUI _openButtonText;
    private TextMeshProUGUI _indexText;
    private TextMeshProUGUI _categoryNameText;
    private TextMeshProUGUI _categoryCountText;
    private TextMeshProUGUI _emptyStateText;
    private Sequence _windowSequence;

    public void Initialize(TMP_FontAsset fontAsset)
    {
        _fontAsset = fontAsset;
        BuildIfNeeded();
    }

    public void ShowEmptyState(string message)
    {
        BuildIfNeeded();
        _emptyStateText.gameObject.SetActive(true);
        _emptyStateText.text = message;
        _openButton.interactable = false;
        _openButtonText.text = "정보 파일 없음";
        _indexText.text = "-";
        _categoryNameText.text = "이번 주 분류 없음";
        _categoryCountText.text = string.Empty;

        ClearCardEntries();
    }

    public void RenderCategory(
        WeekFileCategoryPagePresentation presentation,
        Action<SO_CardInfoDefinition, int> onSelectOption)
    {
        BuildIfNeeded();
        _emptyStateText.gameObject.SetActive(false);
        _openButton.interactable = true;
        _openButtonText.text = "정보 파일 열기";
        _indexText.text = presentation.CategoryIndexLabel;
        _categoryNameText.text = presentation.CategoryName;
        _categoryCountText.text = presentation.CategoryCountLabel;
        _previousButton.interactable = presentation.CanMovePrevious;
        _nextButton.interactable = presentation.CanMoveNext;

        ClearCardEntries();

        for (int index = 0; index < presentation.Cards.Count; index++)
        {
            CreateCardEntry(presentation.Cards[index], onSelectOption, index);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_cardContainerRoot);
    }

    public void SetWindowVisible(bool visible, bool animate)
    {
        BuildIfNeeded();
        _windowSequence?.Kill();

        if (!animate)
        {
            _windowCanvasGroup.alpha = visible ? 1f : 0f;
            _windowCanvasGroup.interactable = visible;
            _windowCanvasGroup.blocksRaycasts = visible;
            _windowRoot.localScale = visible ? Vector3.one : new Vector3(0.96f, 0.96f, 1f);
            return;
        }

        _windowCanvasGroup.interactable = visible;
        _windowCanvasGroup.blocksRaycasts = visible;
        _windowSequence = DOTween.Sequence().SetUpdate(true);

        if (visible)
        {
            _windowCanvasGroup.alpha = 0f;
            _windowRoot.localScale = new Vector3(0.96f, 0.96f, 1f);
            _windowSequence.Append(_windowCanvasGroup.DOFade(1f, 0.18f));
            _windowSequence.Join(_windowRoot.DOScale(1f, 0.22f).SetEase(Ease.OutBack));
        }
        else
        {
            _windowSequence.Append(_windowCanvasGroup.DOFade(0f, 0.16f));
            _windowSequence.Join(_windowRoot.DOScale(0.96f, 0.16f).SetEase(Ease.InSine));
            _windowSequence.OnComplete(() =>
            {
                _windowCanvasGroup.interactable = false;
                _windowCanvasGroup.blocksRaycasts = false;
            });
        }
    }

    private void BuildIfNeeded()
    {
        if (_root != null)
        {
            return;
        }

        _root = transform as RectTransform;
        if (_root == null)
        {
            _root = gameObject.AddComponent<RectTransform>();
        }

        _root.anchorMin = Vector2.zero;
        _root.anchorMax = Vector2.one;
        _root.offsetMin = Vector2.zero;
        _root.offsetMax = Vector2.zero;

        Image background = AddOrGetComponent<Image>(gameObject);
        background.color = new Color(0.94f, 0.92f, 0.87f, 1f);

        VerticalLayoutGroup layout = AddOrGetComponent<VerticalLayoutGroup>(gameObject);
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = 12;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        _openButton = CreateButton("OpenFileButton", _root, out _openButtonText, new Color(0.21f, 0.31f, 0.47f, 1f), 24, "정보 파일 열기");
        AddLayoutElement(_openButton.gameObject, 72f, 0f, 0f);
        _openButton.onClick.AddListener(() => OpenRequested?.Invoke());

        _windowRoot = CreateRect("WeekFileWindow", _root);
        AddOrGetComponent<Image>(_windowRoot.gameObject).color = new Color(0.14f, 0.16f, 0.22f, 0.98f);
        AddLayoutElement(_windowRoot.gameObject, 0f, 0f, 1f);

        _windowCanvasGroup = AddOrGetComponent<CanvasGroup>(_windowRoot.gameObject);
        _windowCanvasGroup.alpha = 0f;
        _windowCanvasGroup.interactable = false;
        _windowCanvasGroup.blocksRaycasts = false;

        VerticalLayoutGroup windowLayout = AddOrGetComponent<VerticalLayoutGroup>(_windowRoot.gameObject);
        windowLayout.padding = new RectOffset(20, 20, 20, 20);
        windowLayout.spacing = 14;
        windowLayout.childForceExpandHeight = false;
        windowLayout.childControlHeight = true;
        windowLayout.childControlWidth = true;

        RectTransform topRow = CreateRect("TopRow", _windowRoot);
        HorizontalLayoutGroup topLayout = AddOrGetComponent<HorizontalLayoutGroup>(topRow.gameObject);
        topLayout.spacing = 10;
        topLayout.childControlWidth = true;
        topLayout.childControlHeight = true;
        topLayout.childForceExpandWidth = false;
        topLayout.childForceExpandHeight = false;
        AddLayoutElement(topRow.gameObject, 72f, 0f, 0f);

        _previousButton = CreateButton("PreviousButton", topRow, out _, new Color(0.28f, 0.32f, 0.39f, 1f), 22, "이전");
        AddLayoutElement(_previousButton.gameObject, 56f, 98f, 0f);
        _previousButton.onClick.AddListener(() => PreviousRequested?.Invoke());

        RectTransform centerInfo = CreateRect("CenterInfo", topRow);
        VerticalLayoutGroup centerLayout = AddOrGetComponent<VerticalLayoutGroup>(centerInfo.gameObject);
        centerLayout.spacing = 4;
        centerLayout.childControlHeight = true;
        centerLayout.childControlWidth = true;
        centerLayout.childForceExpandHeight = false;
        AddLayoutElement(centerInfo.gameObject, 0f, 0f, 1f);

        _indexText = CreateText("IndexText", centerInfo, 18, FontStyles.Bold, new Color(0.98f, 0.84f, 0.48f, 1f), "카테고리 1 / 1");
        _categoryNameText = CreateText("CategoryNameText", centerInfo, 30, FontStyles.Bold, Color.white, "방문자");
        _categoryCountText = CreateText("CategoryCountText", centerInfo, 16, FontStyles.Normal, new Color(0.8f, 0.86f, 0.93f, 1f), "이번 분류 1건");

        _nextButton = CreateButton("NextButton", topRow, out _, new Color(0.28f, 0.32f, 0.39f, 1f), 22, "다음");
        AddLayoutElement(_nextButton.gameObject, 56f, 98f, 0f);
        _nextButton.onClick.AddListener(() => NextRequested?.Invoke());

        _closeButton = CreateButton("CloseButton", topRow, out _, new Color(0.54f, 0.3f, 0.3f, 1f), 18, "닫기");
        AddLayoutElement(_closeButton.gameObject, 56f, 104f, 0f);
        _closeButton.onClick.AddListener(() => CloseRequested?.Invoke());

        _emptyStateText = CreateText("EmptyStateText", _windowRoot, 20, FontStyles.Bold, new Color(0.93f, 0.95f, 1f, 1f), string.Empty);

        RectTransform cardViewport = CreateRect("CardViewport", _windowRoot);
        AddOrGetComponent<Image>(cardViewport.gameObject).color = new Color(0.1f, 0.12f, 0.18f, 1f);
        AddOrGetComponent<RectMask2D>(cardViewport.gameObject);
        AddLayoutElement(cardViewport.gameObject, 560f, 0f, 1f);

        ScrollRect scrollRect = AddOrGetComponent<ScrollRect>(cardViewport.gameObject);
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 36f;
        scrollRect.viewport = cardViewport;

        RectTransform contentRoot = CreateStretchRect("ViewportContent", cardViewport);
        contentRoot.anchorMin = new Vector2(0f, 1f);
        contentRoot.anchorMax = new Vector2(1f, 1f);
        contentRoot.pivot = new Vector2(0f, 1f);
        contentRoot.anchoredPosition = Vector2.zero;
        contentRoot.offsetMin = Vector2.zero;
        contentRoot.offsetMax = Vector2.zero;

        _cardContainerRoot = CreateRect("CardContainer", contentRoot);
        _cardContainerRoot.anchorMin = new Vector2(0f, 1f);
        _cardContainerRoot.anchorMax = new Vector2(1f, 1f);
        _cardContainerRoot.pivot = new Vector2(0f, 1f);
        _cardContainerRoot.anchoredPosition = Vector2.zero;
        _cardContainerRoot.offsetMin = new Vector2(12f, 0f);
        _cardContainerRoot.offsetMax = new Vector2(-12f, 0f);
        ContentSizeFitter contentSizeFitter = AddOrGetComponent<ContentSizeFitter>(_cardContainerRoot.gameObject);
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        VerticalLayoutGroup cardLayout = AddOrGetComponent<VerticalLayoutGroup>(_cardContainerRoot.gameObject);
        cardLayout.padding = new RectOffset(14, 14, 14, 14);
        cardLayout.spacing = 12;
        cardLayout.childForceExpandHeight = false;
        cardLayout.childControlHeight = true;
        cardLayout.childControlWidth = true;
        scrollRect.content = _cardContainerRoot;
    }

    private void CreateCardEntry(
        WeekFileCardPresentation presentation,
        Action<SO_CardInfoDefinition, int> onSelectOption,
        int index)
    {
        RectTransform cardRoot = CreateRect($"CardEntry_{index}", _cardContainerRoot);
        AddOrGetComponent<Image>(cardRoot.gameObject).color = new Color(0.97f, 0.96f, 0.92f, 1f);
        AddLayoutElement(cardRoot.gameObject, 0f, 0f, 0f).preferredHeight = 360f;

        VerticalLayoutGroup layout = AddOrGetComponent<VerticalLayoutGroup>(cardRoot.gameObject);
        layout.padding = new RectOffset(18, 18, 16, 16);
        layout.spacing = 8;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        CreateText("TypeText", cardRoot, 16, FontStyles.Bold, new Color(0.59f, 0.34f, 0.24f, 1f), presentation.TypeName);
        CreateText("TitleText", cardRoot, 24, FontStyles.Bold, new Color(0.15f, 0.13f, 0.12f, 1f), presentation.Title);

        TextMeshProUGUI originalText = CreateText("OriginalText", cardRoot, 18, FontStyles.Normal, new Color(0.22f, 0.2f, 0.18f, 1f), presentation.OriginalText);
        AddLayoutElement(originalText.gameObject, 110f, 0f, 0f);

        RectTransform selectedPanel = CreateRect("SelectedPanel", cardRoot);
        AddOrGetComponent<Image>(selectedPanel.gameObject).color = new Color(0.87f, 0.94f, 1f, 1f);
        AddLayoutElement(selectedPanel.gameObject, 72f, 0f, 0f);
        TextMeshProUGUI selectedText = CreateText(
            "SelectedText",
            selectedPanel,
            16,
            FontStyles.Bold,
            new Color(0.13f, 0.23f, 0.31f, 1f),
            $"현재 선택: {presentation.SelectedOptionLabel}");
        selectedText.margin = new Vector4(14f, 10f, 14f, 10f);

        RectTransform optionRow = CreateRect("OptionRow", cardRoot);
        HorizontalLayoutGroup optionLayout = AddOrGetComponent<HorizontalLayoutGroup>(optionRow.gameObject);
        optionLayout.spacing = 8;
        optionLayout.childControlHeight = true;
        optionLayout.childControlWidth = true;
        optionLayout.childForceExpandHeight = false;
        optionLayout.childForceExpandWidth = true;
        AddLayoutElement(optionRow.gameObject, 56f, 0f, 0f);

        for (int optionIndex = 0; optionIndex < presentation.Options.Count; optionIndex++)
        {
            CardOptionData option = presentation.Options[optionIndex];
            Button optionButton = CreateButton(
                $"OptionButton_{optionIndex}",
                optionRow,
                out _,
                optionIndex == presentation.SelectedOptionIndex
                    ? new Color(0.19f, 0.49f, 0.78f, 1f)
                    : new Color(0.36f, 0.39f, 0.45f, 1f),
                16,
                option.Label);
            AddLayoutElement(optionButton.gameObject, 52f, 0f, 0f);

            int capturedOptionIndex = optionIndex;
            optionButton.onClick.AddListener(() => onSelectOption?.Invoke(presentation.CardDefinition, capturedOptionIndex));
        }
    }

    private void ClearCardEntries()
    {
        for (int index = _cardContainerRoot.childCount - 1; index >= 0; index--)
        {
            Transform child = _cardContainerRoot.GetChild(index);
            child.SetParent(null, false);
            Destroy(child.gameObject);
        }
    }

    private Button CreateButton(
        string objectName,
        RectTransform parent,
        out TextMeshProUGUI labelText,
        Color backgroundColor,
        float fontSize,
        string label)
    {
        RectTransform root = CreateRect(objectName, parent);
        Image background = AddOrGetComponent<Image>(root.gameObject);
        background.color = backgroundColor;

        Button button = AddOrGetComponent<Button>(root.gameObject);
        button.targetGraphic = background;

        labelText = CreateText("Label", root, fontSize, FontStyles.Bold, Color.white, label);
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.margin = new Vector4(10f, 8f, 10f, 8f);
        return button;
    }

    private TextMeshProUGUI CreateText(
        string objectName,
        RectTransform parent,
        float fontSize,
        FontStyles fontStyle,
        Color color,
        string text)
    {
        RectTransform rect = CreateStretchRect(objectName, parent);
        TextMeshProUGUI textComponent = AddOrGetComponent<TextMeshProUGUI>(rect.gameObject);
        textComponent.font = _fontAsset != null ? _fontAsset : TMP_Settings.defaultFontAsset;
        textComponent.fontSize = fontSize;
        textComponent.fontStyle = fontStyle;
        textComponent.color = color;
        textComponent.text = text;
        textComponent.textWrappingMode = TextWrappingModes.Normal;
        textComponent.overflowMode = TextOverflowModes.Overflow;
        return textComponent;
    }

    private static RectTransform CreateStretchRect(string objectName, RectTransform parent)
    {
        RectTransform rect = CreateRect(objectName, parent);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }

    private static RectTransform CreateRect(string objectName, RectTransform parent)
    {
        GameObject gameObject = new(objectName, typeof(RectTransform));
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.localScale = Vector3.one;
        return rectTransform;
    }

    private static T AddOrGetComponent<T>(GameObject gameObject) where T : Component
    {
        if (gameObject.TryGetComponent(out T component))
        {
            return component;
        }

        return gameObject.AddComponent<T>();
    }

    private static LayoutElement AddLayoutElement(GameObject gameObject, float preferredHeight, float preferredWidth, float flexibleHeight)
    {
        LayoutElement element = AddOrGetComponent<LayoutElement>(gameObject);
        element.preferredHeight = preferredHeight;
        element.preferredWidth = preferredWidth;
        element.flexibleHeight = flexibleHeight;
        return element;
    }
}
