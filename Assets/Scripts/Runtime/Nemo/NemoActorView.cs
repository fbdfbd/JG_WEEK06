using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NemoActorView : MonoBehaviour
{
    private const float DialogueVisibleDuration = 3.2f;

    private RectTransform _stageRoot;
    private RectTransform _assignedStageRoot;
    private RectTransform _nemoRoot;
    private RawImage _bodyImage;
    private RectTransform _leftEye;
    private RectTransform _rightEye;
    private RectTransform _mouth;
    private RectTransform _bubbleRoot;
    private RawImage _bubbleImage;
    private TextMeshProUGUI _dialogueText;
    private CanvasGroup _bubbleCanvasGroup;
    private TMP_FontAsset _dialogueFontAsset;

    private Sequence _idleSequence;
    private Sequence _dialogueSequence;
    private Vector2 _baseActorPosition;
    private Vector2 _leftEyeBasePosition;
    private Vector2 _rightEyeBasePosition;

    public void SetDialogueFont(TMP_FontAsset fontAsset)
    {
        _dialogueFontAsset = fontAsset;

        if (_dialogueText != null && _dialogueFontAsset != null)
        {
            _dialogueText.font = _dialogueFontAsset;
        }
    }

    public void SetStageRoot(RectTransform stageRoot)
    {
        if (_assignedStageRoot == stageRoot)
        {
            return;
        }

        _assignedStageRoot = stageRoot;
        _stageRoot = stageRoot;
        DisposeRuntimeHierarchy();
    }

    public void Present(NemoFeedbackPresentation presentation)
    {
        EnsureHierarchy();
        ApplyVisualState(presentation.VisualState);
        ShowDialogue(presentation.DialogueLine);
    }

    public void ResetPresentation()
    {
        EnsureHierarchy();
        ApplyVisualState(ENemoVisualState.Neutral);
        ShowDialogue("이번 주엔 어떤 이야기가 나한테 닿을까?");
    }

    private void EnsureHierarchy()
    {
        if (_stageRoot == null)
        {
            _stageRoot = _assignedStageRoot != null ? _assignedStageRoot : FindStageRoot();
        }

        if (_stageRoot == null || _nemoRoot != null)
        {
            return;
        }

        _nemoRoot = CreateUIRect("NemoRoot", _stageRoot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), new Vector2(180f, 180f));
        _baseActorPosition = _nemoRoot.anchoredPosition;

        RectTransform bodyRect = CreateUIRect("Body", _nemoRoot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(150f, 150f));
        _bodyImage = bodyRect.gameObject.AddComponent<RawImage>();
        _bodyImage.texture = Texture2D.whiteTexture;
        _bodyImage.color = new Color(0.95f, 0.91f, 0.68f, 1f);

        _leftEye = CreateUIRect("LeftEye", bodyRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-28f, 18f), new Vector2(18f, 18f));
        RawImage leftEyeImage = _leftEye.gameObject.AddComponent<RawImage>();
        leftEyeImage.texture = Texture2D.whiteTexture;
        leftEyeImage.color = new Color(0.14f, 0.14f, 0.18f, 1f);

        _rightEye = CreateUIRect("RightEye", bodyRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(28f, 18f), new Vector2(18f, 18f));
        RawImage rightEyeImage = _rightEye.gameObject.AddComponent<RawImage>();
        rightEyeImage.texture = Texture2D.whiteTexture;
        rightEyeImage.color = new Color(0.14f, 0.14f, 0.18f, 1f);

        _mouth = CreateUIRect("Mouth", bodyRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -24f), new Vector2(46f, 8f));
        RawImage mouthImage = _mouth.gameObject.AddComponent<RawImage>();
        mouthImage.texture = Texture2D.whiteTexture;
        mouthImage.color = new Color(0.18f, 0.18f, 0.2f, 0.9f);

        _leftEyeBasePosition = _leftEye.anchoredPosition;
        _rightEyeBasePosition = _rightEye.anchoredPosition;

        _bubbleRoot = CreateUIRect("DialogueBubble", _stageRoot, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -100f), new Vector2(360f, 120f));
        _bubbleImage = _bubbleRoot.gameObject.AddComponent<RawImage>();
        _bubbleImage.texture = Texture2D.whiteTexture;
        _bubbleImage.color = new Color(0.08f, 0.11f, 0.16f, 0.88f);
        _bubbleCanvasGroup = _bubbleRoot.gameObject.AddComponent<CanvasGroup>();
        _bubbleCanvasGroup.alpha = 0f;

        RectTransform textRect = CreateUIRect("DialogueText", _bubbleRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        textRect.offsetMin = new Vector2(18f, 14f);
        textRect.offsetMax = new Vector2(-18f, -14f);
        _dialogueText = textRect.gameObject.AddComponent<TextMeshProUGUI>();
        _dialogueText.font = _dialogueFontAsset;
        _dialogueText.fontSize = 26;
        _dialogueText.alignment = TextAlignmentOptions.Center;
        _dialogueText.textWrappingMode = TextWrappingModes.Normal;
        _dialogueText.overflowMode = TextOverflowModes.Overflow;
        _dialogueText.color = new Color(0.97f, 0.97f, 0.98f, 1f);
    }

    private void DisposeRuntimeHierarchy()
    {
        _idleSequence?.Kill();
        _dialogueSequence?.Kill();

        if (_nemoRoot != null)
        {
            Destroy(_nemoRoot.gameObject);
        }

        if (_bubbleRoot != null)
        {
            Destroy(_bubbleRoot.gameObject);
        }

        _nemoRoot = null;
        _bodyImage = null;
        _leftEye = null;
        _rightEye = null;
        _mouth = null;
        _bubbleRoot = null;
        _bubbleImage = null;
        _dialogueText = null;
        _bubbleCanvasGroup = null;
    }

    private void ApplyVisualState(ENemoVisualState visualState)
    {
        if (_nemoRoot == null)
        {
            return;
        }

        _idleSequence?.Kill();
        _nemoRoot.DOKill();
        _bodyImage.DOKill();
        _leftEye.DOKill();
        _rightEye.DOKill();
        _mouth.DOKill();

        _nemoRoot.anchoredPosition = _baseActorPosition;
        _nemoRoot.localScale = Vector3.one;
        _nemoRoot.localRotation = Quaternion.identity;
        _leftEye.anchoredPosition = _leftEyeBasePosition;
        _rightEye.anchoredPosition = _rightEyeBasePosition;
        _mouth.localScale = Vector3.one;
        _mouth.sizeDelta = new Vector2(46f, 8f);

        Color bodyColor = visualState switch
        {
            ENemoVisualState.Curious => new Color(1f, 0.86f, 0.52f, 1f),
            ENemoVisualState.Anxious => new Color(0.91f, 0.63f, 0.58f, 1f),
            ENemoVisualState.Trusting => new Color(0.62f, 0.88f, 0.74f, 1f),
            ENemoVisualState.Obedient => new Color(0.72f, 0.8f, 0.95f, 1f),
            ENemoVisualState.Conflicted => new Color(0.89f, 0.74f, 0.95f, 1f),
            _ => new Color(0.95f, 0.91f, 0.68f, 1f),
        };

        _bodyImage.color = bodyColor;
        _idleSequence = DOTween.Sequence().SetUpdate(true);

        switch (visualState)
        {
            case ENemoVisualState.Curious:
                _idleSequence.Append(_nemoRoot.DOAnchorPosY(_baseActorPosition.y + 10f, 0.6f).SetEase(Ease.InOutSine));
                _idleSequence.Join(_nemoRoot.DORotate(new Vector3(0f, 0f, -6f), 0.6f).SetEase(Ease.InOutSine));
                _idleSequence.Join(_leftEye.DOAnchorPosX(_leftEyeBasePosition.x + 4f, 0.6f));
                _idleSequence.Join(_rightEye.DOAnchorPosX(_rightEyeBasePosition.x + 4f, 0.6f));
                _idleSequence.Append(_nemoRoot.DOAnchorPosY(_baseActorPosition.y - 4f, 0.7f).SetEase(Ease.InOutSine));
                _idleSequence.Join(_nemoRoot.DORotate(Vector3.zero, 0.7f).SetEase(Ease.InOutSine));
                _idleSequence.Join(_leftEye.DOAnchorPos(_leftEyeBasePosition, 0.7f));
                _idleSequence.Join(_rightEye.DOAnchorPos(_rightEyeBasePosition, 0.7f));
                _idleSequence.SetLoops(-1);
                break;

            case ENemoVisualState.Anxious:
                _idleSequence.Append(_nemoRoot.DOShakeAnchorPos(0.85f, new Vector2(10f, 0f), 20, 90f, false, true));
                _idleSequence.Join(_nemoRoot.DOScale(0.94f, 0.4f).SetEase(Ease.InOutSine));
                _idleSequence.Append(_nemoRoot.DOScale(1f, 0.45f).SetEase(Ease.InOutSine));
                _idleSequence.SetLoops(-1);
                break;

            case ENemoVisualState.Trusting:
                _idleSequence.Append(_nemoRoot.DOScale(1.08f, 0.75f).SetEase(Ease.InOutQuad));
                _idleSequence.Join(_nemoRoot.DOAnchorPosY(_baseActorPosition.y + 8f, 0.75f).SetEase(Ease.InOutQuad));
                _idleSequence.Append(_nemoRoot.DOScale(1f, 0.75f).SetEase(Ease.InOutQuad));
                _idleSequence.Join(_nemoRoot.DOAnchorPosY(_baseActorPosition.y, 0.75f).SetEase(Ease.InOutQuad));
                _idleSequence.SetLoops(-1);
                break;

            case ENemoVisualState.Obedient:
                _idleSequence.Append(_nemoRoot.DOScaleY(0.96f, 1.1f).SetEase(Ease.InOutSine));
                _idleSequence.Append(_nemoRoot.DOScaleY(1f, 1.1f).SetEase(Ease.InOutSine));
                _idleSequence.SetLoops(-1);
                break;

            case ENemoVisualState.Conflicted:
                _idleSequence.Append(_nemoRoot.DORotate(new Vector3(0f, 0f, 7f), 0.45f).SetEase(Ease.InOutSine));
                _idleSequence.Join(_nemoRoot.DOAnchorPosX(_baseActorPosition.x + 8f, 0.45f));
                _idleSequence.Append(_nemoRoot.DORotate(new Vector3(0f, 0f, -7f), 0.45f).SetEase(Ease.InOutSine));
                _idleSequence.Join(_nemoRoot.DOAnchorPosX(_baseActorPosition.x - 8f, 0.45f));
                _idleSequence.Append(_nemoRoot.DORotate(Vector3.zero, 0.5f).SetEase(Ease.InOutSine));
                _idleSequence.Join(_nemoRoot.DOAnchorPosX(_baseActorPosition.x, 0.5f));
                _idleSequence.SetLoops(-1);
                break;

            default:
                _idleSequence.Append(_nemoRoot.DOScale(1.03f, 1.25f).SetEase(Ease.InOutSine));
                _idleSequence.Append(_nemoRoot.DOScale(1f, 1.25f).SetEase(Ease.InOutSine));
                _idleSequence.SetLoops(-1);
                break;
        }
    }

    private void ShowDialogue(string dialogueLine)
    {
        if (_dialogueText == null || _bubbleCanvasGroup == null)
        {
            return;
        }

        _dialogueSequence?.Kill();
        _bubbleCanvasGroup.DOKill();

        _dialogueText.text = string.IsNullOrWhiteSpace(dialogueLine) ? "..." : dialogueLine;

        _bubbleCanvasGroup.alpha = 0f;
        _bubbleRoot.localScale = new Vector3(0.95f, 0.95f, 1f);

        _dialogueSequence = DOTween.Sequence().SetUpdate(true);
        _dialogueSequence.Append(_bubbleCanvasGroup.DOFade(1f, 0.2f));
        _dialogueSequence.Join(_bubbleRoot.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
        _dialogueSequence.AppendInterval(DialogueVisibleDuration);
        _dialogueSequence.Append(_bubbleCanvasGroup.DOFade(0f, 0.25f));
    }

    private RectTransform FindStageRoot()
    {
        GameObject panel = GameObject.Find("CharacterPanel");
        if (panel != null && panel.TryGetComponent(out RectTransform panelRect))
        {
            return panelRect;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            return canvas.GetComponent<RectTransform>();
        }

        return null;
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
}
