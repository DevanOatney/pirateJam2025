using UnityEngine;

public class HealthUi : MonoBehaviour
{
    public GameObject hud;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPauseStarted()
    {
        hud.SetActive(false);
    }

    public void OnPauseEnded()
    {
        hud.SetActive(true);
    }
}
