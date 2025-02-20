using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseUi : MonoBehaviour
{
    public GameObject ui;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ui.SetActive(false);
    }

    public UnityEvent PauseStarted;
    public UnityEvent PauseEnded;

    public bool canShowUi;

    private bool currentPause;

    public void TogglePause()
    {
        currentPause = !currentPause;

        if (currentPause)
        {
            
            ui.SetActive(true);
            PauseStarted.Invoke();
            Time.timeScale = 0;
        }
        else
        {
            
            ui.SetActive(false);
            PauseEnded.Invoke();
            Time.timeScale = 1;
        }
    }

    public void PauseStart()
    {
        if (currentPause)
        {
            return;
        }

        currentPause = true;
        ui.SetActive(canShowUi);
        PauseStarted.Invoke();
        Time.timeScale = 0;
    }

    public void PauseStop()
    {
        if (currentPause)
        {
            return;
        }

        currentPause = true;
        ui.SetActive(false);
        PauseStarted.Invoke();
        Time.timeScale = 0;
    }

    public void OnResumeClicked()
    {
        TogglePause();
    }

    public void OnQuitClicked()
    {
        SceneManager.LoadScene("Boot");
        // Application.Quit();
    }
}
