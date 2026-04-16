using UnityEngine;

[CreateAssetMenu(
    fileName = "CardInfoType_",
    menuName = "Scriptable Objects/Card/CardInfoTypeDefinition")]
public class SO_CardInfoTypeDefinition : ScriptableObject
{
    [SerializeField] private string _id = string.Empty;
    [SerializeField] private string _displayName = string.Empty;

    public string Id => _id;
    public string DisplayName => _displayName;
}
