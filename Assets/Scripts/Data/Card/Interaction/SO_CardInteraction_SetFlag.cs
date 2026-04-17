using UnityEngine;

[CreateAssetMenu(
    fileName = "Interaction_SetFlag",
    menuName = "Scriptable Objects/Card/Interaction/SetFlag")]
public class SO_CardInteraction_SetFlag : SO_CardInteractionDefinition
{
    [SerializeField] private SO_FlagDefinition _flagDefinition;

    public override void Apply(RuntimeChildState childState)
    {
        childState.SetFlag(_flagDefinition);
    }
}
