using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

// Uses: https://github.com/Unity-Technologies/graph-visualizer/blob/master/Editor/Graph/Layouts/ReingoldTilford.cs
// Which is an implementation of the Reingold-Tilford algorithm for graph layout

// NOTE: The original code assumes the depth of the root to be 1, so I've added 1 to depth queries where needed
// See https://github.com/Unity-Technologies/graph-visualizer/blob/master/Editor/Graph/Node.cs

public class SkillTreeBuilder : MonoBehaviour
{
    [SerializeField] private SkillTreeInstance _skillTree;
    [SerializeField] private CollectionBoundPrefabs<Vertex, EquippedSkillIcon> _vertexUi;
    [SerializeField] private CollectionBoundPrefabs<Edge, SkillTreeEdge> _edgeUi;
    
    [Header("Parameters")]
    [Tooltip("By convention, all graph layout algorithms should have a minimum distance of 1 unit between nodes")]
    [SerializeField] private float _distanceBetweenNodes = 1.0f;
    [Tooltip("Used to specify the vertical distance two non-attached trees in the graph.")]
    [SerializeField] private float _verticalDistanceBetweenTrees = 3.0f;
    [Tooltip("Used to lengthen the wire when lots of children are connected. If 1, all levels will be evenly separated")]
    [SerializeField] private float _wireLengthFactorForLargeSpanningTrees = 3.0f;
    [SerializeField] private float _maxChildrenThreshold = 6.0f;
    [SerializeField] private bool _leftToRight = true;

    // Helper structure to easily find the vertex associated to a given Node.
    [ShowInInspector, ReadOnly]
    private readonly Dictionary<SkillNode, Vertex> _nodeVertexLookup = new();

    public IEnumerable<Vertex> vertices => _nodeVertexLookup.Values;

    public IEnumerable<Edge> edges
    {
        get
        {
            List<Edge> edgesList = new List<Edge>();
            foreach (KeyValuePair<SkillNode, Vertex> node in _nodeVertexLookup)
            {
                Vertex v = node.Value;
                foreach (SkillNode child in v.node.ChildrenBase)
                {
                    edgesList.Add(new Edge(_vertexUi[v], _vertexUi[_nodeVertexLookup[child]]));
                }
            }

            return edgesList;
        }
    }
    
    private void OnEnable()
    {
        _skillTree.Value.OnModified += Build;
        
        Build();
        StartCoroutine(SelectFirstEmptyNode());
    }
    
    /// <summary>
    /// For tutorialization/bringing the player's attention to extending their chain of behaviors
    /// </summary>
    private IEnumerator SelectFirstEmptyNode()
    {
        yield return new WaitForEndOfFrame();  // Wait until end of frame so everything can be enabled
        
        foreach (EquippedSkillIcon equippedSkillIcon in _vertexUi.Values)
        {
            if (equippedSkillIcon.TryGetComponent(out SkillIcon skillIcon) && skillIcon.BoundTarget.IsEmpty &&
                equippedSkillIcon.TryGetComponent(out SkillIconUiEventHandler eventHandler))
            {
                eventHandler.OnPointerClick(new PointerEventData(EventSystem.current));
                yield break;
            }
        }
    }
    
    [Button]
    public void Build()
    {
        CalculateLayout();
        RecenterVertices();
        
        _vertexUi.Bind(vertices);
        _edgeUi.Bind(edges);
    }
    
    private void CalculateLayout()
    {
        _nodeVertexLookup.Clear();
        foreach (SkillNode node in _skillTree.Value.nodes)
        {
            _nodeVertexLookup.Add(node, new Vertex(node));
        }

        if (_nodeVertexLookup.Count == 0) return;

        IList<float> horizontalPositions = ComputeHorizontalPositionForEachLevel();

        List<SkillNode> roots = _nodeVertexLookup.Keys.Where(n => !n.HasParent).ToList();

        for (int i = 0; i < roots.Count; ++i)
        {
            RecursiveLayout(roots[i], 0, horizontalPositions);

            if (i > 0)
            {
                Vector2 previousRootRange = ComputeRangeRecursive(roots[i - 1]);
                RecursiveMoveSubtree(roots[i],
                    previousRootRange.y + _verticalDistanceBetweenTrees + _distanceBetweenNodes);
            }
        }
    }

