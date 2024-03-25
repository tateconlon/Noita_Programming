using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Global Ability", menuName = "ScriptableObject/Actor System/Global Ability", order = 0)]
public class GlobalAbility : ScriptableObject
{
    [HideLabel]
    public Ability value;
}