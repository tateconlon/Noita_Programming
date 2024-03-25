using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

public class RuntimeSkillGraph : MonoBehaviour, IPointerClickHandler
{
    [Header("Graph")]
    public SkillTree tree;
    
    [Header("Prefabs")]
    public SkillNodeUi runtimeMathNodePrefab;
    public Connection runtimeConnectionPrefab;
    
    [Header("References")]
    public UGUIContextMenu graphContextMenu;
    public UGUIContextMenu nodeContextMenu;
    public UGUITooltip tooltip;

    public ScrollRect scrollRect { get; private set; }
    private List<UiNodeBase> nodes;

    private void Awake()
    {
        // Create a clone so we don't modify the original asset
        tree = tree.Copy() as SkillTree;
        scrollRect = GetComponentInChildren<ScrollRect>();
        graphContextMenu.onClickSpawn -= SpawnNode;
        graphContextMenu.onClickSpawn += SpawnNode;
    }

    private void Start()
    {
        SpawnGraph();
    }

    public void Refresh()
    {
        Clear();
        SpawnGraph();
    }

    public void Clear()
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            Destroy(nodes[i].gameObject);
        }

        nodes.Clear();
    }

    public void SpawnGraph()
    {
        if (nodes != null) nodes.Clear();
        else nodes = new List<UiNodeBase>();

        for (int i = 0; i < tree.nodes.Count; i++)
        {
            Node node = tree.nodes[i];

            UiNodeBase runtimeSkillNode = null;
            if (node is SkillNode)
            {
                runtimeSkillNode = Instantiate(runtimeMathNodePrefab);
            }

            runtimeSkillNode.transform.SetParent(scrollRect.content);
            runtimeSkillNode.node = node;
            runtimeSkillNode.graph = this;
            nodes.Add(runtimeSkillNode);
        }
    }

    public UiNodeBase GetRuntimeNode(Node node)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].node == node)
            {
                return nodes[i];
            }
            else
            {
            }
        }

        return null;
    }

    public void SpawnNode(Type type, Vector2 position)
    {
        Node node = tree.AddNode(type);
        node.name = type.Name;
        node.position = position;
        Refresh();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        graphContextMenu.OpenAt(eventData.position);
    }
}