using UnityEngine;

[CreateAssetMenu(
    fileName = "CardInteraction_Group",
    menuName = "Scriptable Objects/Card/Interaction/GroupDelta")]
public class SO_CardInteraction_GroupStatDelta : SO_CardInteractionDefinition
{
    [SerializeField] private SO_CardInteractionDefinition[] _interactions;

    public override void Apply(RuntimeChildState childState)
    {
        foreach (var interaction in _interactions)
        {
            interaction.Apply(childState);
        }
    }
}
