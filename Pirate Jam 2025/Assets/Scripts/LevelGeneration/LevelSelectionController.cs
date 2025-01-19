using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelSelectionController : MonoBehaviour
{
    [SerializeField] private UILevelController uiLevelController;
    [SerializeField] private Transform levelNodesParent;

    private LevelNodeUIController currentSelectedNode;
    private List<LevelNodeUIController> allNodeControllers = new List<LevelNodeUIController>();

    public void InitializeLevelNodes()
    {
        foreach (Transform child in levelNodesParent)
        {
            var nodeController = child.GetComponent<LevelNodeUIController>();
            if (nodeController != null)
            {
                allNodeControllers.Add(nodeController);
                InitializeNode(nodeController);
            }
        }

        // Set the initial current node (e.g., the Start node)
        if (allNodeControllers.Count > 0)
        {
            SetCurrentNode(allNodeControllers[0]);
        }
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            ReturnToLevelSelection();
        }
    }

    private void InitializeNode(LevelNodeUIController nodeController)
    {
        nodeController.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnNodeSelected(nodeController);
        });
    }

    private void SetCurrentNode(LevelNodeUIController newCurrentNode)
    {
        // Mark the previous current node as completed
        if (currentSelectedNode != null)
        {
            currentSelectedNode.SetAsCompletedNode();
        }

        // Update the current node
        currentSelectedNode = newCurrentNode;
        currentSelectedNode.SetAsCurrentNode();

        // Update node states based on the new current node
        UpdateNodeStates();
    }

    private void OnNodeSelected(LevelNodeUIController nodeController)
    {
        if (nodeController == null || !currentSelectedNode.levelNode.connectedNodes.Contains(nodeController.levelNode))
        {
            Debug.LogWarning("Selected node is not accessible.");
            return;
        }

        // Hide the level selection UI and activate the selected node's action
        uiLevelController.HideLevelSelection();
        ActivateNodeAction(nodeController.levelNode);

        // Update the current node to the selected node
        SetCurrentNode(nodeController);
    }

    private void UpdateNodeStates()
    {
        foreach (var nodeController in allNodeControllers)
        {
            if (nodeController == currentSelectedNode)
            {
                nodeController.SetAsCurrentNode();
            }
            else if (currentSelectedNode.levelNode.connectedNodes.Contains(nodeController.levelNode))
            {
                nodeController.SetAsAccessibleNode();
            }
            else if (nodeController.levelNode.connectedNodes.Count == 0)
            {
                nodeController.SetAsUnavailableNode();
            }
            else
            {
                nodeController.SetAsUnavailableNode(); // Default to unavailable state
            }
        }
    }

    public void ReturnToLevelSelection()
    {
        uiLevelController.ShowLevelSelection();
        UpdateNodeStates();
    }

    private void ActivateNodeAction(LevelNode levelNode)
    {
        switch (levelNode.nodeType)
        {
            case NodeType.Start:
                Debug.Log("Starting the journey!");
                break;
            case NodeType.Encounter:
                Debug.Log("Preparing for an encounter!");
                break;
            case NodeType.Boss:
                Debug.Log("Facing the boss!");
                break;
            case NodeType.Treasure:
                Debug.Log("Opening the treasure chest!");
                break;
            case NodeType.Mystery:
                Debug.Log("Exploring the mystery!");
                break;
            case NodeType.End:
                Debug.Log("Journey complete!");
                break;
            default:
                Debug.LogWarning($"Unhandled NodeType: {levelNode.nodeType}");
                break;
        }
    }
}
