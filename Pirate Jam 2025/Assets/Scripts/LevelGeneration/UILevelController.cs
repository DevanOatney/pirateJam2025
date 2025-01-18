using System.Collections.Generic;
using UnityEngine;

public class UILevelController : MonoBehaviour
{
    [Header("References")]
    public LevelGenerator levelGenerator;
    public RectTransform canvasRoot; 
    public GameObject uiNodePrefab; 
    public float uiNodeSpacing = 100f;
    public float stageSpacing = 200f; 

    [Header("UI Hierarchy")]
    public Transform nodesParent; 
    public Transform linesParent; 

    private List<List<GameObject>> uiStages = new List<List<GameObject>>();

    public void SetupLevelUI()
    {
        foreach (Transform child in nodesParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in linesParent)
        {
            Destroy(child.gameObject);
        }
        uiStages.Clear();

        var stages = levelGenerator.GetStages();
        if (stages == null || stages.Count == 0)
        {
            levelGenerator.GenerateLevel();
            Debug.LogError("Had to force generate the level.");
            stages = levelGenerator.GetStages();
        }

        foreach (var stage in stages)
        {
            float stageHeight = (stage.Count - 1) * uiNodeSpacing;

            List<GameObject> uiStage = new List<GameObject>();

            foreach (var node in stage)
            {
                GameObject nodeUI = Instantiate(uiNodePrefab, nodesParent);
                nodeUI.name = $"{node.nodeType} Node";

                LevelNodeUIController uiController = nodeUI.GetComponent<LevelNodeUIController>();

                if (uiController != null)
                {
                    uiController.Initialize(node, canvasRoot.GetComponent<Canvas>()); 
                }

                RectTransform rectTransform = nodeUI.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    float canvasWidth = canvasRoot.rect.width;
                    float canvasHeight = canvasRoot.rect.height;

                    Vector2 centerOffset = new Vector2(canvasWidth / 2, canvasHeight / 2);

                    rectTransform.anchoredPosition = new Vector2(
                        node.position.x * stageSpacing,
                        node.position.y * uiNodeSpacing + centerOffset.y
                    ) - centerOffset;
                }

                uiStage.Add(nodeUI);

                UpdateNodeAppearance(nodeUI, node.nodeType);
            }

            uiStages.Add(uiStage);
        }

        DrawConnections(uiStages);
    }

    private void DrawConnections(List<List<GameObject>> uiStages)
    {
        for (int i = 0; i < uiStages.Count - 1; i++)
        {
            List<GameObject> currentStage = uiStages[i];
            List<GameObject> nextStage = uiStages[i + 1];

            foreach (GameObject currentNodeUI in currentStage)
            {
                LevelNodeUIController currentController = currentNodeUI.GetComponent<LevelNodeUIController>();
                if (currentController == null || currentController.levelNode == null)
                    continue;

                foreach (LevelNode connectedNode in currentController.levelNode.connectedNodes)
                {
                    GameObject nextNodeUI = nextStage.Find(nodeUI =>
                    {
                        var uiController = nodeUI.GetComponent<LevelNodeUIController>();
                        return uiController != null && uiController.levelNode == connectedNode;
                    });

                    if (nextNodeUI == null) continue;

                    CreateConnectionLine(
                        currentNodeUI.GetComponent<RectTransform>().anchoredPosition,
                        nextNodeUI.GetComponent<RectTransform>().anchoredPosition
                    );
                }
            }
        }
    }

    private void CreateConnectionLine(Vector2 startPos, Vector2 endPos)
    {
        GameObject line = new GameObject("ConnectionLine", typeof(RectTransform));
        line.transform.SetParent(linesParent, false); 

        var image = line.AddComponent<UnityEngine.UI.Image>();
        image.color = Color.gray;

        RectTransform rectTransform = line.GetComponent<RectTransform>();
        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;

        rectTransform.sizeDelta = new Vector2(distance, 5f);
        rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f); 
        rectTransform.pivot = new Vector2(0f, 0.5f); 
        rectTransform.anchoredPosition = startPos;
        rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }



    public void Start()
    {
        SetupLevelUI();
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            levelGenerator.GenerateLevel();
            SetupLevelUI();
        }
    }

    private void UpdateNodeAppearance(GameObject nodeUI, NodeType nodeType)
    {
        var image = nodeUI.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            switch (nodeType)
            {
                case NodeType.Start:
                    image.color = Color.blue;
                    break;
                case NodeType.Encounter:
                    image.color = Color.red;
                    break;
                case NodeType.Boss:
                    image.color = Color.black;
                    break;
                case NodeType.Treasure:
                    image.color = Color.yellow;
                    break;
                case NodeType.Mystery:
                    image.color = Color.magenta;
                    break;
                case NodeType.End:
                    image.color = Color.green;
                    break;
                default:
                    image.color = Color.white;
                    break;
            }
        }
    }
}
