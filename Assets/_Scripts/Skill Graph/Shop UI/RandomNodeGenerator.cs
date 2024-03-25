using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Serialization;

public interface INodeGenerator
{
    //public static INodeGenerator Instance = new DefaultNodeGenerator();
    public void RequestGenerateNodes(int numNodes, int numNodes2);
}

public class DefaultNodeGenerator : INodeGenerator
{
    public void RequestGenerateNodes(int numNodes, int numNodes2)
    {
        Debug.LogException(new UnassignedReferenceException($"No implementation of {nameof(INodeGenerator)} assigned"));
    }
}

public class RandomNodeGenerator : MonoBehaviour, INodeGenerator
{
    [SerializeField] private SkillWeightsPerShopLevelDict _skillWeightsPerShopLevel;
    [SerializeField] private ScopedVariable<int> _shopLevel;
    [SerializeField] private ScopedVariable<int> _playerLevel;
    [SerializeField] private List<SkillDefinition> _actionNodesInShop;
    [SerializeField] private List<SkillDefinition> _triggerNodesInShop;

    [SerializeField] private SkillShopInstance _outputActionNodesInventory; 
    [SerializeField] private SkillShopInstance _outputTriggerNodesInventory;
    
    private void Awake()
    {
        //INodeGenerator.Instance = this;
    }

    private void OnEnable()
    {
        _playerLevel.onChangeValue.AddResponse(OnPlayerLevelChange);
    }
    
    private void Start()
    {
        RequestGenerateNodes(3, 2);
    }

    private void OnPlayerLevelChange(int newLevel)
    {
        RequestGenerateNodes(3, 2);
    }
    
    public void RequestGenerateNodes(int numNodes, int numNodes2)
    {
        // Now we can delete the previous nodes since the selected one has been copied to inventory
        _outputActionNodesInventory.Value.Clear();
        _outputTriggerNodesInventory.Value.Clear();
        
        foreach (SkillNode newNode in Generate(numNodes, _actionNodesInShop))
        {
            _outputActionNodesInventory.Value.CopyNode(newNode);
        }
        
        foreach (SkillNode newNode in Generate(numNodes2, _triggerNodesInShop))
        {
            _outputTriggerNodesInventory.Value.CopyNode(newNode);
        }
    }
    
    [Button]
    public List<SkillNode> Generate(int numNodes, List<SkillDefinition> _nodeList)
    {
        List<SkillNode> nodes = new();

        if (!_skillWeightsPerShopLevel.TryGetValue(_shopLevel.value, out SkillWeightDict skillWeights))
        {
            Debug.LogException(new KeyNotFoundException($"No skill weights found for shop level {_shopLevel.value}"), this);
            return nodes;
        }

        for (int i = 0; i < numNodes; i++)
        {
            SkillDefinition definition = _nodeList.WeightedPick(entry => skillWeights.GetValueOrDefault(entry.tier));
            // SkillTier tier = _tierWeights.WeightedPick(entry => entry.Value);

            SkillNode newNode;

            // This check is nasty - should have a better solution.
            // Really Minions and Triggers shouldn't inherit from same base class, they shouldn't be treated the same
            if (definition.actorPrefab != null)
            {
                newNode = ScriptableObject.CreateInstance<ActionNode>();
            }
            else
            {
                newNode = ScriptableObject.CreateInstance<TriggerNode>();
            }

            newNode.Definition = definition;
            
            nodes.Add(newNode);
        }
        
        return nodes;
    }
    
    private void OnDisable()
    {
        _playerLevel.onChangeValue.RemoveResponse(OnPlayerLevelChange);
    }

    [Serializable] private class SkillWeightsPerShopLevelDict : UnitySerializedDictionary<int, SkillWeightDict> { }
    [Serializable] private class SkillWeightDict : UnitySerializedDictionary<int, float> { }
    // [Serializable] private class TierWeightDict : UnitySerializedDictionary<SkillTier, float> { }
}
