using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    public void Show()
    {
        troopSelect.gameObject.SetActive(true);
        OnValueChange();
    }

    public void Hide()
    {
        troopSelect.gameObject.SetActive(false);
    }

    public void SetTroopMinMax(int min, int max)
    {
        troopMin = min;
        troopMax = max;
    }

    public void OnValueChange()
    {
        int num = Mathf.RoundToInt(troopMin + (slider.value - 0) * (troopMax - troopMin));
        troopNum.text = num.ToString();
        troopCount = num;
    }

    public Action<int> ActionAfterTroopSelectConfirm;
    public void Confirm()
    {
        Hide();
        if (ActionAfterTroopSelectConfirm != null) ActionAfterTroopSelectConfirm(troopCount);
    }

    public Action ActionAfterTroopSelectCancel;
    public void Cancel()
    {
        Hide();
        if (ActionAfterTroopSelectCancel != null) ActionAfterTroopSelectCancel();
    }
}
