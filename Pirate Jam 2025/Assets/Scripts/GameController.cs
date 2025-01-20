using UnityEngine;

public enum CanvasState { LevelSelect, TreasuryEvent, MysteryEvent, EncounterEvent }

public class GameController : MonoBehaviour
{
    public LevelSelectionController levelSelectionController;
    public GameObject treasuryEventCanvas;
    public GameObject mysteryEventCanvas;
    public GameObject combatEventCanvas;

    public void Start()
    {
        levelSelectionController.InitializeLevelSelectionScreen();
        SetCanvasStates(CanvasState.LevelSelect);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            SetCanvasStates(CanvasState.LevelSelect);
        }
    }

    public void SetCanvasStates(CanvasState newState)
    {
        switch (newState)
        {
            case CanvasState.LevelSelect:
                treasuryEventCanvas.SetActive(false);
                mysteryEventCanvas.SetActive(false);
                combatEventCanvas.SetActive(false);
                levelSelectionController.ReturnToLevelSelection();
                break;
            case CanvasState.TreasuryEvent:
                treasuryEventCanvas.SetActive(true);
                mysteryEventCanvas.SetActive(false);
                combatEventCanvas.SetActive(false);
                break;
            case CanvasState.MysteryEvent:
                treasuryEventCanvas.SetActive(false);
                mysteryEventCanvas.SetActive(true);
                combatEventCanvas.SetActive(false);
                break;
            case CanvasState.EncounterEvent:
                treasuryEventCanvas.SetActive(false);
                mysteryEventCanvas.SetActive(false);
                combatEventCanvas.SetActive(true);
                break;
        }
    }

}
