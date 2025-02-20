using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionController : MonoBehaviour
{
    [SerializeField] private UILevelController uiLevelController;
    [SerializeField] private Transform levelNodesParent;
    [SerializeField] private GameController gameController;

    private LevelNodeUIController currentSelectedNode;
    private List<LevelNodeUIController> allNodeControllers = new List<LevelNodeUIController>();
    private List<LevelNodeUIController> completedNodes = new List<LevelNodeUIController>();

    public void InitializeLevelSelectionScreen()
    {
        uiLevelController.InitializeLevelSelectionUI();
    }

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
            case NodeType.Treasure:
                ShowTreasuryUI(levelNode.treasuryData);
                break;
            case NodeType.Mystery:
                ShowMysteryUI(levelNode.mysteryData);
                break;
            case NodeType.Encounter:
            case NodeType.Elite:
            case NodeType.Boss:
                StartEncounter(levelNode.combatData);
                break;
            default:
                Debug.LogWarning($"Unhandled NodeType: {levelNode.nodeType}");
                break;
        }
    }

    private void ShowTreasuryUI(TreasuryData treasuryData)
    {
        gameController.SetCanvasStates(CanvasState.TreasuryEvent);
    }

    private void ShowMysteryUI(MysteryData mysteryData)
    {
        gameController.SetCanvasStates(CanvasState.MysteryEvent);
    }

    private void StartEncounter(EncounterData combatData)
    {
        gameController.SetCanvasStates(CanvasState.EncounterEvent);
    }

}
