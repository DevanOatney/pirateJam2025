using UnityEngine;
using UnityEngine.EventSystems;

public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public System.Action OnHoldStart;
    public System.Action OnHoldEnd;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnHoldStart?.Invoke(); // Trigger hold start
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnHoldEnd?.Invoke(); // Trigger hold end
    }
}