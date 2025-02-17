using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class LevelNodeUIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public LevelNode levelNode;
    [SerializeField] private RectTransform nodeTransform;
    [SerializeField] private Image nodeImage;
    [SerializeField] private Text nodeLabel;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color completedColor;
    [SerializeField] private Color unavailableColor;
    private Tween currentTween;
    private Tween colorTween;

    private bool isAccessible;
    private bool isCurrent;
    private bool isCompleted;
    private LevelSelectTooltipController tooltipController;

    public void Initialize(LevelNode node, LevelSelectTooltipController ttController)
    {
        levelNode = node;
        tooltipController = ttController;
    }

    private void ResetNodeState()
    {
        currentTween?.Kill();
        colorTween?.Kill();
        nodeTransform.localScale = Vector3.one;

        if (nodeImage != null)
        {
            nodeImage.color = isCompleted ? completedColor : (isAccessible ? defaultColor : unavailableColor);
        }

        if (nodeLabel != null)
        {
            nodeLabel.color = Color.white;
        }
    }

    public void SetAsCurrentNode()
    {
        ResetNodeState();

        if (nodeImage != null)
        {
            colorTween?.Kill();
            colorTween = nodeImage.DOColor(completedColor, 0.5f);
        }

        currentTween = nodeTransform.DOScale(Vector3.one * 1.2f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void SetAsCompletedNode()
    {
        isCompleted = true;
        isCurrent = false;
        isAccessible = false;
        ResetNodeState();
    }

    public void SetAsAccessibleNode()
    {
        isAccessible = true;
        isCurrent = false;
        isCompleted = false;
        ResetNodeState();
    }

    public void SetAsUnavailableNode()
    {
        isAccessible = false;
        isCurrent = false;
        isCompleted = false;
        ResetNodeState();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipController != null)
        {
            tooltipController.gameObject.SetActive(true);
            string tooltipText = levelNode.nodeType.ToString();
            switch (levelNode.nodeType)
            {
                case NodeType.Start:
                    tooltipText = "Encounter";
                    break;
                case NodeType.Elite:
                    tooltipText = "Elite";
                    break;
                case NodeType.End:
                    tooltipText = "Boss";
                    break;
            }
            tooltipController.text.text = tooltipText;
        }
        if (!isAccessible) return;

        currentTween?.Kill();
        currentTween = nodeTransform.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutQuad);

        if (nodeImage != null)
        {
            colorTween?.Kill();
            colorTween = nodeImage.DOColor(highlightColor, 0.2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipController != null)
        {
            tooltipController.gameObject.SetActive(false);
            tooltipController.text.text = "";
        }

        ResetNodeState();
    }

    public void SelectNode()
    {
        currentTween = nodeTransform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1);
    }
}