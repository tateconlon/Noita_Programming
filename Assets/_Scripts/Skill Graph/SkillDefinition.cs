using UnityEngine;

[CreateAssetMenu(fileName = "Skill Definition", menuName = "ScriptableObject/Skills/Skill Definition", order = 0)]
public class SkillDefinition : ScriptableObject
{
    public string displayName;
    public string description;
    public Sprite icon;
    public SkillCategory category;
    public bool canRollInShop = true;
    public int tier;
    public int Cost => tier;
    public bool ignoreReceiver = false;
    
    [Header("Prefabs")]
    public Actor actorPrefab;
    public AbilityTrigger abilityTriggerPrefab;
    public Ability spawnActorAbility;
}
