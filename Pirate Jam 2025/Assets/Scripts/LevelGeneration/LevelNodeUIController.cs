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

    public void Initialize(LevelNode node, Canvas _rootCanvas)
    {
        levelNode = node;
    }

    private void Start()
    {
        ResetNode();
    }

    public void SetAsCurrentNode()
    {
        ResetNode();
        currentTween = nodeTransform.DOScale(Vector3.one * 1.2f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ResetNode();
        currentTween = nodeTransform.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutQuad);

        if (nodeImage != null)
        {
            colorTween?.Kill();
            colorTween = nodeImage.DOColor(highlightColor, 0.2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetNode();
    }

    public void SelectNode()
    {
        ResetNode();
        currentTween = nodeTransform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1);
    }

    public void SetAsCompletedNode()
    {
        ResetNode();
        if (nodeImage != null)
        {
            colorTween?.Kill();
            colorTween = nodeImage.DOColor(completedColor, 0.5f);
        }
    }

    public void SetAsUnavailableNode()
    {
        ResetNode();
        currentTween = nodeTransform.DOShakePosition(0.5f, strength: new Vector3(10f, 0, 0), vibrato: 10, randomness: 90, snapping: false, fadeOut: true);

        if (nodeImage != null)
        {
            colorTween?.Kill();
            colorTween = nodeImage.DOColor(unavailableColor, 0.5f);
        }
    }

    private void ResetNode()
    {
        currentTween?.Kill();
        colorTween?.Kill();
        nodeTransform.localScale = Vector3.one;

        if (nodeImage != null)
        {
            nodeImage.color = defaultColor;
        }

        if (nodeLabel != null)
        {
            nodeLabel.color = Color.white;
        }
    }

    public void SetAsAccessibleNode()
    {
        ResetNode();

        if (nodeImage != null)
        {
            colorTween?.Kill();
            colorTween = nodeImage.DOColor(highlightColor, 0.5f);
        }

        currentTween = nodeTransform.DOScale(Vector3.one * 1.1f, 0.2f)
            .SetEase(Ease.OutQuad);
    }

}
