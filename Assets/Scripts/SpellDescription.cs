using UnityEngine;

public class SpellDescription : ScriptableObject
{
    [Header("Spell Description")]
    public string SpellName;

    [TextArea(5,10)]
    public string Description;

    public Sprite SpellIcon;

    public float Duration;
    public float Cooldown;
    public string AnimationVariableName;

    [Header("UI")]
    public float UIInCastMaxSize = 1;
    public float UIShrinkDuration = 0.3f;
}
