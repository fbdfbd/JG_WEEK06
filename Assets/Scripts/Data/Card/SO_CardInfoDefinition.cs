using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CardInfoDefinition_",
    menuName = "Scriptable Objects/Card/CardInfoDefinition")]
public class SO_CardInfoDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string _id;
    [SerializeField] private SO_CardInfoTypeDefinition _cardType;

    [SerializeField] private string _title;
    [SerializeField, TextArea(3, 6)] private string _originalText;

    [Header("Options")]
    [SerializeField] private CardOptionData[] _options;

    public string Id => _id;
    public SO_CardInfoTypeDefinition CardType => _cardType;
    public string Title => _title;
    public string OriginalText => _originalText;
    public CardOptionData[] Options => _options;


}

[Serializable]
public class CardOptionData
{
    [SerializeField] private ECardOptionSemantic _semantic;
    [SerializeField] private string _label;
    [SerializeField, TextArea] private string _presentedText;
    [SerializeField] private SO_CardInteractionDefinition[] _interactions;

    public ECardOptionSemantic Semantic => _semantic;
    public string Label => _label;
    public string PresentedText => _presentedText;
    public SO_CardInteractionDefinition[] Interactions => _interactions;
}

public enum ECardOptionSemantic
{
    Direct,      // 원본 그대로 통과
    Modified,    // 수정/순화/검열 후 통과
    Blocked      // 차단/보류/거부
}
