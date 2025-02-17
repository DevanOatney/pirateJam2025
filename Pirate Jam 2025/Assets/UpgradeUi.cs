using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUi : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject ui;
    public PauseUi pause;
    public Player player;

    public List<Mod> modsToDisplay;
    private bool hasSelected;

    public void OnPowerClicked()
    {
        if (hasSelected)
        {
            return;
        }

        player.powerPoints++;
        player.attackDamageMult += 0.1f;
    }

    public void OnHealthClicked()
    {
        if (hasSelected)
        {
            return;
        }

        player.defensePoints++;
        player.healthMult += 0.1f;
    }

    public void OnSpeedClicked()
    {
        if (hasSelected)
        {
            return;
        }

        hasSelected = true;
        player.speedPoints++;

        player.moveSpeedMult += 0.05f;
        player.attackSpeedMult += 0.05f;
    }

    public void OnUpgradeReceived()
    {
        pause.canShowUi = true;
        player.canPause = false;
        pause.PauseStart();
        
    }

    public void OnUpgradeSelected()
    {
        ui.SetActive(false);
        pause.PauseStop();
        pause.canShowUi = true;
    }
}