    // Precompute the horizontal position for each level.
    // Levels with few wires (as measured by the maximum number of children for one node) are placed closer
    // apart; very cluttered levels are placed further apart.
    private float[] ComputeHorizontalPositionForEachLevel()
    {
        // Gather information about depths.
        int maxDepth = int.MinValue;
        Dictionary<int, List<SkillNode>> nodeDepths = new();
        foreach (SkillNode node in _nodeVertexLookup.Keys)
        {
            int depth = _skillTree.Value.GetNodeDepth(node) + 1;  // NOTE: root depth = 1 here
            List<SkillNode> nodes;
            if (!nodeDepths.TryGetValue(depth, out nodes))
            {
                nodeDepths[depth] = nodes = new List<SkillNode>();
            }

            nodes.Add(node);
            maxDepth = Mathf.Max(depth, maxDepth);
        }

        // Bake the left to right horizontal positions.
        float[] horizontalPositionForDepth = new float[maxDepth];
        horizontalPositionForDepth[0] = 0;
        for (int d = 1; d < maxDepth; ++d)
        {
            IEnumerable<SkillNode> nodesOnThisLevel = nodeDepths[d + 1];

            int maxChildren = nodesOnThisLevel.Max(x => x.ChildrenBase.Length);

            float wireLengthHeuristic = Mathf.Lerp(1, _wireLengthFactorForLargeSpanningTrees,
                Mathf.Min(1, maxChildren / _maxChildrenThreshold));

            horizontalPositionForDepth[d] = horizontalPositionForDepth[d - 1] +
                                            _distanceBetweenNodes * wireLengthHeuristic;
        }

        return _leftToRight ? horizontalPositionForDepth : horizontalPositionForDepth.Reverse().ToArray();
    }

    // Traverse the graph and place all nodes according to the algorithm
    private void RecursiveLayout(SkillNode node, int depth, IList<float> horizontalPositions)
    {
        IList<SkillNode> children = node.ChildrenBase;
        foreach (SkillNode child in children)
        {
            RecursiveLayout(child, depth + 1, horizontalPositions);
        }

        float yPos = 0.0f;
        if (children.Count > 0)
        {
            SeparateSubtrees(children);
            yPos = GetAveragePosition(children).y;
        }

        Vector2 pos = new Vector2(horizontalPositions[depth], yPos);
        _nodeVertexLookup[node].position = pos;
    }

    private Vector2 ComputeRangeRecursive(SkillNode node)
    {
        Vector2 range = Vector2.one * _nodeVertexLookup[node].position.y;
        foreach (SkillNode child in node.ChildrenBase)
        {
            Vector2 childRange = ComputeRangeRecursive(child);
            range.x = Mathf.Min(range.x, childRange.x);
            range.y = Mathf.Max(range.y, childRange.y);
        }

        return range;
    }

    // Determine parent's vertical position based on its children
    private Vector2 GetAveragePosition(ICollection<SkillNode> children)
    {
        Vector2 centroid = new();

        centroid = children.Aggregate(centroid, (current, n) => current + _nodeVertexLookup[n].position);

        if (children.Count > 0)
            centroid /= children.Count;

        return centroid;
    }

    // Separate the given subtrees so they do not overlap
    private void SeparateSubtrees(IList<SkillNode> subroots)
    {
        if (subroots.Count < 2)
            return;

        SkillNode upperNode = subroots[0];

        Dictionary<int, Vector2> upperTreeBoundaries = GetBoundaryPositions(upperNode);
        for (int s = 0; s < subroots.Count - 1; s++)
        {
            SkillNode lowerNode = subroots[s + 1];
            Dictionary<int, Vector2> lowerTreeBoundaries = GetBoundaryPositions(lowerNode);

            int minDepth = upperTreeBoundaries.Keys.Min();
            if (minDepth != lowerTreeBoundaries.Keys.Min())
                Debug.LogError("Cannot separate subtrees which do not start at the same root depth");

            int lowerMaxDepth = lowerTreeBoundaries.Keys.Max();
            int upperMaxDepth = upperTreeBoundaries.Keys.Max();
            int maxDepth = System.Math.Min(upperMaxDepth, lowerMaxDepth);

            for (int depth = minDepth; depth <= maxDepth; depth++)
            {
                float delta = _distanceBetweenNodes - (lowerTreeBoundaries[depth].x - upperTreeBoundaries[depth].y);
                delta = System.Math.Max(delta, 0);
                RecursiveMoveSubtree(lowerNode, delta);
                for (int i = minDepth; i <= lowerMaxDepth; i++)
                    lowerTreeBoundaries[i] += new Vector2(delta, delta);
            }

            upperTreeBoundaries = CombineBoundaryPositions(upperTreeBoundaries, lowerTreeBoundaries);
        }
    }

