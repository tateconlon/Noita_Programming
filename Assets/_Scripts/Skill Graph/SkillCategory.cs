using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Category", menuName = "ScriptableObject/Skills/Skill Category", order = 0)]
public class SkillCategory : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public Color backgroundColor;
    public List<SkillCategory> rightSideCompatible;
}