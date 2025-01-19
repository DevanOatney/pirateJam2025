using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

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

    public void Initialize(LevelNode node)
    {
        levelNode = node;
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
            colorTween = nodeImage.DOColor(highlightColor, 0.5f);
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
        ResetNodeState();
    }

    public void SelectNode()
    {
        currentTween = nodeTransform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1);
    }
}
