using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class TroopSelectScript : MonoBehaviour
{
    public Transform troopSelect;

    private Slider slider;
    private TextMeshProUGUI troopNum;

    private int troopMin = 1;
    private int troopMax = 10;
    private int troopCount = 0;

    private void Start()
    {
        slider = troopSelect.Find("Slider").GetComponent<Slider>();
        troopNum = troopSelect.Find("TroopCount").GetComponent<TextMeshProUGUI>();
    }

    // Shows the slider
    public void Show()
    {
        troopSelect.gameObject.SetActive(true);
        OnValueChange();
    }

    // Hides the slider
    public void Hide()
    {
        troopSelect.gameObject.SetActive(false);
    }

    // Sets the max and min amt of troops that the slider can slide between ??? if that makes sense
    public void SetTroopMinMax(int min, int max)
    {
        troopMin = min;
        troopMax = max;
    }

    // Triggered by unity slider when..... the value changes
    public void OnValueChange()
    {
        int num = Mathf.RoundToInt(troopMin + (slider.value - 0) * (troopMax - troopMin));
        troopNum.text = num.ToString();
        troopCount = num;
    }

    // Scripts subscribe to the confirm event so when the player confirms their selection the subscribers get notified with the amt of troops selected
    public Action<int> ActionAfterTroopSelectConfirm;
    public void Confirm()
    {
        Hide();
        if (ActionAfterTroopSelectConfirm != null)
        {
            ActionAfterTroopSelectConfirm(troopCount);
        }
    }

    // Haha sike i dont wanna select any troops loser
    public Action ActionAfterTroopSelectCancel;
    public void Cancel()
    {
        Hide();
        if (ActionAfterTroopSelectCancel != null) ActionAfterTroopSelectCancel();
    }
}
