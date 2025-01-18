using UnityEngine;
using System.Collections.Generic;


public class LevelGenerator : MonoBehaviour
{
    [Header("Level Settings")]
    public int numStages = 5; 
    public int minNodesPerStage = 1; 
    public int maxNodesPerStage = 4; 
    public float stageSpacing = 3.0f; 
    public float nodeSpacing = 2.0f; 
    [Range(0f, 1f)] public float minNodeProbability = 0.05f;
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

        List<LevelNode> stage1 = new List<LevelNode>();
        LevelNode startNode = new LevelNode
        {
            nodeType = NodeType.Start,
            position = new Vector2(0, 0)
        };
        stage1.Add(startNode);
        stages.Add(stage1);

        NormalizeNodeTypeProbabilities();

        for (int i = 1; i < numStages - 1; i++) 
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
            nodeType = NodeType.Boss, 
            position = new Vector2((numStages - 1) * stageSpacing, 0)
        };
        lastStage.Add(bossNode);
        stages.Add(lastStage);

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

            HashSet<LevelNode> connectedNextStageNodes = new HashSet<LevelNode>();

            foreach (LevelNode currentNode in currentStage)
            {
                LevelNode preferredNode = FindPreferredNode(currentNode, nextStage);

                currentNode.connectedNodes.Add(preferredNode);
                connectedNextStageNodes.Add(preferredNode);
            }

            foreach (LevelNode nextNode in nextStage)
            {
                if (!connectedNextStageNodes.Contains(nextNode))
                {
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
            if (Mathf.Abs(node.position.y - currentNode.position.y) <= 1)
            {
                return node;
            }

            float distance = Mathf.Abs(node.position.y - currentNode.position.y);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestNode = node;
            }
        }

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