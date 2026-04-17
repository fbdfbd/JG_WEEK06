using UnityEngine;

[CreateAssetMenu(
    fileName = "FlagDefinition_",
    menuName = "Scriptable Objects/Runtime/FlagDefinition")]
public class SO_FlagDefinition : ScriptableObject
{
    [SerializeField] private string _id = string.Empty;
    [SerializeField] private string _displayName = string.Empty;
    [SerializeField, TextArea(2, 4)] private string _description = string.Empty;

    public string Id => _id;
    public string DisplayName => _displayName;
    public string Description => _description;
}
