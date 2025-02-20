using UnityEngine;
using UnityEngine.Events;

public class RewardProp : MonoBehaviour
{
    private bool hasClaimed;
    public UnityEvent UpgradeRequested;

    private void OnTriggerEnter(Collider collision)
    {
        Entity target = collision.GetComponent<Player>();

        if (target != null && !hasClaimed)
        {
            UpgradeRequested.Invoke();
        }
    }
}
