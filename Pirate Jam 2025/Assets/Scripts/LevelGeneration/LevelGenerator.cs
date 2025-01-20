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

    private List<TreasuryData> treasuryEvents = new List<TreasuryData>();
    private List<MysteryData> mysteryEvents = new List<MysteryData>();
    private List<EncounterData> encounterEvents = new List<EncounterData>();
    private List<EncounterData> miniBossEvents = new List<EncounterData>();
    private List<EncounterData> bossEvents = new List<EncounterData>();

    public List<List<LevelNode>> GetStages() => stages;

    public void InitializeGenerator()
    {
        LoadNodeData();
        GenerateLevel();
    }

    private void LoadNodeData()
    {
        treasuryEvents = NormalizeWeights(
            new List<TreasuryData>(Resources.LoadAll<TreasuryData>("TreasuryEvents")),
            data => data.probability
        );

        mysteryEvents = NormalizeWeights(
            new List<MysteryData>(Resources.LoadAll<MysteryData>("MysteryEvents")),
            data => data.probability
        );

        encounterEvents = NormalizeWeights(
            new List<EncounterData>(Resources.LoadAll<EncounterData>("EncounterEvents")),
            data => data.probability
        );

        miniBossEvents = NormalizeWeights(
            new List<EncounterData>(Resources.LoadAll<EncounterData>("MiniBossEvents")),
            data => data.probability
        );

        bossEvents = NormalizeWeights(
            new List<EncounterData>(Resources.LoadAll<EncounterData>("BossEvents")),
            data => data.probability
        );
    }

    private List<T> NormalizeWeights<T>(List<T> dataList, System.Func<T, float> weightSelector)
    {
        float totalWeight = 0f;

        foreach (var data in dataList)
        {
            totalWeight += weightSelector(data);
        }

        if (totalWeight == 0)
        {
            Debug.LogWarning("All weights are zero! Defaulting to equal probabilities.");
            return dataList;
        }

        List<T> normalizedList = new List<T>();
        foreach (var data in dataList)
        {
            float weight = weightSelector(data);
            if (weight > 0)
            {
                int copies = Mathf.RoundToInt((weight / totalWeight) * 100);
                for (int i = 0; i < copies; i++)
                {
                    normalizedList.Add(data);
                }
            }
        }

        return normalizedList;
    }

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
                NodeType nodeType = GetRandomNodeType();
                LevelNode node = new LevelNode
                {
                    nodeType = nodeType,
                    position = new Vector2(i * stageSpacing, j * nodeSpacing)
                };

                switch (nodeType)
                {
                    case NodeType.Treasure:
                        if (treasuryEvents.Count > 0)
                            node.treasuryData = treasuryEvents[Random.Range(0, treasuryEvents.Count)];
                        break;
                    case NodeType.Mystery:
                        if (mysteryEvents.Count > 0)
                            node.mysteryData = mysteryEvents[Random.Range(0, mysteryEvents.Count)];
                        break;
                    case NodeType.Encounter:
                        if (encounterEvents.Count > 0)
                            node.combatData = encounterEvents[Random.Range(0, encounterEvents.Count)];
                        break;
                    case NodeType.MiniBoss:
                        if (miniBossEvents.Count > 0)
                            node.combatData = miniBossEvents[Random.Range(0, miniBossEvents.Count)];
                        break;
                    case NodeType.Boss:
                        if (bossEvents.Count > 0)
                            node.combatData = bossEvents[Random.Range(0, bossEvents.Count)];
                        break;
                }

                currentStage.Add(node);
            }

            stages.Add(currentStage);
        }

        List<LevelNode> lastStage = new List<LevelNode>();
        LevelNode bossNode = new LevelNode
        {
            nodeType = NodeType.Boss,
            position = new Vector2((numStages - 1) * stageSpacing, 0)
        };
        if (bossEvents.Count > 0)
        {
            bossNode.combatData = bossEvents[Random.Range(0, bossEvents.Count)];
        }
        lastStage.Add(bossNode);
        stages.Add(lastStage);

        CreateConnections();
    }

    private NodeType GetRandomNodeType()
    {
        float randomValue = Random.value;
        float cumulativeProbability = 0f;

        foreach (var prob in nodeTypeProbabilities)
        {
            cumulativeProbability += prob.probability;
            if (randomValue <= cumulativeProbability)
            {
                return prob.nodeType;
            }
        }

        return NodeType.Encounter;
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
