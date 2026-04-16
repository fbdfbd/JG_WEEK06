using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestWeekRunnerBehaviour : MonoBehaviour
{
    private const float ExpandedPileHeight = 280f;
    private const float CollapsedPileHeight = 0f;

    [Header("Week")]
    [SerializeField] private SO_WeekDefinition _weekDefinition;
    [SerializeField] private SO_WeekDefinition[] _weekDefinitions = Array.Empty<SO_WeekDefinition>();

    [Header("Font")]
    [SerializeField] private TMP_FontAsset _nemoDialogueFont;

    private readonly WeekRunner _weekRunner = new();
    private readonly List<CardListItemRuntime> _cardItemViews = new();
    private readonly List<CardOptionRuntime> _cardOptionViews = new();
    private readonly Dictionary<EChildStatusType, Slider> _statSliders = new();
    private readonly Dictionary<EChildStatusType, TextMeshProUGUI> _statValueTexts = new();
    private readonly Dictionary<EChildStatusType, Image> _statFillImages = new();
    private readonly Dictionary<EChildStatusType, List<Image>> _statPipImages = new();

    private RuntimeChildState _childState;
    private RuntimeWeekResult _lastWeekResult;
    private NemoActorView _nemoActorView;
    private WeekFileWindowView _weekFileWindowView;
    private WeekFileWindowPresenter _weekFileWindowPresenter;

    private RectTransform _dayPanelRoot;
    private RectTransform _cardPanelRoot;
    private RectTransform _buttonPanelRoot;
    private RectTransform _characterPanelRoot;

    private RectTransform _cardDeskRoot;
    private RectTransform _cardPileContentRoot;
    private RectTransform _cardDetailRoot;
    private RectTransform _cardOptionButtonRoot;
    private RectTransform _nemoStageRoot;
    private RectTransform _weekFeedbackOverlayRoot;
    private RectTransform _weekFeedbackEventRoot;
    private RectTransform _weekEventOverlayRoot;
    private RectTransform _privateDialogueOverlayRoot;
    private RectTransform _privateDialogueChoiceRoot;

    private LayoutElement _cardPileLayoutElement;
    private CanvasGroup _cardPileCanvasGroup;
    private CanvasGroup _weekFeedbackOverlayCanvasGroup;
    private CanvasGroup _weekEventOverlayCanvasGroup;
    private CanvasGroup _privateDialogueOverlayCanvasGroup;

    private TextMeshProUGUI _weekIndexText;
    private TextMeshProUGUI _weekTitleText;
    private TextMeshProUGUI _weekSummaryText;
    private TextMeshProUGUI _pileToggleLabel;
    private TextMeshProUGUI _detailTypeText;
    private TextMeshProUGUI _detailTitleText;
    private TextMeshProUGUI _detailOriginalText;
    private TextMeshProUGUI _detailPreviewTitleText;
    private TextMeshProUGUI _detailPreviewText;
    private TextMeshProUGUI _statusMessageText;
    private TextMeshProUGUI _weekSummaryReportText;
    private TextMeshProUGUI _nemoStateHintText;
    private TextMeshProUGUI _weekFeedbackTitleText;
    private TextMeshProUGUI _weekFeedbackSummaryText;
    private TextMeshProUGUI _weekFeedbackStatDeltaText;
    private TextMeshProUGUI _weekFeedbackCloseButtonLabelText;
    private TextMeshProUGUI _weekEventTitleText;
    private TextMeshProUGUI _weekEventSituationText;
    private TextMeshProUGUI _weekEventReactionText;
    private TextMeshProUGUI _weekEventStatDeltaText;
    private TextMeshProUGUI _weekEventContinueButtonLabelText;
    private TextMeshProUGUI _privateDialogueTitleText;
    private TextMeshProUGUI _privateDialogueOpeningText;
    private TextMeshProUGUI _privateDialogueResponseText;
    private TextMeshProUGUI _privateDialogueContinueButtonLabelText;

    private Image _detailPanelImage;
    private Image _pileToggleImage;

    private Button _pileToggleButton;
    private Button _runWeekButton;
    private Button _resetSelectionsButton;
    private Button _resetChildStateButton;
    private Button _weekFeedbackCloseButton;
    private Button _weekEventContinueButton;
    private Button _privateDialogueContinueButton;

    private int[] _selectedOptionIndices = Array.Empty<int>();
    private int _focusedCardIndex;
    private int _currentWeekSequenceIndex;
    private bool _isPileExpanded = true;
    private bool _uiBuilt;
    private bool _endingReached;

    private SO_WeekDefinition _cachedWeekDefinition;
    private int _cachedCardCount = -1;
    private Sequence _pileSequence;
    private Sequence _detailSequence;
    private Sequence _weekFeedbackOverlaySequence;
    private Sequence _weekEventOverlaySequence;
    private Sequence _privateDialogueOverlaySequence;
    private readonly List<Button> _privateDialogueChoiceButtons = new();
    private readonly List<TextMeshProUGUI> _privateDialogueChoiceLabels = new();
    private WeekFixedEventPresentation _pendingWeekEventPresentation;
    private WeekPrivateDialoguePresentation _pendingPrivateDialoguePresentation;
    private bool _pendingEndingAfterNarrative;
    private bool _weekEventApplied;
    private bool _privateDialogueChoiceApplied;
    private WeekDialogueChoicePresentation _selectedDialogueChoicePresentation;
    private string _statusMessage = "주차 실행 준비 완료";

    private void Awake()
    {
        _childState = new RuntimeChildState();
        InitializeWeekSequence();
        EnsureSelectionBuffer();
        EnsureNemoActorView();
        BuildRuntimeUI();
        RefreshUI(true);
        PresentDefaultNemoState();
    }

    private void OnValidate()
    {
        InitializeWeekSequence();
        EnsureSelectionBuffer();
    }

    private void OnDestroy()
    {
        _pileSequence?.Kill();
        _detailSequence?.Kill();
        _weekFeedbackOverlaySequence?.Kill();
        _weekEventOverlaySequence?.Kill();
        _privateDialogueOverlaySequence?.Kill();
    }

    private void BuildRuntimeUI()
    {
        if (_uiBuilt)
        {
            return;
        }

        EnsurePanelRoots();
        BuildHeaderView();
        BuildWeekFileDeskView();
        BuildActionView();
        BuildCharacterView();
        BuildWeekFeedbackOverlayView();
        BuildWeekEventOverlayView();
        BuildPrivateDialogueOverlayView();

        _uiBuilt = true;
    }

    private void BuildHeaderView()
    {
        RectTransform root = GetOrCreateStretchRect("WeekHeaderRuntime", _dayPanelRoot);
        AddOrGetImage(root.gameObject, new Color(0.1f, 0.14f, 0.22f, 0.92f));

        VerticalLayoutGroup layout = AddOrGetComponent<VerticalLayoutGroup>(root.gameObject);
        layout.padding = new RectOffset(24, 24, 18, 18);
        layout.spacing = 8;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        _weekIndexText = CreateText("WeekIndexText", root, 22, FontStyles.Bold, new Color(0.95f, 0.87f, 0.52f, 1f), "WEEK 1");
        _weekTitleText = CreateText("WeekTitleText", root, 34, FontStyles.Bold, Color.white, "이번 주 브리핑");
        _weekSummaryText = CreateText("WeekSummaryText", root, 18, FontStyles.Normal, new Color(0.88f, 0.91f, 0.95f, 1f), string.Empty);
        _weekSummaryText.textWrappingMode = TextWrappingModes.Normal;
        _weekSummaryText.overflowMode = TextOverflowModes.Overflow;
    }

    private void BuildCardDeskView()
    {
        _cardDeskRoot = GetOrCreateStretchRect("WeekDeskRuntime", _cardPanelRoot);
        AddOrGetImage(_cardDeskRoot.gameObject, new Color(0.96f, 0.94f, 0.88f, 0.96f));

        VerticalLayoutGroup layout = AddOrGetComponent<VerticalLayoutGroup>(_cardDeskRoot.gameObject);
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = 12;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        _pileToggleButton = CreateButton(
            "CardPileButton",
            _cardDeskRoot,
            out _pileToggleLabel,
            out _pileToggleImage,
            new Color(0.22f, 0.3f, 0.44f, 1f),
            22,
            FontStyles.Bold,
            Color.white);
        AddLayoutElement(_pileToggleButton.gameObject, 64f, 0f, 1f);
        _pileToggleButton.onClick.AddListener(ToggleCardPile);

        RectTransform pileContainer = GetOrCreateRect("CardPileContainer", _cardDeskRoot);
        AddOrGetImage(pileContainer.gameObject, new Color(0.91f, 0.89f, 0.82f, 1f));
        AddLayoutElement(pileContainer.gameObject, ExpandedPileHeight, 0f, 1f);
        _cardPileLayoutElement = pileContainer.GetComponent<LayoutElement>();
        _cardPileCanvasGroup = AddOrGetComponent<CanvasGroup>(pileContainer.gameObject);

        _cardPileContentRoot = GetOrCreateStretchRect("CardPileContent", pileContainer);
        VerticalLayoutGroup pileLayout = AddOrGetComponent<VerticalLayoutGroup>(_cardPileContentRoot.gameObject);
        pileLayout.padding = new RectOffset(12, 12, 12, 12);
        pileLayout.spacing = 10;
        pileLayout.childForceExpandHeight = false;
        pileLayout.childControlHeight = true;
        pileLayout.childControlWidth = true;

        _cardDetailRoot = GetOrCreateRect("CardDetailPanel", _cardDeskRoot);
        _detailPanelImage = AddOrGetImage(_cardDetailRoot.gameObject, new Color(1f, 0.99f, 0.95f, 1f));
        AddLayoutElement(_cardDetailRoot.gameObject, 0f, 0f, 1f);

        VerticalLayoutGroup detailLayout = AddOrGetComponent<VerticalLayoutGroup>(_cardDetailRoot.gameObject);
        detailLayout.padding = new RectOffset(18, 18, 18, 18);
        detailLayout.spacing = 10;
        detailLayout.childForceExpandHeight = false;
        detailLayout.childControlHeight = true;
        detailLayout.childControlWidth = true;

        _detailTypeText = CreateText("DetailTypeText", _cardDetailRoot, 18, FontStyles.Bold, new Color(0.55f, 0.32f, 0.21f, 1f), string.Empty);
        _detailTitleText = CreateText("DetailTitleText", _cardDetailRoot, 28, FontStyles.Bold, new Color(0.17f, 0.14f, 0.12f, 1f), string.Empty);
        _detailOriginalText = CreateText("DetailOriginalText", _cardDetailRoot, 19, FontStyles.Normal, new Color(0.25f, 0.22f, 0.2f, 1f), string.Empty);
        _detailOriginalText.textWrappingMode = TextWrappingModes.Normal;
        _detailOriginalText.overflowMode = TextOverflowModes.Overflow;

        _detailPreviewTitleText = CreateText("DetailPreviewTitleText", _cardDetailRoot, 18, FontStyles.Bold, new Color(0.16f, 0.32f, 0.44f, 1f), "네모에게 닿는 정보");

        RectTransform previewPanel = GetOrCreateRect("PreviewPanel", _cardDetailRoot);
        AddOrGetImage(previewPanel.gameObject, new Color(0.9f, 0.95f, 1f, 1f));
        AddLayoutElement(previewPanel.gameObject, 120f, 0f, 1f);

        _detailPreviewText = CreateText("DetailPreviewText", GetOrCreateStretchRect("PreviewTextRoot", previewPanel), 18, FontStyles.Normal, new Color(0.14f, 0.2f, 0.28f, 1f), string.Empty);
        _detailPreviewText.margin = new Vector4(16f, 14f, 16f, 12f);
        _detailPreviewText.textWrappingMode = TextWrappingModes.Normal;
        _detailPreviewText.overflowMode = TextOverflowModes.Overflow;

        _cardOptionButtonRoot = GetOrCreateRect("OptionButtonRoot", _cardDetailRoot);
        VerticalLayoutGroup optionLayout = AddOrGetComponent<VerticalLayoutGroup>(_cardOptionButtonRoot.gameObject);
        optionLayout.spacing = 10;
        optionLayout.childForceExpandHeight = false;
        optionLayout.childControlHeight = true;
        optionLayout.childControlWidth = true;
    }

    private void BuildWeekFileDeskView()
    {
        _cardDeskRoot = GetOrCreateStretchRect("WeekDeskRuntime", _cardPanelRoot);
        AddOrGetImage(_cardDeskRoot.gameObject, new Color(0.96f, 0.94f, 0.88f, 0.96f));

        _weekFileWindowView = AddOrGetComponent<WeekFileWindowView>(_cardDeskRoot.gameObject);
        _weekFileWindowView.Initialize(GetUIFont());
        _weekFileWindowPresenter = new WeekFileWindowPresenter(
            _weekFileWindowView,
            GetSelectionIndexForCard,
            SetSelectionIndexForCard);
        _weekFileWindowPresenter.SetWeek(GetOrderedCardEntries());
    }

    private void BuildActionView()
    {
        RectTransform root = GetOrCreateStretchRect("WeekActionRuntime", _buttonPanelRoot);
        AddOrGetImage(root.gameObject, new Color(0.13f, 0.15f, 0.19f, 0.94f));

        VerticalLayoutGroup layout = AddOrGetComponent<VerticalLayoutGroup>(root.gameObject);
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = 14;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        RectTransform buttonRow = GetOrCreateRect("ActionButtonRow", root);
        HorizontalLayoutGroup buttonRowLayout = AddOrGetComponent<HorizontalLayoutGroup>(buttonRow.gameObject);
        buttonRowLayout.spacing = 10;
        buttonRowLayout.childForceExpandWidth = true;
        buttonRowLayout.childForceExpandHeight = false;
        buttonRowLayout.childControlWidth = true;
        buttonRowLayout.childControlHeight = true;
        AddLayoutElement(buttonRow.gameObject, 84f, 0f, 1f);

        _runWeekButton = CreateButton("RunWeekButton", buttonRow, out _, out _, new Color(0.34f, 0.54f, 0.28f, 1f), 28, FontStyles.Bold, Color.white, "주차 진행");
        AddLayoutElement(_runWeekButton.gameObject, 84f, 0f, 1f);
        _runWeekButton.onClick.AddListener(RunCurrentWeek);

        _resetSelectionsButton = CreateButton("ResetSelectionButton", buttonRow, out _, out _, new Color(0.39f, 0.36f, 0.32f, 1f), 20, FontStyles.Bold, Color.white, "선택 초기화");
        AddLayoutElement(_resetSelectionsButton.gameObject, 72f, 0f, 1f);
        _resetSelectionsButton.onClick.AddListener(HandleResetSelections);

        _resetChildStateButton = CreateButton("ResetChildButton", buttonRow, out _, out _, new Color(0.53f, 0.31f, 0.31f, 1f), 20, FontStyles.Bold, Color.white, "네모 리셋");
        AddLayoutElement(_resetChildStateButton.gameObject, 72f, 0f, 1f);
        _resetChildStateButton.onClick.AddListener(HandleResetChildState);

        _statusMessageText = CreateText("StatusMessageText", root, 18, FontStyles.Bold, new Color(0.96f, 0.89f, 0.58f, 1f), string.Empty);
        _weekSummaryReportText = CreateText("WeekSummaryReportText", root, 17, FontStyles.Normal, new Color(0.93f, 0.95f, 0.98f, 1f), string.Empty);
        _weekSummaryReportText.textWrappingMode = TextWrappingModes.Normal;
        _weekSummaryReportText.overflowMode = TextOverflowModes.Overflow;
    }

    private void BuildCharacterView()
    {
        RectTransform root = GetOrCreateStretchRect("CharacterRuntime", _characterPanelRoot);
        AddOrGetImage(root.gameObject, new Color(0.11f, 0.12f, 0.16f, 0.96f));

        VerticalLayoutGroup layout = AddOrGetComponent<VerticalLayoutGroup>(root.gameObject);
        layout.padding = new RectOffset(16, 16, 16, 16);
        layout.spacing = 14;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        _nemoStageRoot = GetOrCreateRect("NemoStageRoot", root);
        AddOrGetImage(_nemoStageRoot.gameObject, new Color(0.16f, 0.19f, 0.24f, 1f));
        AddLayoutElement(_nemoStageRoot.gameObject, 320f, 0f, 1f);

        _nemoStateHintText = CreateText("NemoStateHintText", root, 18, FontStyles.Bold, new Color(0.92f, 0.95f, 0.99f, 1f), "네모의 현재 상태");

        RectTransform statusRoot = GetOrCreateRect("StatusRoot", root);
        VerticalLayoutGroup statusLayout = AddOrGetComponent<VerticalLayoutGroup>(statusRoot.gameObject);
        statusLayout.spacing = 12;
        statusLayout.childForceExpandHeight = false;
        statusLayout.childControlHeight = true;
        statusLayout.childControlWidth = true;
        AddLayoutElement(statusRoot.gameObject, 0f, 0f, 1f);

        CreateStatRow(statusRoot, EChildStatusType.Trust, "신뢰", new Color(0.47f, 0.84f, 0.67f, 1f));
        CreateStatRow(statusRoot, EChildStatusType.Curiosity, "호기심", new Color(0.98f, 0.79f, 0.36f, 1f));
        CreateStatRow(statusRoot, EChildStatusType.Anxiety, "불안", new Color(0.94f, 0.53f, 0.49f, 1f));
        CreateStatRow(statusRoot, EChildStatusType.Obedience, "순응", new Color(0.56f, 0.71f, 0.96f, 1f));

        EnsureNemoActorView();
        _nemoActorView.SetStageRoot(_nemoStageRoot);
        _nemoActorView.SetDialogueFont(GetUIFont());
    }

    private void BuildWeekFeedbackOverlayView()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        _weekFeedbackOverlayRoot = GetOrCreateStretchRect("WeekFeedbackOverlayRuntime", canvas.GetComponent<RectTransform>());
        Image dimImage = AddOrGetImage(_weekFeedbackOverlayRoot.gameObject, new Color(0.04f, 0.05f, 0.08f, 0.88f));
        dimImage.raycastTarget = true;

        _weekFeedbackOverlayCanvasGroup = AddOrGetComponent<CanvasGroup>(_weekFeedbackOverlayRoot.gameObject);
        _weekFeedbackOverlayCanvasGroup.alpha = 0f;
        _weekFeedbackOverlayCanvasGroup.interactable = false;
        _weekFeedbackOverlayCanvasGroup.blocksRaycasts = false;

        RectTransform panel = CreateUIRect(
            "WeekFeedbackPanel",
            _weekFeedbackOverlayRoot,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            new Vector2(1040f, 720f));
        AddOrGetImage(panel.gameObject, new Color(0.11f, 0.13f, 0.18f, 0.98f));

        VerticalLayoutGroup layout = AddOrGetComponent<VerticalLayoutGroup>(panel.gameObject);
        layout.padding = new RectOffset(36, 36, 30, 30);
        layout.spacing = 18;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        _weekFeedbackTitleText = CreateText("OverlayTitle", panel, 34, FontStyles.Bold, new Color(0.98f, 0.86f, 0.52f, 1f), "네모의 하루");
        AddLayoutElement(_weekFeedbackTitleText.gameObject, 0f, 0f, 0f).preferredHeight = 56f;

        _weekFeedbackStatDeltaText = CreateText("OverlayStatDelta", panel, 20, FontStyles.Bold, new Color(0.77f, 0.91f, 1f, 1f), string.Empty);

        RectTransform eventPanel = GetOrCreateRect("OverlayEventPanel", panel);
        AddOrGetImage(eventPanel.gameObject, new Color(0.15f, 0.18f, 0.25f, 1f));
        AddLayoutElement(eventPanel.gameObject, 320f, 0f, 1f);

        _weekFeedbackEventRoot = GetOrCreateStretchRect("OverlayEventRoot", eventPanel);
        VerticalLayoutGroup eventLayout = AddOrGetComponent<VerticalLayoutGroup>(_weekFeedbackEventRoot.gameObject);
        eventLayout.padding = new RectOffset(22, 22, 22, 22);
        eventLayout.spacing = 16;
        eventLayout.childForceExpandHeight = false;
        eventLayout.childControlHeight = true;
        eventLayout.childControlWidth = true;

        _weekFeedbackSummaryText = CreateText("OverlaySummary", panel, 24, FontStyles.Bold, Color.white, string.Empty);
        _weekFeedbackSummaryText.textWrappingMode = TextWrappingModes.Normal;
        _weekFeedbackSummaryText.overflowMode = TextOverflowModes.Overflow;

        _weekFeedbackCloseButton = CreateButton(
            "OverlayCloseButton",
            panel,
            out _weekFeedbackCloseButtonLabelText,
            out _,
            new Color(0.28f, 0.49f, 0.76f, 1f),
            22,
            FontStyles.Bold,
            Color.white,
            "닫기");
        AddLayoutElement(_weekFeedbackCloseButton.gameObject, 72f, 0f, 0f);
        _weekFeedbackCloseButton.onClick.AddListener(HideWeekFeedbackOverlay);
    }

    private void BuildWeekEventOverlayView()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        _weekEventOverlayRoot = GetOrCreateStretchRect("WeekEventOverlayRuntime", canvas.GetComponent<RectTransform>());
        AddOrGetImage(_weekEventOverlayRoot.gameObject, new Color(0.05f, 0.06f, 0.1f, 0.9f)).raycastTarget = true;

        _weekEventOverlayCanvasGroup = AddOrGetComponent<CanvasGroup>(_weekEventOverlayRoot.gameObject);
        _weekEventOverlayCanvasGroup.alpha = 0f;
        _weekEventOverlayCanvasGroup.interactable = false;
        _weekEventOverlayCanvasGroup.blocksRaycasts = false;

        RectTransform panel = CreateUIRect(
            "WeekEventPanel",
            _weekEventOverlayRoot,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            new Vector2(980f, 620f));
        AddOrGetImage(panel.gameObject, new Color(0.12f, 0.14f, 0.2f, 0.98f));

        VerticalLayoutGroup layout = AddOrGetComponent<VerticalLayoutGroup>(panel.gameObject);
        layout.padding = new RectOffset(36, 36, 30, 30);
        layout.spacing = 18;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        _weekEventTitleText = CreateText("WeekEventTitle", panel, 32, FontStyles.Bold, new Color(0.98f, 0.86f, 0.52f, 1f), "주차 확정 사건");
        _weekEventSituationText = CreateText("WeekEventSituation", panel, 22, FontStyles.Normal, new Color(0.93f, 0.95f, 1f, 1f), string.Empty);
        _weekEventSituationText.textWrappingMode = TextWrappingModes.Normal;

        RectTransform reactionPanel = GetOrCreateRect("WeekEventReactionPanel", panel);
        AddOrGetImage(reactionPanel.gameObject, new Color(0.17f, 0.2f, 0.28f, 1f));
        AddLayoutElement(reactionPanel.gameObject, 210f, 0f, 0f);

        _weekEventReactionText = CreateText("WeekEventReaction", reactionPanel, 24, FontStyles.Bold, Color.white, string.Empty);
        _weekEventReactionText.margin = new Vector4(22f, 20f, 22f, 20f);
        _weekEventReactionText.textWrappingMode = TextWrappingModes.Normal;

        _weekEventStatDeltaText = CreateText("WeekEventStatDelta", panel, 20, FontStyles.Bold, new Color(0.77f, 0.91f, 1f, 1f), string.Empty);

        _weekEventContinueButton = CreateButton(
            "WeekEventContinueButton",
            panel,
            out _weekEventContinueButtonLabelText,
            out _,
            new Color(0.31f, 0.53f, 0.78f, 1f),
            22,
            FontStyles.Bold,
            Color.white,
            "네모와 대화하기");
        AddLayoutElement(_weekEventContinueButton.gameObject, 72f, 0f, 0f);
        _weekEventContinueButton.onClick.AddListener(HandleWeekEventContinue);
    }

    private void BuildPrivateDialogueOverlayView()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        _privateDialogueOverlayRoot = GetOrCreateStretchRect("PrivateDialogueOverlayRuntime", canvas.GetComponent<RectTransform>());
        AddOrGetImage(_privateDialogueOverlayRoot.gameObject, new Color(0.05f, 0.05f, 0.09f, 0.9f)).raycastTarget = true;

        _privateDialogueOverlayCanvasGroup = AddOrGetComponent<CanvasGroup>(_privateDialogueOverlayRoot.gameObject);
        _privateDialogueOverlayCanvasGroup.alpha = 0f;
        _privateDialogueOverlayCanvasGroup.interactable = false;
        _privateDialogueOverlayCanvasGroup.blocksRaycasts = false;

        RectTransform panel = CreateUIRect(
            "PrivateDialoguePanel",
            _privateDialogueOverlayRoot,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            new Vector2(980f, 620f));
        AddOrGetImage(panel.gameObject, new Color(0.1f, 0.11f, 0.17f, 0.98f));

        VerticalLayoutGroup layout = AddOrGetComponent<VerticalLayoutGroup>(panel.gameObject);
        layout.padding = new RectOffset(36, 36, 30, 30);
        layout.spacing = 18;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        _privateDialogueTitleText = CreateText("PrivateDialogueTitle", panel, 32, FontStyles.Bold, new Color(0.98f, 0.86f, 0.52f, 1f), "개인 대화");
        _privateDialogueOpeningText = CreateText("PrivateDialogueOpening", panel, 24, FontStyles.Bold, Color.white, string.Empty);
        _privateDialogueOpeningText.textWrappingMode = TextWrappingModes.Normal;

        _privateDialogueChoiceRoot = GetOrCreateRect("PrivateDialogueChoiceRoot", panel);
        VerticalLayoutGroup choiceLayout = AddOrGetComponent<VerticalLayoutGroup>(_privateDialogueChoiceRoot.gameObject);
        choiceLayout.spacing = 10;
        choiceLayout.childForceExpandHeight = false;
        choiceLayout.childControlHeight = true;
        choiceLayout.childControlWidth = true;

        for (int index = 0; index < 3; index++)
        {
            Button choiceButton = CreateButton(
                $"PrivateDialogueChoice_{index}",
                _privateDialogueChoiceRoot,
                out TextMeshProUGUI choiceLabel,
                out _,
                new Color(0.29f, 0.33f, 0.41f, 1f),
                20,
                FontStyles.Bold,
                Color.white,
                $"선택지 {index + 1}");
            AddLayoutElement(choiceButton.gameObject, 62f, 0f, 0f);

            int capturedIndex = index;
            choiceButton.onClick.AddListener(() => HandlePrivateDialogueChoice(capturedIndex));
            _privateDialogueChoiceButtons.Add(choiceButton);
            _privateDialogueChoiceLabels.Add(choiceLabel);
        }

        RectTransform responsePanel = GetOrCreateRect("PrivateDialogueResponsePanel", panel);
        AddOrGetImage(responsePanel.gameObject, new Color(0.16f, 0.19f, 0.28f, 1f));
        AddLayoutElement(responsePanel.gameObject, 180f, 0f, 0f);

        _privateDialogueResponseText = CreateText("PrivateDialogueResponse", responsePanel, 22, FontStyles.Normal, new Color(0.95f, 0.97f, 1f, 1f), string.Empty);
        _privateDialogueResponseText.margin = new Vector4(20f, 18f, 20f, 18f);
        _privateDialogueResponseText.textWrappingMode = TextWrappingModes.Normal;

        _privateDialogueContinueButton = CreateButton(
            "PrivateDialogueContinueButton",
            panel,
            out _privateDialogueContinueButtonLabelText,
            out _,
            new Color(0.31f, 0.53f, 0.78f, 1f),
            22,
            FontStyles.Bold,
            Color.white,
            "마무리하기");
        AddLayoutElement(_privateDialogueContinueButton.gameObject, 72f, 0f, 0f);
        _privateDialogueContinueButton.onClick.AddListener(FinishPrivateDialogue);
        _privateDialogueContinueButton.gameObject.SetActive(false);
    }

    private void BuildCardListItems()
    {
        _cardItemViews.Clear();

        foreach (WeekCardEntryData entry in GetOrderedCardEntries())
        {
            RectTransform itemRoot = CreateRect($"CardItem_{_cardItemViews.Count}", _cardPileContentRoot);
            Image background = AddOrGetImage(itemRoot.gameObject, new Color(0.97f, 0.96f, 0.92f, 1f));
            AddLayoutElement(itemRoot.gameObject, 76f, 0f, 1f);

            Button button = AddOrGetComponent<Button>(itemRoot.gameObject);
            button.targetGraphic = background;

            HorizontalLayoutGroup rowLayout = AddOrGetComponent<HorizontalLayoutGroup>(itemRoot.gameObject);
            rowLayout.padding = new RectOffset(12, 12, 10, 10);
            rowLayout.spacing = 8;
            rowLayout.childAlignment = TextAnchor.MiddleLeft;
            rowLayout.childForceExpandWidth = true;
            rowLayout.childForceExpandHeight = false;
            rowLayout.childControlHeight = true;
            rowLayout.childControlWidth = true;

            RectTransform textRoot = CreateRect("TextRoot", itemRoot);
            VerticalLayoutGroup textLayout = AddOrGetComponent<VerticalLayoutGroup>(textRoot.gameObject);
            textLayout.spacing = 4;
            textLayout.childForceExpandHeight = false;
            textLayout.childControlHeight = true;
            textLayout.childControlWidth = true;
            AddLayoutElement(textRoot.gameObject, 0f, 0f, 1f);

            TextMeshProUGUI titleText = CreateText("TitleText", textRoot, 18, FontStyles.Bold, new Color(0.16f, 0.14f, 0.12f, 1f), string.Empty);
            titleText.textWrappingMode = TextWrappingModes.NoWrap;
            titleText.overflowMode = TextOverflowModes.Ellipsis;

            TextMeshProUGUI metaText = CreateText("MetaText", textRoot, 15, FontStyles.Normal, new Color(0.42f, 0.38f, 0.33f, 1f), string.Empty);
            metaText.textWrappingMode = TextWrappingModes.NoWrap;
            metaText.overflowMode = TextOverflowModes.Ellipsis;

            int itemIndex = _cardItemViews.Count;
            button.onClick.AddListener(() => FocusCard(itemIndex));

            _cardItemViews.Add(new CardListItemRuntime(button, background, titleText, metaText, entry.Card));
        }
    }

    private void RebuildCardListItems()
    {
        if (_cardPileContentRoot == null)
        {
            return;
        }

        for (int index = _cardPileContentRoot.childCount - 1; index >= 0; index--)
        {
            Transform child = _cardPileContentRoot.GetChild(index);
            child.SetParent(null, false);
            Destroy(child.gameObject);
        }

        _cardItemViews.Clear();
        BuildCardListItems();
        _focusedCardIndex = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_cardPileContentRoot);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_cardDeskRoot);
    }

    private void CreateStatRow(RectTransform parent, EChildStatusType statType, string label, Color fillColor)
    {
        RectTransform row = GetOrCreateRect($"{statType}Row", parent);
        AddLayoutElement(row.gameObject, 72f, 0f, 1f);

        HorizontalLayoutGroup rowLayout = AddOrGetComponent<HorizontalLayoutGroup>(row.gameObject);
        rowLayout.spacing = 12;
        rowLayout.childForceExpandWidth = false;
        rowLayout.childForceExpandHeight = false;
        rowLayout.childControlHeight = true;
        rowLayout.childControlWidth = true;
        rowLayout.childAlignment = TextAnchor.MiddleLeft;

        TextMeshProUGUI labelText = CreateText("Label", row, 18, FontStyles.Bold, new Color(0.9f, 0.93f, 0.98f, 1f), label);
        AddLayoutElement(labelText.gameObject, 0f, 88f, 0f);

        Slider slider = CreateSlider(row, fillColor);
        AddLayoutElement(slider.gameObject, 0f, 0f, 1f);

        RectTransform pipRoot = CreateRect($"{statType}PipRoot", row);
        HorizontalLayoutGroup pipLayout = AddOrGetComponent<HorizontalLayoutGroup>(pipRoot.gameObject);
        pipLayout.spacing = 4;
        pipLayout.childAlignment = TextAnchor.MiddleCenter;
        pipLayout.childForceExpandWidth = false;
        pipLayout.childForceExpandHeight = false;
        pipLayout.childControlHeight = true;
        pipLayout.childControlWidth = true;
        AddLayoutElement(pipRoot.gameObject, 0f, 84f, 0f);

        TextMeshProUGUI valueText = CreateText("Value", row, 18, FontStyles.Bold, new Color(1f, 0.95f, 0.66f, 1f), RuntimeChildState.DefaultStatValue.ToString());
        valueText.alignment = TextAlignmentOptions.MidlineRight;
        AddLayoutElement(valueText.gameObject, 0f, 42f, 0f);

        _statSliders[statType] = slider;
        _statValueTexts[statType] = valueText;
        _statFillImages[statType] = slider.fillRect != null ? slider.fillRect.GetComponent<Image>() : null;

        List<Image> pipImages = new();
        for (int index = 0; index < RuntimeChildState.MaxStatValue; index++)
        {
            RectTransform pip = CreateRect($"{statType}Pip_{index}", pipRoot);
            AddLayoutElement(pip.gameObject, 18f, 12f, 0f);
            Image pipImage = AddOrGetImage(pip.gameObject, new Color(0.25f, 0.29f, 0.36f, 1f));
            pipImages.Add(pipImage);
        }

        _statPipImages[statType] = pipImages;
    }

    private void RefreshUI(bool forceRefreshDetail)
    {
        EnsureSelectionBuffer();

        if (!_uiBuilt)
        {
            return;
        }

        RefreshWeekHeader();
        _weekFileWindowPresenter?.SetWeek(GetOrderedCardEntries());
        _weekFileWindowPresenter?.Refresh();
        RefreshStatusTexts();
        RefreshSummaryText();
    }

    private void RefreshWeekHeader()
    {
        if (_weekDefinition == null)
        {
            _weekIndexText.text = "WEEK ?";
            _weekTitleText.text = "주차 정보가 없습니다";
            _weekSummaryText.text = "WeekDefinition 에셋을 연결해 주세요.";
            return;
        }

        _weekIndexText.text = $"WEEK {_weekDefinition.WeekIndex}";
        _weekTitleText.text = string.IsNullOrWhiteSpace(_weekDefinition.Title) ? "제목 없는 주차" : _weekDefinition.Title;
        _weekSummaryText.text = string.IsNullOrWhiteSpace(_weekDefinition.Summary)
            ? "이번 주에 네모에게 어떤 정보가 닿을지 설정해 주세요."
            : _weekDefinition.Summary;
    }

    private void RefreshPileHeader()
    {
        int cardCount = _cardItemViews.Count;
        _pileToggleLabel.text = _isPileExpanded
            ? $"이번 주 정보 묶음  {cardCount}건  접기"
            : $"이번 주 정보 묶음  {cardCount}건  펼치기";
    }

    private void RefreshCardList()
    {
        for (int index = 0; index < _cardItemViews.Count; index++)
        {
            CardListItemRuntime item = _cardItemViews[index];
            SO_CardInfoDefinition card = item.CardDefinition;

            string typeName = card != null && card.CardType != null ? card.CardType.DisplayName : "카드";
            string optionName = GetSelectedOptionLabel(card, index);

            item.TitleText.text = $"{index + 1}. {(card != null ? card.Title : "비어 있는 카드")}";
            item.MetaText.text = $"{typeName} · 선택: {optionName}";
            item.Background.color = index == _focusedCardIndex
                ? new Color(0.73f, 0.86f, 0.98f, 1f)
                : new Color(0.97f, 0.96f, 0.92f, 1f);
        }
    }

    private void RefreshCardDetail()
    {
        if (_cardItemViews.Count == 0)
        {
            _detailTypeText.text = "카드 없음";
            _detailTitleText.text = "이번 주에 배치된 카드가 없습니다.";
            _detailOriginalText.text = string.Empty;
            _detailPreviewText.text = string.Empty;
            UpdateOptionButtons(Array.Empty<CardOptionData>());
            return;
        }

        CardListItemRuntime focusedItem = _cardItemViews[Mathf.Clamp(_focusedCardIndex, 0, _cardItemViews.Count - 1)];
        SO_CardInfoDefinition card = focusedItem.CardDefinition;

        string typeName = card != null && card.CardType != null ? card.CardType.DisplayName : "카드";
        _detailTypeText.text = typeName;
        _detailTitleText.text = card != null ? card.Title : "알 수 없는 카드";
        _detailOriginalText.text = card != null ? card.OriginalText : string.Empty;

        CardOptionData[] options = card != null ? card.Options ?? Array.Empty<CardOptionData>() : Array.Empty<CardOptionData>();
        CardOptionData selectedOption = GetSelectedOption(card, _focusedCardIndex);
        _detailPreviewText.text = selectedOption != null && !string.IsNullOrWhiteSpace(selectedOption.PresentedText)
            ? selectedOption.PresentedText
            : "이 선택은 네모에게 직접적으로 닿는 정보가 거의 없어요.";

        UpdateOptionButtons(options);

        _detailSequence?.Kill();
        _detailPanelImage.color = new Color(0.82f, 0.9f, 1f, 1f);
        _detailSequence = DOTween.Sequence().SetUpdate(true);
        _detailSequence.Append(_detailPanelImage.DOColor(new Color(1f, 0.99f, 0.95f, 1f), 0.35f));
    }

    private void UpdateOptionButtons(IReadOnlyList<CardOptionData> options)
    {
        while (_cardOptionViews.Count < options.Count)
        {
            RectTransform buttonRoot = GetOrCreateRect($"OptionButton_{_cardOptionViews.Count}", _cardOptionButtonRoot);
            Image background = AddOrGetImage(buttonRoot.gameObject, new Color(0.32f, 0.36f, 0.44f, 1f));
            Button button = AddOrGetComponent<Button>(buttonRoot.gameObject);
            button.targetGraphic = background;
            AddLayoutElement(buttonRoot.gameObject, 56f, 0f, 1f);

            TextMeshProUGUI labelText = CreateText("Label", GetOrCreateStretchRect("LabelRoot", buttonRoot), 18, FontStyles.Bold, Color.white, string.Empty);
            labelText.alignment = TextAlignmentOptions.Center;

            _cardOptionViews.Add(new CardOptionRuntime(button, background, labelText, buttonRoot));
        }

        for (int index = 0; index < _cardOptionViews.Count; index++)
        {
            bool shouldBeVisible = index < options.Count;
            CardOptionRuntime optionRuntime = _cardOptionViews[index];
            optionRuntime.Root.gameObject.SetActive(shouldBeVisible);

            if (!shouldBeVisible)
            {
                continue;
            }

            CardOptionData option = options[index];
            optionRuntime.LabelText.text = option.Label;
            optionRuntime.Button.onClick.RemoveAllListeners();

            int optionIndex = index;
            optionRuntime.Button.onClick.AddListener(() => SelectOption(optionIndex));

            bool isSelected = _selectedOptionIndices.Length > _focusedCardIndex &&
                              _selectedOptionIndices[_focusedCardIndex] == optionIndex;

            optionRuntime.Background.color = isSelected
                ? new Color(0.21f, 0.52f, 0.77f, 1f)
                : new Color(0.3f, 0.33f, 0.39f, 1f);
        }
    }

    private void RefreshStatusTexts()
    {
        foreach (EChildStatusType statType in Enum.GetValues(typeof(EChildStatusType)))
        {
            int statValue = _childState.GetStat(statType);

            if (_statSliders.TryGetValue(statType, out Slider slider))
            {
                slider.value = statValue;
            }

            if (_statValueTexts.TryGetValue(statType, out TextMeshProUGUI valueText))
            {
                valueText.text = statValue.ToString();
            }

            RefreshStatPips(statType, statValue);
        }

        _statusMessageText.text = _statusMessage;

        string stateHint = _lastWeekResult == null
            ? "네모는 아직 이번 주의 결과를 기다리고 있어요."
            : BuildStateHintText();
        _nemoStateHintText.text = stateHint;
    }

    private void RefreshSummaryText()
    {
        if (_lastWeekResult == null)
        {
            _weekSummaryReportText.text = "아직 주차를 진행하지 않았습니다.\n카드 묶음을 펼쳐 이번 주에 들려줄 정보와 차단할 정보를 정해 주세요.";
            return;
        }

        _weekSummaryReportText.text = BuildResultSummaryText(_lastWeekResult);
    }

    private void ToggleCardPile()
    {
        SetPileExpanded(!_isPileExpanded, true);
    }

    private void SetPileExpanded(bool expanded, bool animate)
    {
        _isPileExpanded = expanded;
        RefreshPileHeader();

        float targetHeight = expanded ? GetExpandedPileHeight() : CollapsedPileHeight;
        float targetAlpha = expanded ? 1f : 0f;

        _pileSequence?.Kill();

        if (!animate)
        {
            _cardPileLayoutElement.preferredHeight = targetHeight;
            _cardPileCanvasGroup.alpha = targetAlpha;
            _cardPileCanvasGroup.interactable = expanded;
            _cardPileCanvasGroup.blocksRaycasts = expanded;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_cardDeskRoot);
            return;
        }

        _cardPileCanvasGroup.interactable = expanded;
        _cardPileCanvasGroup.blocksRaycasts = expanded;

        _pileSequence = DOTween.Sequence().SetUpdate(true);
        _pileSequence.Append(
            DOTween.To(
                () => _cardPileLayoutElement.preferredHeight,
                value =>
                {
                    _cardPileLayoutElement.preferredHeight = value;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_cardDeskRoot);
                },
                targetHeight,
                0.28f).SetEase(Ease.OutCubic));
        _pileSequence.Join(_cardPileCanvasGroup.DOFade(targetAlpha, 0.2f));
    }

    private float GetExpandedPileHeight()
    {
        int cardCount = Mathf.Max(1, _cardItemViews.Count);
        return Mathf.Clamp(20f + (cardCount * 86f), 180f, ExpandedPileHeight);
    }

    private void FocusCard(int cardIndex)
    {
        _focusedCardIndex = Mathf.Clamp(cardIndex, 0, Mathf.Max(0, _cardItemViews.Count - 1));
        RefreshCardList();
        RefreshCardDetail();
    }

    private void SelectOption(int optionIndex)
    {
        if (_focusedCardIndex < 0 || _focusedCardIndex >= _selectedOptionIndices.Length)
        {
            return;
        }

        _selectedOptionIndices[_focusedCardIndex] = optionIndex;
        RefreshCardList();
        RefreshCardDetail();
        _statusMessage = "카드 처리 방식이 업데이트되었습니다.";
        _statusMessageText.text = _statusMessage;
    }

    private void HandleResetSelections()
    {
        ResetSelections();
        _lastWeekResult = null;
        _statusMessage = "카드 선택을 기본값으로 되돌렸습니다.";
        RefreshUI(false);
    }

    private void HandleResetChildState()
    {
        _childState = new RuntimeChildState();
        _lastWeekResult = null;
        _endingReached = false;
        _statusMessage = "네모의 상태를 초기값으로 되돌렸습니다.";
        RefreshUI(false);
        PresentDefaultNemoState();
    }

    private void RunCurrentWeek()
    {
        if (_endingReached)
        {
            _statusMessage = "이미 마지막 엔딩까지 도달했습니다. 네모를 리셋하거나 씬을 다시 시작해 주세요.";
            RefreshStatusTexts();
            return;
        }

        if (_weekDefinition == null)
        {
            _statusMessage = "WeekDefinition 이 연결되지 않았습니다.";
            RefreshStatusTexts();
            return;
        }

        try
        {
            Dictionary<EChildStatusType, int> previousStats = CaptureCurrentStats();
            _childState.ClearReactionLogs();

            RuntimeWeekSelection[] selections = BuildSelections();
            SO_WeekDefinition executedWeek = _weekDefinition;
            _lastWeekResult = _weekRunner.RunWeek(_weekDefinition, _childState, selections);
            WeekFeedbackPresentation feedbackPresentation =
                WeekFeedbackResolver.Resolve(executedWeek, _lastWeekResult, _childState, previousStats);
            _pendingWeekEventPresentation = WeekNarrativeResolver.ResolveFixedEvent(executedWeek, _childState);
            _pendingPrivateDialoguePresentation = WeekNarrativeResolver.ResolvePrivateDialogue(executedWeek);
            _weekEventApplied = false;
            _privateDialogueChoiceApplied = false;
            _selectedDialogueChoicePresentation = default;

            AnimateStatSliders(previousStats);
            PresentNemoFeedback();
            Debug.Log(BuildResultDebugText(_lastWeekResult));

            bool isFinalWeek = IsCurrentWeekFinal();
            bool advanced = !isFinalWeek && TryAdvanceToNextWeek();
            _pendingEndingAfterNarrative = isFinalWeek;
            if (false && isFinalWeek)
            {
                EndingPresentation endingPresentation = EndingResolver.Resolve(_childState);
                _endingReached = true;
                _statusMessage = $"{executedWeek.Title}이 끝났습니다. 네모의 결말을 확인해 주세요.";
                RefreshUI(true);
                ShowEndingOverlay(endingPresentation);
                return;
            }

            _statusMessage = advanced
                ? $"WEEK {executedWeek.WeekIndex} 완료. 이제 WEEK {_weekDefinition.WeekIndex}를 준비합니다."
                : $"WEEK {executedWeek.WeekIndex} 결과가 네모에게 반영되었습니다.";

            RefreshUI(true);
            ShowWeekFeedbackOverlay(feedbackPresentation);
        }
        catch (Exception exception)
        {
            _statusMessage = $"주차 진행 중 오류가 발생했습니다: {exception.Message}";
            RefreshStatusTexts();
            Debug.LogError(exception);
        }
    }

    private RuntimeWeekSelection[] BuildSelections()
    {
        WeekCardEntryData[] orderedEntries = GetOrderedCardEntries();
        RuntimeWeekSelection[] selections = new RuntimeWeekSelection[orderedEntries.Length];

        for (int index = 0; index < orderedEntries.Length; index++)
        {
            selections[index] = new RuntimeWeekSelection(orderedEntries[index].Card, _selectedOptionIndices[index]);
        }

        return selections;
    }

    private Dictionary<EChildStatusType, int> CaptureCurrentStats()
    {
        Dictionary<EChildStatusType, int> snapshot = new();

        foreach (EChildStatusType statType in Enum.GetValues(typeof(EChildStatusType)))
        {
            snapshot[statType] = _childState.GetStat(statType);
        }

        return snapshot;
    }

    private void AnimateStatSliders(IReadOnlyDictionary<EChildStatusType, int> previousStats)
    {
        foreach (EChildStatusType statType in Enum.GetValues(typeof(EChildStatusType)))
        {
            if (!_statSliders.TryGetValue(statType, out Slider slider))
            {
                continue;
            }

            int previousValue = previousStats.TryGetValue(statType, out int storedValue)
                ? storedValue
                : RuntimeChildState.DefaultStatValue;
            int currentValue = _childState.GetStat(statType);

            slider.DOKill();
            slider.value = previousValue;
            DOTween.To(() => slider.value, value => slider.value = value, currentValue, 0.38f)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);

            if (_statValueTexts.TryGetValue(statType, out TextMeshProUGUI valueText))
            {
                valueText.text = currentValue.ToString();
            }

            RefreshStatPips(statType, currentValue);

            if (_statFillImages.TryGetValue(statType, out Image fillImage) && fillImage != null)
            {
                Color baseColor = fillImage.color;
                fillImage.DOKill();
                fillImage.color = Color.white;
                fillImage.DOColor(baseColor, 0.35f).SetEase(Ease.OutSine).SetUpdate(true);
            }
        }
    }

    private void RefreshStatPips(EChildStatusType statType, int statValue)
    {
        if (!_statPipImages.TryGetValue(statType, out List<Image> pipImages))
        {
            return;
        }

        Color activeColor = statType switch
        {
            EChildStatusType.Trust => new Color(0.47f, 0.84f, 0.67f, 1f),
            EChildStatusType.Curiosity => new Color(0.98f, 0.79f, 0.36f, 1f),
            EChildStatusType.Anxiety => new Color(0.94f, 0.53f, 0.49f, 1f),
            EChildStatusType.Obedience => new Color(0.56f, 0.71f, 0.96f, 1f),
            _ => Color.white,
        };

        for (int index = 0; index < pipImages.Count; index++)
        {
            Image pipImage = pipImages[index];
            bool isFilled = index < statValue;
            pipImage.color = isFilled
                ? activeColor
                : new Color(0.25f, 0.29f, 0.36f, 1f);
        }
    }

    private string BuildResultSummaryText(RuntimeWeekResult result)
    {
        StringBuilder builder = new();
        builder.AppendLine("이번 주 보고");

        foreach (RuntimeResolvedCardRecord resolvedCard in result.ResolvedCards)
        {
            string cardTitle = resolvedCard.CardDefinition != null ? resolvedCard.CardDefinition.Title : "알 수 없는 카드";
            string optionLabel = resolvedCard.SelectedOption != null ? resolvedCard.SelectedOption.Label : "선택 없음";
            builder.Append("• ");
            builder.Append(cardTitle);
            builder.Append(" → ");
            builder.AppendLine(optionLabel);
        }

        if (_childState.ReactionLogs.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("네모의 흔적");
            foreach (string reactionLog in _childState.ReactionLogs)
            {
                builder.Append("• ");
                builder.AppendLine(reactionLog);
            }
        }

        if (result.WeekLogs.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("주간 메모");
            foreach (string weekLog in result.WeekLogs)
            {
                builder.Append("• ");
                builder.AppendLine(weekLog);
            }
        }

        return builder.ToString().TrimEnd();
    }

    private void ShowWeekFeedbackOverlay(WeekFeedbackPresentation presentation)
    {
        if (_weekFeedbackOverlayRoot == null || _weekFeedbackOverlayCanvasGroup == null)
        {
            return;
        }

        if (_weekFeedbackCloseButtonLabelText != null)
        {
            _weekFeedbackCloseButtonLabelText.text = "닫기";
        }

        _weekFeedbackTitleText.text = presentation.Title;
        _weekFeedbackSummaryText.text = presentation.SummaryLine;
        _weekFeedbackStatDeltaText.text = presentation.StatDeltaLine;

        for (int index = _weekFeedbackEventRoot.childCount - 1; index >= 0; index--)
        {
            Transform child = _weekFeedbackEventRoot.GetChild(index);
            child.SetParent(null, false);
            Destroy(child.gameObject);
        }

        for (int index = 0; index < presentation.EventLines.Count; index++)
        {
            RectTransform item = CreateRect($"FeedbackEvent_{index}", _weekFeedbackEventRoot);
            AddOrGetImage(item.gameObject, new Color(0.19f, 0.23f, 0.31f, 1f));
            AddLayoutElement(item.gameObject, 72f, 0f, 0f);

            TextMeshProUGUI lineText = CreateText("EventText", item, 20, FontStyles.Normal, new Color(0.95f, 0.97f, 1f, 1f), presentation.EventLines[index]);
            lineText.margin = new Vector4(18f, 14f, 18f, 14f);
            lineText.textWrappingMode = TextWrappingModes.Normal;
            lineText.overflowMode = TextOverflowModes.Overflow;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_weekFeedbackEventRoot);
        _weekFeedbackOverlaySequence?.Kill();
        _weekFeedbackOverlayCanvasGroup.alpha = 0f;
        _weekFeedbackOverlayCanvasGroup.interactable = true;
        _weekFeedbackOverlayCanvasGroup.blocksRaycasts = true;
        _weekFeedbackOverlayRoot.localScale = new Vector3(0.98f, 0.98f, 1f);

        _weekFeedbackOverlaySequence = DOTween.Sequence().SetUpdate(true);
        _weekFeedbackOverlaySequence.Append(_weekFeedbackOverlayCanvasGroup.DOFade(1f, 0.18f));
        _weekFeedbackOverlaySequence.Join(_weekFeedbackOverlayRoot.DOScale(1f, 0.22f).SetEase(Ease.OutBack));
    }

    private void ShowEndingOverlay(EndingPresentation presentation)
    {
        if (_weekFeedbackOverlayRoot == null || _weekFeedbackOverlayCanvasGroup == null)
        {
            return;
        }

        if (_weekFeedbackCloseButtonLabelText != null)
        {
            _weekFeedbackCloseButtonLabelText.text = "엔딩 닫기";
        }

        _weekFeedbackTitleText.text = presentation.Title;
        _weekFeedbackSummaryText.text = $"{presentation.Summary}\n\n{presentation.ClosingLine}";
        _weekFeedbackStatDeltaText.text = $"{BuildEndingStatsText()}\n{presentation.ReputationLine}";

        for (int index = _weekFeedbackEventRoot.childCount - 1; index >= 0; index--)
        {
            Transform child = _weekFeedbackEventRoot.GetChild(index);
            child.SetParent(null, false);
            Destroy(child.gameObject);
        }

        for (int index = 0; index < presentation.DetailLines.Count; index++)
        {
            RectTransform item = CreateRect($"EndingLine_{index}", _weekFeedbackEventRoot);
            AddOrGetImage(item.gameObject, new Color(0.18f, 0.17f, 0.26f, 1f));
            AddLayoutElement(item.gameObject, 76f, 0f, 0f);

            TextMeshProUGUI lineText = CreateText("EndingText", item, 20, FontStyles.Normal, new Color(0.97f, 0.97f, 1f, 1f), presentation.DetailLines[index]);
            lineText.margin = new Vector4(18f, 14f, 18f, 14f);
            lineText.textWrappingMode = TextWrappingModes.Normal;
            lineText.overflowMode = TextOverflowModes.Overflow;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_weekFeedbackEventRoot);
        _weekFeedbackOverlaySequence?.Kill();
        _weekFeedbackOverlayCanvasGroup.alpha = 0f;
        _weekFeedbackOverlayCanvasGroup.interactable = true;
        _weekFeedbackOverlayCanvasGroup.blocksRaycasts = true;
        _weekFeedbackOverlayRoot.localScale = new Vector3(0.98f, 0.98f, 1f);

        _nemoActorView?.Present(new NemoFeedbackPresentation(presentation.VisualState, presentation.ClosingLine));

        _weekFeedbackOverlaySequence = DOTween.Sequence().SetUpdate(true);
        _weekFeedbackOverlaySequence.Append(_weekFeedbackOverlayCanvasGroup.DOFade(1f, 0.18f));
        _weekFeedbackOverlaySequence.Join(_weekFeedbackOverlayRoot.DOScale(1f, 0.22f).SetEase(Ease.OutBack));
    }

    private void HideWeekFeedbackOverlay()
    {
        if (_weekFeedbackOverlayCanvasGroup == null)
        {
            return;
        }

        _weekFeedbackOverlaySequence?.Kill();
        _weekFeedbackOverlaySequence = DOTween.Sequence().SetUpdate(true);
        _weekFeedbackOverlaySequence.Append(_weekFeedbackOverlayCanvasGroup.DOFade(0f, 0.18f));
        _weekFeedbackOverlaySequence.Join(_weekFeedbackOverlayRoot.DOScale(0.98f, 0.18f).SetEase(Ease.InSine));
        _weekFeedbackOverlaySequence.OnComplete(() =>
        {
            _weekFeedbackOverlayCanvasGroup.interactable = false;
            _weekFeedbackOverlayCanvasGroup.blocksRaycasts = false;
            _weekFeedbackOverlayRoot.localScale = Vector3.one;
            ContinuePostWeekNarrativeFlow();
        });
    }

    private void ShowWeekEventOverlay(WeekFixedEventPresentation presentation)
    {
        if (_weekEventOverlayRoot == null || _weekEventOverlayCanvasGroup == null || !presentation.HasContent)
        {
            ContinuePostWeekNarrativeFlow();
            return;
        }

        _weekEventTitleText.text = presentation.Title;
        _weekEventSituationText.text = presentation.SituationText;
        _weekEventReactionText.text = presentation.ReactionText;
        _weekEventStatDeltaText.text = presentation.StatDeltaLine;

        _weekEventOverlaySequence?.Kill();
        _weekEventOverlayCanvasGroup.alpha = 0f;
        _weekEventOverlayCanvasGroup.interactable = true;
        _weekEventOverlayCanvasGroup.blocksRaycasts = true;
        _weekEventOverlayRoot.localScale = new Vector3(0.98f, 0.98f, 1f);

        _weekEventOverlaySequence = DOTween.Sequence().SetUpdate(true);
        _weekEventOverlaySequence.Append(_weekEventOverlayCanvasGroup.DOFade(1f, 0.18f));
        _weekEventOverlaySequence.Join(_weekEventOverlayRoot.DOScale(1f, 0.22f).SetEase(Ease.OutBack));
    }

    private void HideWeekEventOverlay()
    {
        if (_weekEventOverlayCanvasGroup == null)
        {
            return;
        }

        _weekEventOverlaySequence?.Kill();
        _weekEventOverlaySequence = DOTween.Sequence().SetUpdate(true);
        _weekEventOverlaySequence.Append(_weekEventOverlayCanvasGroup.DOFade(0f, 0.18f));
        _weekEventOverlaySequence.Join(_weekEventOverlayRoot.DOScale(0.98f, 0.18f).SetEase(Ease.InSine));
        _weekEventOverlaySequence.OnComplete(() =>
        {
            _weekEventOverlayCanvasGroup.interactable = false;
            _weekEventOverlayCanvasGroup.blocksRaycasts = false;
            _weekEventOverlayRoot.localScale = Vector3.one;
            ContinuePostWeekNarrativeFlow();
        });
    }

    private void ShowPrivateDialogueOverlay(WeekPrivateDialoguePresentation presentation)
    {
        if (_privateDialogueOverlayRoot == null || _privateDialogueOverlayCanvasGroup == null || !presentation.HasContent)
        {
            ContinuePostWeekNarrativeFlow();
            return;
        }

        _privateDialogueTitleText.text = presentation.Title;
        _privateDialogueOpeningText.text = presentation.OpeningLine;
        _privateDialogueResponseText.text = "네모의 말을 듣고, 이번에는 어떤 태도로 답할지 정해 주세요.";
        _privateDialogueContinueButton.gameObject.SetActive(false);
        _privateDialogueChoiceApplied = false;
        _selectedDialogueChoicePresentation = default;

        for (int index = 0; index < _privateDialogueChoiceButtons.Count; index++)
        {
            bool isVisible = index < presentation.Choices.Count;
            _privateDialogueChoiceButtons[index].gameObject.SetActive(isVisible);
            if (isVisible)
            {
                _privateDialogueChoiceButtons[index].interactable = true;
                _privateDialogueChoiceLabels[index].text = presentation.Choices[index].Label;
            }
        }

        _privateDialogueOverlaySequence?.Kill();
        _privateDialogueOverlayCanvasGroup.alpha = 0f;
        _privateDialogueOverlayCanvasGroup.interactable = true;
        _privateDialogueOverlayCanvasGroup.blocksRaycasts = true;
        _privateDialogueOverlayRoot.localScale = new Vector3(0.98f, 0.98f, 1f);

        _privateDialogueOverlaySequence = DOTween.Sequence().SetUpdate(true);
        _privateDialogueOverlaySequence.Append(_privateDialogueOverlayCanvasGroup.DOFade(1f, 0.18f));
        _privateDialogueOverlaySequence.Join(_privateDialogueOverlayRoot.DOScale(1f, 0.22f).SetEase(Ease.OutBack));
    }

    private void HidePrivateDialogueOverlay()
    {
        if (_privateDialogueOverlayCanvasGroup == null)
        {
            return;
        }

        _privateDialogueOverlaySequence?.Kill();
        _privateDialogueOverlaySequence = DOTween.Sequence().SetUpdate(true);
        _privateDialogueOverlaySequence.Append(_privateDialogueOverlayCanvasGroup.DOFade(0f, 0.18f));
        _privateDialogueOverlaySequence.Join(_privateDialogueOverlayRoot.DOScale(0.98f, 0.18f).SetEase(Ease.InSine));
        _privateDialogueOverlaySequence.OnComplete(() =>
        {
            _privateDialogueOverlayCanvasGroup.interactable = false;
            _privateDialogueOverlayCanvasGroup.blocksRaycasts = false;
            _privateDialogueOverlayRoot.localScale = Vector3.one;
            ContinuePostWeekNarrativeFlow();
        });
    }

    private void ContinuePostWeekNarrativeFlow()
    {
        if (_pendingWeekEventPresentation.HasContent && !_weekEventApplied)
        {
            ShowWeekEventOverlay(_pendingWeekEventPresentation);
            return;
        }

        if (_pendingPrivateDialoguePresentation.HasContent && !_privateDialogueChoiceApplied)
        {
            ShowPrivateDialogueOverlay(_pendingPrivateDialoguePresentation);
            return;
        }

        if (_pendingEndingAfterNarrative)
        {
            _pendingEndingAfterNarrative = false;
            EndingPresentation endingPresentation = EndingResolver.Resolve(_childState);
            _endingReached = true;
            ShowEndingOverlay(endingPresentation);
            return;
        }

        _pendingWeekEventPresentation = WeekFixedEventPresentation.Empty;
        _pendingPrivateDialoguePresentation = WeekPrivateDialoguePresentation.Empty;
    }

    private void HandleWeekEventContinue()
    {
        if (!_weekEventApplied && _pendingWeekEventPresentation.HasContent)
        {
            Dictionary<EChildStatusType, int> previousStats = CaptureCurrentStats();
            WeekNarrativeResolver.ApplyDeltas(_childState, _pendingWeekEventPresentation.StatDeltas);
            _weekEventApplied = true;
            AnimateStatSliders(previousStats);
            RefreshStatusTexts();
            _statusMessage = "주차 확정 사건의 여운이 네모에게 남았습니다.";
            _nemoActorView?.Present(new NemoFeedbackPresentation(
                WeekNarrativeResolver.GetVisualStateForStat(_pendingWeekEventPresentation.DominantStat),
                _pendingWeekEventPresentation.ReactionText));
        }

        HideWeekEventOverlay();
    }

    private void HandlePrivateDialogueChoice(int choiceIndex)
    {
        if (_privateDialogueChoiceApplied || !_pendingPrivateDialoguePresentation.HasContent)
        {
            return;
        }

        if (choiceIndex < 0 || choiceIndex >= _pendingPrivateDialoguePresentation.Choices.Count)
        {
            return;
        }

        _selectedDialogueChoicePresentation = _pendingPrivateDialoguePresentation.Choices[choiceIndex];
        _privateDialogueChoiceApplied = true;

        Dictionary<EChildStatusType, int> previousStats = CaptureCurrentStats();
        WeekNarrativeResolver.ApplyDeltas(_childState, _selectedDialogueChoicePresentation.SourceData.StatDeltas);
        AnimateStatSliders(previousStats);
        RefreshStatusTexts();
        _statusMessage = "네모와의 대화가 다음 주의 분위기를 남겼습니다.";

        _privateDialogueResponseText.text =
            $"{_selectedDialogueChoicePresentation.ResponseLine}\n\n{_selectedDialogueChoicePresentation.StatDeltaLine}";

        for (int index = 0; index < _privateDialogueChoiceButtons.Count; index++)
        {
            _privateDialogueChoiceButtons[index].interactable = false;
        }

        _privateDialogueContinueButton.gameObject.SetActive(true);
        _nemoActorView?.Present(new NemoFeedbackPresentation(
            WeekNarrativeResolver.GetVisualStateForStat(WeekNarrativeResolver.GetDominantStat(_childState)),
            _selectedDialogueChoicePresentation.ResponseLine));
    }

    private void FinishPrivateDialogue()
    {
        HidePrivateDialogueOverlay();
    }

    private string BuildStateHintText()
    {
        RuntimeResolvedCardRecord lastCard = _lastWeekResult != null && _lastWeekResult.ResolvedCards.Count > 0
            ? _lastWeekResult.ResolvedCards[_lastWeekResult.ResolvedCards.Count - 1]
            : null;

        NemoFeedbackPresentation presentation = NemoFeedbackResolver.Resolve(_childState, lastCard);
        string dominantStatLabel = GetDominantStatLabel();
        return $"{dominantStatLabel} 기색이 강해졌어요. {presentation.DialogueLine}";
    }

    private string GetDominantStatLabel()
    {
        EChildStatusType dominantStat = Enum.GetValues(typeof(EChildStatusType))
            .Cast<EChildStatusType>()
            .OrderByDescending(statType => _childState.GetStat(statType))
            .First();

        return dominantStat switch
        {
            EChildStatusType.Trust => "신뢰",
            EChildStatusType.Curiosity => "호기심",
            EChildStatusType.Anxiety => "불안",
            EChildStatusType.Obedience => "순응",
            _ => "중립",
        };
    }

    private string BuildEndingStatsText()
    {
        return
            $"신뢰 {_childState.GetStat(EChildStatusType.Trust)}   " +
            $"호기심 {_childState.GetStat(EChildStatusType.Curiosity)}   " +
            $"불안 {_childState.GetStat(EChildStatusType.Anxiety)}   " +
            $"순응 {_childState.GetStat(EChildStatusType.Obedience)}";
    }

    private string BuildResultDebugText(RuntimeWeekResult result)
    {
        StringBuilder builder = new();
        builder.AppendLine($"[WeekRunner] WEEK {result.WeekDefinition.WeekIndex} - {result.WeekDefinition.Title}");

        foreach (RuntimeResolvedCardRecord resolvedCard in result.ResolvedCards)
        {
            string cardTitle = resolvedCard.CardDefinition != null ? resolvedCard.CardDefinition.Title : "Unknown";
            string optionLabel = resolvedCard.SelectedOption != null ? resolvedCard.SelectedOption.Label : "Unknown";
            builder.AppendLine($"Card: {cardTitle} / Option: {optionLabel}");

            if (!string.IsNullOrWhiteSpace(resolvedCard.SelectedOption?.PresentedText))
            {
                builder.AppendLine($"Presented: {resolvedCard.SelectedOption.PresentedText}");
            }
        }

        builder.AppendLine(
            $"Stats -> Trust:{_childState.GetStat(EChildStatusType.Trust)}, " +
            $"Curiosity:{_childState.GetStat(EChildStatusType.Curiosity)}, " +
            $"Anxiety:{_childState.GetStat(EChildStatusType.Anxiety)}, " +
            $"Obedience:{_childState.GetStat(EChildStatusType.Obedience)}");

        if (_childState.Flags.Count > 0)
        {
            builder.AppendLine($"Flags -> {string.Join(", ", _childState.Flags)}");
        }

        foreach (string reactionLog in _childState.ReactionLogs)
        {
            builder.AppendLine($"Reaction -> {reactionLog}");
        }

        foreach (string weekLog in result.WeekLogs)
        {
            builder.AppendLine($"WeekLog -> {weekLog}");
        }

        return builder.ToString();
    }

    private bool IsCurrentWeekFinal()
    {
        if (_weekDefinitions == null || _weekDefinitions.Length == 0)
        {
            return true;
        }

        return _currentWeekSequenceIndex >= _weekDefinitions.Length - 1;
    }

    private void InitializeWeekSequence()
    {
        if (_weekDefinitions == null)
        {
            _weekDefinitions = Array.Empty<SO_WeekDefinition>();
        }

        SO_WeekDefinition[] validWeeks = _weekDefinitions
            .Where(week => week != null)
            .OrderBy(week => week.WeekIndex)
            .ToArray();

        if (validWeeks.Length == 0)
        {
            if (_weekDefinition == null)
            {
                _currentWeekSequenceIndex = 0;
                return;
            }

            _weekDefinitions = new[] { _weekDefinition };
            _currentWeekSequenceIndex = 0;
            return;
        }

        _weekDefinitions = validWeeks;

        int matchedIndex = Array.IndexOf(_weekDefinitions, _weekDefinition);
        _currentWeekSequenceIndex = matchedIndex >= 0 ? matchedIndex : 0;
        _weekDefinition = _weekDefinitions[_currentWeekSequenceIndex];
    }

    private bool TryAdvanceToNextWeek()
    {
        if (_weekDefinitions == null || _weekDefinitions.Length == 0)
        {
            return false;
        }

        if (_currentWeekSequenceIndex >= _weekDefinitions.Length - 1)
        {
            return false;
        }

        _currentWeekSequenceIndex++;
        _weekDefinition = _weekDefinitions[_currentWeekSequenceIndex];
        EnsureSelectionBuffer();
        ResetSelections();
        _weekFileWindowPresenter?.SetWeek(GetOrderedCardEntries());
        return true;
    }

    private void EnsurePanelRoots()
    {
        _dayPanelRoot = FindPanel("DayPanel");
        _cardPanelRoot = FindPanel("CardPanel");
        _buttonPanelRoot = FindPanel("ButtonPanel");
        _characterPanelRoot = FindPanel("CharacterPanel");
    }

    private void EnsureSelectionBuffer()
    {
        int targetCount = _weekDefinition != null && _weekDefinition.CardEntries != null
            ? _weekDefinition.CardEntries.Count(entry => entry != null)
            : 0;

        bool needsRebuild =
            _selectedOptionIndices == null ||
            _cachedWeekDefinition != _weekDefinition ||
            _cachedCardCount != targetCount ||
            _selectedOptionIndices.Length != targetCount;

        if (!needsRebuild)
        {
            return;
        }

        int[] previousSelections = _selectedOptionIndices ?? Array.Empty<int>();
        _selectedOptionIndices = new int[targetCount];

        for (int index = 0; index < _selectedOptionIndices.Length; index++)
        {
            _selectedOptionIndices[index] = index < previousSelections.Length
                ? Mathf.Max(0, previousSelections[index])
                : 0;
        }

        _cachedWeekDefinition = _weekDefinition;
        _cachedCardCount = targetCount;
    }

    private void ResetSelections()
    {
        for (int index = 0; index < _selectedOptionIndices.Length; index++)
        {
            _selectedOptionIndices[index] = 0;
        }
    }

    private WeekCardEntryData[] GetOrderedCardEntries()
    {
        if (_weekDefinition == null || _weekDefinition.CardEntries == null)
        {
            return Array.Empty<WeekCardEntryData>();
        }

        return _weekDefinition.CardEntries
            .Where(entry => entry != null)
            .OrderBy(entry => entry.DisplayOrder)
            .ToArray();
    }

    private int GetSelectionIndexForCard(SO_CardInfoDefinition cardDefinition)
    {
        if (cardDefinition == null)
        {
            return 0;
        }

        WeekCardEntryData[] entries = GetOrderedCardEntries();
        for (int index = 0; index < entries.Length; index++)
        {
            if (entries[index]?.Card == cardDefinition)
            {
                return index < _selectedOptionIndices.Length ? _selectedOptionIndices[index] : 0;
            }
        }

        return 0;
    }

    private void SetSelectionIndexForCard(SO_CardInfoDefinition cardDefinition, int optionIndex)
    {
        if (cardDefinition == null)
        {
            return;
        }

        WeekCardEntryData[] entries = GetOrderedCardEntries();
        for (int index = 0; index < entries.Length; index++)
        {
            if (entries[index]?.Card != cardDefinition)
            {
                continue;
            }

            CardOptionData[] options = cardDefinition.Options ?? Array.Empty<CardOptionData>();
            int clampedOptionIndex = options.Length == 0
                ? 0
                : Mathf.Clamp(optionIndex, 0, options.Length - 1);

            if (index < _selectedOptionIndices.Length)
            {
                _selectedOptionIndices[index] = clampedOptionIndex;
                _statusMessage = "카드 처리 방식이 업데이트되었습니다.";
                RefreshStatusTexts();
            }

            return;
        }
    }

    private CardOptionData GetSelectedOption(SO_CardInfoDefinition cardDefinition, int cardIndex)
    {
        if (cardDefinition == null)
        {
            return null;
        }

        CardOptionData[] options = cardDefinition.Options;
        if (options == null || options.Length == 0)
        {
            return null;
        }

        int optionIndex = cardIndex >= 0 && cardIndex < _selectedOptionIndices.Length
            ? Mathf.Clamp(_selectedOptionIndices[cardIndex], 0, options.Length - 1)
            : 0;

        return options[optionIndex];
    }

    private string GetSelectedOptionLabel(SO_CardInfoDefinition cardDefinition, int cardIndex)
    {
        CardOptionData option = GetSelectedOption(cardDefinition, cardIndex);
        return option != null && !string.IsNullOrWhiteSpace(option.Label) ? option.Label : "선택 없음";
    }

    private void EnsureNemoActorView()
    {
        if (_nemoActorView == null)
        {
            _nemoActorView = GetComponent<NemoActorView>();
            if (_nemoActorView == null)
            {
                _nemoActorView = gameObject.AddComponent<NemoActorView>();
            }
        }

        _nemoActorView.SetDialogueFont(GetUIFont());

        if (_nemoStageRoot != null)
        {
            _nemoActorView.SetStageRoot(_nemoStageRoot);
        }
    }

    private void PresentNemoFeedback()
    {
        if (_nemoActorView == null)
        {
            return;
        }

        RuntimeResolvedCardRecord lastResolvedCard = _lastWeekResult != null && _lastWeekResult.ResolvedCards.Count > 0
            ? _lastWeekResult.ResolvedCards[_lastWeekResult.ResolvedCards.Count - 1]
            : null;

        NemoFeedbackPresentation presentation = NemoFeedbackResolver.Resolve(_childState, lastResolvedCard);
        _nemoActorView.Present(presentation);
    }

    private void PresentDefaultNemoState()
    {
        if (_nemoActorView == null)
        {
            return;
        }

        _nemoActorView.Present(new NemoFeedbackPresentation(ENemoVisualState.Neutral, "이번 주엔 어떤 이야기가 나한테 닿을까?"));
    }

    private TMP_FontAsset GetUIFont()
    {
        return _nemoDialogueFont != null ? _nemoDialogueFont : TMP_Settings.defaultFontAsset;
    }

    private static RectTransform FindPanel(string panelName)
    {
        GameObject panelObject = GameObject.Find(panelName);
        if (panelObject != null && panelObject.TryGetComponent(out RectTransform panelRect))
        {
            return panelRect;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        return canvas != null ? canvas.GetComponent<RectTransform>() : null;
    }

    private static RectTransform GetOrCreateStretchRect(string objectName, RectTransform parent)
    {
        RectTransform rect = GetOrCreateRect(objectName, parent);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.localScale = Vector3.one;
        return rect;
    }

    private static RectTransform CreateUIRect(
        string objectName,
        RectTransform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 sizeDelta)
    {
        GameObject gameObject = new(objectName, typeof(RectTransform));
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.localScale = Vector3.one;
        return rectTransform;
    }

    private static RectTransform CreateRect(string objectName, RectTransform parent)
    {
        GameObject gameObject = new(objectName, typeof(RectTransform));
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        return rectTransform;
    }

    private static RectTransform GetOrCreateRect(string objectName, RectTransform parent)
    {
        Transform existingChild = parent.Find(objectName);
        if (existingChild != null && existingChild.TryGetComponent(out RectTransform existingRect))
        {
            return existingRect;
        }

        GameObject gameObject = new(objectName, typeof(RectTransform));
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
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

    private static Image AddOrGetImage(GameObject gameObject, Color color)
    {
        Image image = AddOrGetComponent<Image>(gameObject);
        image.color = color;
        image.type = Image.Type.Sliced;
        return image;
    }

    private TextMeshProUGUI CreateText(
        string objectName,
        RectTransform parent,
        float fontSize,
        FontStyles fontStyle,
        Color color,
        string text)
    {
        RectTransform rect = GetOrCreateStretchRect(objectName, parent);
        TextMeshProUGUI textComponent = AddOrGetComponent<TextMeshProUGUI>(rect.gameObject);
        textComponent.font = GetUIFont();
        textComponent.fontSize = fontSize;
        textComponent.fontStyle = fontStyle;
        textComponent.color = color;
        textComponent.text = text;
        textComponent.textWrappingMode = TextWrappingModes.Normal;
        textComponent.overflowMode = TextOverflowModes.Overflow;
        textComponent.horizontalAlignment = HorizontalAlignmentOptions.Left;
        textComponent.verticalAlignment = VerticalAlignmentOptions.Middle;
        return textComponent;
    }

    private Button CreateButton(
        string objectName,
        RectTransform parent,
        out TextMeshProUGUI labelText,
        out Image backgroundImage,
        Color backgroundColor,
        float fontSize,
        FontStyles fontStyle,
        Color textColor,
        string label = null)
    {
        RectTransform root = GetOrCreateRect(objectName, parent);
        backgroundImage = AddOrGetImage(root.gameObject, backgroundColor);

        Button button = AddOrGetComponent<Button>(root.gameObject);
        button.targetGraphic = backgroundImage;

        RectTransform labelRoot = GetOrCreateStretchRect("LabelRoot", root);
        labelText = CreateText("LabelText", labelRoot, fontSize, fontStyle, textColor, label ?? objectName);
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.margin = new Vector4(16f, 10f, 16f, 10f);
        return button;
    }

    private static LayoutElement AddLayoutElement(GameObject gameObject, float preferredHeight, float preferredWidth, float flexibleHeight)
    {
        LayoutElement element = AddOrGetComponent<LayoutElement>(gameObject);
        element.preferredHeight = preferredHeight;
        element.preferredWidth = preferredWidth;
        element.flexibleHeight = flexibleHeight;
        return element;
    }

    private Slider CreateSlider(RectTransform parent, Color fillColor)
    {
        RectTransform sliderRoot = GetOrCreateRect("SliderRoot", parent);
        Image backgroundImage = AddOrGetImage(sliderRoot.gameObject, new Color(0.22f, 0.26f, 0.33f, 1f));

        Slider slider = AddOrGetComponent<Slider>(sliderRoot.gameObject);
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = RuntimeChildState.MinStatValue;
        slider.maxValue = RuntimeChildState.MaxStatValue;
        slider.wholeNumbers = false;
        slider.interactable = false;
        slider.targetGraphic = backgroundImage;

        RectTransform fillArea = GetOrCreateStretchRect("FillArea", sliderRoot);
        fillArea.offsetMin = new Vector2(4f, 4f);
        fillArea.offsetMax = new Vector2(-4f, -4f);

        RectTransform fillRect = GetOrCreateStretchRect("Fill", fillArea);
        AddOrGetImage(fillRect.gameObject, fillColor);

        slider.fillRect = fillRect;
        slider.handleRect = null;
        slider.value = RuntimeChildState.DefaultStatValue;

        return slider;
    }

    private sealed class CardListItemRuntime
    {
        public CardListItemRuntime(Button button, Image background, TextMeshProUGUI titleText, TextMeshProUGUI metaText, SO_CardInfoDefinition cardDefinition)
        {
            Button = button;
            Background = background;
            TitleText = titleText;
            MetaText = metaText;
            CardDefinition = cardDefinition;
        }

        public Button Button { get; }
        public Image Background { get; }
        public TextMeshProUGUI TitleText { get; }
        public TextMeshProUGUI MetaText { get; }
        public SO_CardInfoDefinition CardDefinition { get; }
    }

    private sealed class CardOptionRuntime
    {
        public CardOptionRuntime(Button button, Image background, TextMeshProUGUI labelText, RectTransform root)
        {
            Button = button;
            Background = background;
            LabelText = labelText;
            Root = root;
        }

        public Button Button { get; }
        public Image Background { get; }
        public TextMeshProUGUI LabelText { get; }
        public RectTransform Root { get; }
    }
}
