using UnityEngine;

public class MinionReceiver : MonoBehaviour
{
    // TODO: enemies will have these so they can have minions attached to them (e.g. poison)
    // (mental model - imagine little minion parasites stuck onto enemies)
    [SerializeField] public Transform attachTransform;
    
    public void AttachSkill(SkillNode skillNode)
    {
        // TODO: spawn SkillInstanceSpawner and pass it skill info
    }
}
