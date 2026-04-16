using UnityEngine;

public abstract class SO_CardInteractionDefinition : ScriptableObject
{
    public abstract void Apply(RuntimeChildState childState);
}