using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

public class MinionPrefabsBuilder : MonoBehaviour
{
    [SerializeField, Required] private Actor _playerActor;
    [SerializeField] private SkillTreeInstance _skillTree;
    [ValidateInput(nameof(ValidateHierarchyRoot), messageType: InfoMessageType.Error)]
    [SerializeField] private Transform _hierarchyRoot;
    [SerializeField] private Tag.RequiredAndBlockedTags _actorsToClean;
    
    private readonly List<AbilityTrigger> _rootTriggers = new();
    
    // UnityEngine.Objects aren't garbage collected, so we need to manually destroy any we instantiate
    private readonly List<ScriptableObject> _runtimeScriptableObjects = new();

    private void OnEnable()
    {
        _skillTree.Value.OnModified += RebuildChain;
        
        RebuildChain();
    }

    [Button]
    public void RebuildChain()
    {
        Clear();
        
        BuildActor();
    }

    private void Clear()
    {
        // Destroy all triggers attached to the root
        foreach (AbilityTrigger rootTrigger in _rootTriggers)
        {
            Destroy(rootTrigger.gameObject);
        }
        _rootTriggers.Clear();
        
        // Destroy originally constructed prefabs
        foreach (Transform child in _hierarchyRoot)
        {
            Destroy(child.gameObject);
        }
        
        // Destroy runtime SO instances
        foreach (ScriptableObject runtimeScriptableObject in _runtimeScriptableObjects)
        {
            Destroy(runtimeScriptableObject);
        }
        _runtimeScriptableObjects.Clear();
        
        foreach (Actor actor in FindObjectsByType<Actor>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (_actorsToClean.IsSatisfiedBy(actor))
            {
                actor.gameObject.SetActive(false);
            }
        }
        
        foreach (LeanGameObjectPool pool in FindObjectsByType<LeanGameObjectPool>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (pool.Prefab == null || pool.Prefab.TryGetComponent(out Actor actor) && _actorsToClean.IsSatisfiedBy(actor))
            {
                pool.DespawnAll();
                pool.Clean();
                
                Destroy(pool.gameObject);
            }
        }
    }
    
    public void BuildActor()
    {
        Debug.LogException(new Exception("Deprecated"), this);
        
        // ActionNode root = _skillTree.Value.Root;
        //
        // if (root == null)
        // {
        //     Debug.LogException(new NullReferenceException($"No valid root for graph {_skillTree.name}"));
        //     return;
        // }
        //
        // Queue<(ActionNode node, Actor actor)> actorSpawnQueue = new();
        // actorSpawnQueue.Enqueue((root, _playerActor));
        //
        // while (actorSpawnQueue.Count > 0)
        // {
        //     // e.g. (brain, playerActor) on the first iteration
        //     (ActionNode actionNode, Actor actor) = actorSpawnQueue.Dequeue();
        //     
        //     // Children are direct children. Descendents tunnel all the way down
        //     // We use Children not child to support multi branching nodes
        //     foreach (TriggerNode triggerNode in actionNode.Children)
        //     {
        //         if (triggerNode.IsEmpty) continue; 
        //         
        //         AbilityTrigger curTrigger = Instantiate(triggerNode.Definition.abilityTriggerPrefab, actor.transform);
        //         
        //         // For cleanup since we have to manually delete triggers attached to player
        //         if (actor == _playerActor)
        //         {
        //             _rootTriggers.Add(curTrigger);
        //         }
        //         
        //         // ---- So far we've attached the Trigger Game Object, but the ability list is empty ----
        //         // ---- Now we will attach the triggered Ability(ies) to the Abilities GO of the $actor ----
        //         List<Ability> curTriggerAbilitiesToActivate = new();
        //         
        //         // We use Children not child to support multi branching nodes
        //         foreach (ActionNode nextActionNode in triggerNode.Children)
        //         {
        //             if (nextActionNode.IsEmpty) continue;
        //             
        //             Ability curAbility = Instantiate(nextActionNode.Definition.spawnActorAbility);
        //             actor.Abilities.AddAbility(curAbility);
        //             curTriggerAbilitiesToActivate.Add(curAbility);
        //             _runtimeScriptableObjects.Add(curAbility);  // For cleanup
        //             
        //             // All Actors are spawned under the inactive _hierarchyRoot so Awake and OnEnable are NOT called
        //             // These actors are then modified by later iterations of the loop
        //             Actor nextActor = Instantiate(nextActionNode.Definition.actorPrefab, _hierarchyRoot);
        //             
        //             curAbility.spawnedActorPrefab = nextActor;
        //             
        //             actorSpawnQueue.Enqueue((nextActionNode, nextActor));
        //         }
        //         
        //         curTrigger.Init(actor, curTriggerAbilitiesToActivate);
        //     }
        // }
    }
    
    private void OnDisable()
    {
        _skillTree.Value.OnModified -= RebuildChain;
    }

    private void OnDestroy()
    {
        // Destroy runtime SO instances
        foreach (ScriptableObject runtimeScriptableObject in _runtimeScriptableObjects)
        {
            Destroy(runtimeScriptableObject);
        }
        _runtimeScriptableObjects.Clear();
    }

    private bool ValidateHierarchyRoot(Transform hierarchyRoot, ref string errorMessage)
    {
        if (hierarchyRoot == null) return false;
        if (hierarchyRoot.gameObject.activeInHierarchy == false) return true;

        errorMessage = $"{nameof(_hierarchyRoot)} '{hierarchyRoot.name}' must be a disabled GameObject";
        return false;
    }
}
