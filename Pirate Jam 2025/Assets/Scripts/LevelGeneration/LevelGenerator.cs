using UnityEngine;
using System.Collections.Generic;


public class LevelGenerator : MonoBehaviour
{
    [Header("Level Settings")]
    public int numStages = 5; // Total number of stages (columns)
    public int minNodesPerStage = 1; // Minimum nodes in each stage (except the start)
    public int maxNodesPerStage = 4; // Maximum nodes in each stage
    public float stageSpacing = 3.0f; // Horizontal spacing between stages
    public float nodeSpacing = 2.0f; // Vertical spacing between nodes in a stage
    [Range(0f, 1f)] public float minNodeProbability = 0.05f; // Likelihood of selecting minNodesPerStage
    [Range(0f, 1f)] public float maxNodeProbability = 0.05f;

    [Header("Node Type Likelihoods")]
    public List<NodeTypeProbability> nodeTypeProbabilities = new List<NodeTypeProbability>();

    [System.Serializable]
    public class NodeTypeProbability
    {
        public NodeType nodeType;
        [Range(0f, 1f)] public float probability;
    }


    private List<List<LevelNode>> stages = new List<List<LevelNode>>();

    public List<List<LevelNode>> GetStages() => stages;

    private void NormalizeNodeTypeProbabilities()
    {
        float totalProbability = 0f;
        foreach (var prob in nodeTypeProbabilities)
        {
            totalProbability += prob.probability;
        }

        if (totalProbability == 0)
        {
            Debug.LogWarning("Node type probabilities are all zero! Defaulting to equal probabilities.");
            float equalProbability = 1f / nodeTypeProbabilities.Count;
            foreach (var prob in nodeTypeProbabilities)
            {
                prob.probability = equalProbability;
            }
            return;
        }

        foreach (var prob in nodeTypeProbabilities)
        {
            prob.probability /= totalProbability;
        }
    }

    private int GetNodeCount()
    {
        float randomValue = Random.value;

        if (randomValue < minNodeProbability)
        {
            return minNodesPerStage;
        }
        else if (randomValue > 1f - maxNodeProbability)
        {
            return maxNodesPerStage;
        }
        else
        {
            return Random.Range(minNodesPerStage + 1, maxNodesPerStage);
        }
    }


    public void GenerateLevel()
    {
        stages.Clear();

        // Step 1: Create Stage 1 (Start Node)
        List<LevelNode> stage1 = new List<LevelNode>();
        LevelNode startNode = new LevelNode
        {
            nodeType = NodeType.Start,
            position = new Vector2(0, 0)
        };
        stage1.Add(startNode);
        stages.Add(stage1);

        // Step 2: Normalize Node Type Probabilities
        NormalizeNodeTypeProbabilities();

        // Step 3: Create Intermediate Stages
        for (int i = 1; i < numStages - 1; i++) // Exclude last stage
        {
            int nodeCount = GetNodeCount();
            List<LevelNode> currentStage = new List<LevelNode>();

            for (int j = 0; j < nodeCount; j++)
            {
                LevelNode node = new LevelNode
                {
                    nodeType = GetRandomNodeType(),
                    position = new Vector2(i * stageSpacing, j * nodeSpacing)
                };
                currentStage.Add(node);
            }

            stages.Add(currentStage);
        }

        // Step 4: Create Last Stage (End Node)
        List<LevelNode> lastStage = new List<LevelNode>();
        LevelNode bossNode = new LevelNode
        {
            nodeType = NodeType.Boss, // Ensure a boss node in the final stage
            position = new Vector2((numStages - 1) * stageSpacing, 0)
        };
        lastStage.Add(bossNode);
        stages.Add(lastStage);

        // Step 5: Create Connections Between Stages
        CreateConnections();
    }



    private NodeType GetRandomNodeType()
    {
        return (NodeType)Random.Range(1, System.Enum.GetValues(typeof(NodeType)).Length - 1);
    }

    private void CreateConnections()
    {
        for (int i = 0; i < stages.Count - 1; i++)
        {
            List<LevelNode> currentStage = stages[i];
            List<LevelNode> nextStage = stages[i + 1];

            // Track connections for each node in the next stage to ensure all are connected
            HashSet<LevelNode> connectedNextStageNodes = new HashSet<LevelNode>();

            // Step 1: Connect nodes in the current stage to the next stage
            foreach (LevelNode currentNode in currentStage)
            {
                // Find a preferred node in the next stage (-1, 0, +1 range)
                LevelNode preferredNode = FindPreferredNode(currentNode, nextStage);

                // Add connection
                currentNode.connectedNodes.Add(preferredNode);
                connectedNextStageNodes.Add(preferredNode);
            }

            // Step 2: Ensure each node in the next stage has at least one connection
            foreach (LevelNode nextNode in nextStage)
            {
                if (!connectedNextStageNodes.Contains(nextNode))
                {
                    // Find the closest node from the current stage
                    LevelNode closestNode = FindClosestNodeByY(currentStage, nextNode);
                    closestNode.connectedNodes.Add(nextNode);
                }
            }
        }
    }

    private LevelNode FindPreferredNode(LevelNode currentNode, List<LevelNode> nextStage)
    {
        LevelNode closestNode = null;
        float smallestDistance = float.MaxValue;

        foreach (LevelNode node in nextStage)
        {
            // Check if the node is in the preferred range
            if (Mathf.Abs(node.position.y - currentNode.position.y) <= 1)
            {
                return node; // Immediately return if a preferred node is found
            }

            // Calculate distance for fallback
            float distance = Mathf.Abs(node.position.y - currentNode.position.y);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestNode = node;
            }
        }

        // Fallback to the closest node
        return closestNode;
    }

    private LevelNode FindClosestNodeByY(List<LevelNode> previousStage, LevelNode targetNode)
    {
        LevelNode closestNode = null;
        float smallestDistance = float.MaxValue;

        foreach (LevelNode node in previousStage)
        {
            float distance = Mathf.Abs(node.position.y - targetNode.position.y);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }

}