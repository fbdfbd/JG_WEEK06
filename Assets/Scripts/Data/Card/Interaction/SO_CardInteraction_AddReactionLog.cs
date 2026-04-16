using UnityEngine;

[CreateAssetMenu(
    fileName = "Interaction_AddReactionLog",
    menuName = "Scriptable Objects/Card/Interaction/AddReactionLog")]
public class SO_CardInteraction_AddReactionLog : SO_CardInteractionDefinition
{
    [SerializeField, TextArea] private string _reactionText;

    public override void Apply(RuntimeChildState childState)
    {
        childState.AddReactionLog(_reactionText);
    }
}
