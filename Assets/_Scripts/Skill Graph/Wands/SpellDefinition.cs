using UnityEngine;

public abstract class SpellDefinition : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    [TextArea]
    public string description;

    [TextArea] public string attackDescTooltip;
    
    public abstract string SpellTypeDisplayName { get; }
    
    // Could also try implementing the data above ^^^ as "fragments" like here:
    // https://docs.unrealengine.com/5.0/en-US/lyra-inventory-and-equipment-in-unreal-engine/
    // Not sure how they work, but maybe it's like each fragment provides its own UI representation (i.e. prefab)?
    
    // TODO: Handle spell attributes this way?  https://noita.fandom.com/wiki/Spells#Spell_Attributes
    // [SerializeField] public List<Ability> wandMods = new();  // E.g. modify wand's recharge time
}