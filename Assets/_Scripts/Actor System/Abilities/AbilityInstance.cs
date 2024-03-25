// An ability that has requirements and takes time to use,
// e.g. casting a spell that requires mana, has cooldown, takes time to cast, can be interrupted, etc.
// https://docs.unrealengine.com/5.2/en-US/using-gameplay-abilities-in-unreal-engine/
// https://github.com/sjai013/unity-gameplay-ability-system/wiki/A-simple-ability
// https://github.com/tranek/GASDocumentation#concepts-ga-spec

//TATE: We won't use Ability Instance to instance a definition of an ability any more, since we just do new PortalAbility()
//But, it might still be useful to wrap the use of an ability in a packet to get more context of when/how it was used
//Similar to all the extra parameters you might want in an Event.
public class AbilityInstance
{
    public readonly Ability Type;
    public readonly Actor Owner;
    
    public AbilityInstance(Ability ability, Actor owner)
    {
        Type = ability;
        Owner = owner;
    }
    
    public bool Activate(TargetData targetData)
    {
        // TODO: add in ability to modify effects based on number of targets hit in this method
        foreach (Actor target in targetData.Targets)
        {
            //target.TryReceiveAbility(this);
        }

        return true;
    }
    
    public bool TryCancel()
    {
        // TODO
        
        return true;
    }
    
    // needs state, e.g. activated, in-progress, finished
    // TryActivate, TryCancel, and OnCancelled event. Internal functionality to cleanup on ability end
}