    // Using a Vector2 at each depth to hold the extrema vertical positions
    private Dictionary<int, Vector2> GetBoundaryPositions(SkillNode subTreeRoot)
    {
        Dictionary<int, Vector2> extremePositions = new();

        IEnumerable<SkillNode> descendants = GetSubtreeNodes(subTreeRoot);

        foreach (SkillNode node in descendants)
        {
            int depth = _skillTree.Value.GetNodeDepth(_nodeVertexLookup[node].node) + 1;  // NOTE: root depth = 1 here
            float pos = _nodeVertexLookup[node].position.y;
            if (extremePositions.ContainsKey(depth))
                extremePositions[depth] = new Vector2(Mathf.Min(extremePositions[depth].x, pos),
                    Mathf.Max(extremePositions[depth].y, pos));
            else
                extremePositions[depth] = new Vector2(pos, pos);
        }

        return extremePositions;
    }

    // Includes all descendants and the subtree root itself
    private IEnumerable<SkillNode> GetSubtreeNodes(SkillNode root)
    {
        List<SkillNode> allDescendants = new() { root };
        foreach (SkillNode child in root.ChildrenBase)
        {
            allDescendants.AddRange(GetSubtreeNodes(child));
        }

        return allDescendants;
    }

    // After adjusting a subtree, compute its new boundary positions
    private Dictionary<int, Vector2> CombineBoundaryPositions(Dictionary<int, Vector2> upperTree,
        Dictionary<int, Vector2> lowerTree)
    {
        Dictionary<int, Vector2> combined = new();
        int minDepth = upperTree.Keys.Min();
        int maxDepth = System.Math.Max(upperTree.Keys.Max(), lowerTree.Keys.Max());

        for (int d = minDepth; d <= maxDepth; d++)
        {
            float upperBoundary = upperTree.ContainsKey(d) ? upperTree[d].x : lowerTree[d].x;
            float lowerBoundary = lowerTree.ContainsKey(d) ? lowerTree[d].y : upperTree[d].y;
            combined[d] = new Vector2(upperBoundary, lowerBoundary);
        }

        return combined;
    }

    // Apply a vertical delta to all nodes in a subtree
    private void RecursiveMoveSubtree(SkillNode subtreeRoot, float yDelta)
    {
        Vector2 pos = _nodeVertexLookup[subtreeRoot].position;
        _nodeVertexLookup[subtreeRoot].position = new Vector2(pos.x, pos.y + yDelta);

        foreach (SkillNode child in subtreeRoot.ChildrenBase)
        {
            RecursiveMoveSubtree(child, yDelta);
        }
    }

    private void RecenterVertices()
    {
        Vector2 center = vertices.GetCenterPoint(vertex => vertex.position);

        foreach (Vertex vertex in vertices)
        {
            vertex.Recenter(center);
        }
    }
    
    private void OnDisable()
    {
        _skillTree.Value.OnModified -= Build;
    }
    
    public class Edge
    {
        // Indices in the vertex array of the layout algorithm.
        public Edge(EquippedSkillIcon src, EquippedSkillIcon dest)
        {
            source = src;
            destination = dest;
        }

        public EquippedSkillIcon source { get; }

        public EquippedSkillIcon destination { get; }
    }

    // One vertex is associated to each node in the graph.
    public class Vertex
    {
        // Center of the node in the graph layout.
        public Vector2 position { get; set; }

        // The Node represented by the vertex.
        public SkillNode node { get; }

        public Vertex(SkillNode node)
        {
            this.node = node;
        }

        public void Recenter(Vector2 newCenter)
        {
            position -= newCenter;
        }
    }
}