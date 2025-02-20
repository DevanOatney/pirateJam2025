using UnityEngine;
using UnityEngine.UI;

public class FadeInUi : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public GameObject fadeTarget;

    // Update is called once per frame
    void Update()
    {
        // fadeTarget.color = new Color(fadeTarget.color.r, fadeTarget.color.g, fadeTarget.color.b, );
    }

    public float fadeInTime;
    public bool hasFadedIn;

    public void FadeIn()
    {
        fadeTarget.SetActive(false);
    }
}
