using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionController : MonoBehaviour
{
    [SerializeField] private UILevelController uiLevelController;
    [SerializeField] private Transform levelNodesParent;

    private LevelNodeUIController currentSelectedNode;
    private List<LevelNodeUIController> allNodeControllers = new List<LevelNodeUIController>();
    private List<LevelNodeUIController> completedNodes = new List<LevelNodeUIController>();


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

        if (allNodeControllers.Count > 0)
        {
            var firstNode = allNodeControllers[0];
            completedNodes.Add(firstNode);
            firstNode.SetAsCompletedNode();
            SetCurrentNode(firstNode);
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
        if (currentSelectedNode != null && !completedNodes.Contains(currentSelectedNode))
        {
            completedNodes.Add(currentSelectedNode);
            currentSelectedNode.SetAsCompletedNode();
        }

        currentSelectedNode = newCurrentNode;
        currentSelectedNode.SetAsCurrentNode();

        UpdateNodeStates();
    }



    private void OnNodeSelected(LevelNodeUIController nodeController)
    {
        if (!currentSelectedNode.levelNode.connectedNodes.Contains(nodeController.levelNode))
        {
            Debug.LogWarning("Selected node is not accessible.");
            return;
        }

        uiLevelController.HideLevelSelection();
        ActivateNodeAction(nodeController.levelNode);

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
            else if (completedNodes.Contains(nodeController))
            {
                nodeController.SetAsCompletedNode();
            }
            else if (currentSelectedNode.levelNode.connectedNodes.Contains(nodeController.levelNode))
            {
                nodeController.SetAsAccessibleNode();
            }
            else
            {
                nodeController.SetAsUnavailableNode();
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
