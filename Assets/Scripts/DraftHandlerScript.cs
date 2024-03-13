using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftHandlerScript : MonoBehaviour
{
    private int TroopsAwarded = 5;
    private int remainingTroops;

    private int playerNum;

    private CountryScript selectedCountry;
    public Button nextPhaseButton;

    private void Start()
    {
        GameMasterScript.Instance.ActionAfterTurnChange += BeginDraft;
    }

    public void BeginDraft() 
    {
        remainingTroops = TroopsAwarded;
        playerNum = GameMasterScript.Instance.getPlayersTurn();
        nextPhaseButton.interactable = false;

        GameMasterScript.Instance.SelectionHandler.ActionAfterCountrySelect1 += SelectCountry;

        foreach (CountryScript country in GameMasterScript.Instance.getAllCountries()) 
        {
            if (country.ownerID == playerNum) country.Highlight();
            else country.Darken();
        }
    }

    public void EndDraft()
    {
        GameMasterScript.Instance.SelectionHandler.ActionAfterCountrySelect1 -= SelectCountry;
        nextPhaseButton.interactable = true;
    }

    public void SelectCountry(GameObject country)
    {
        if (selectedCountry != null) return;

        if (country.GetComponent<CountryScript>().ownerID == playerNum)
        {
            selectedCountry = country.GetComponent<CountryScript>();
            GameMasterScript.Instance.TroopSelectScript.SetTroopMinMax(0, remainingTroops);
            GameMasterScript.Instance.TroopSelectScript.Show();
            GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectConfirm += AddTroops;
            GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectCancel += Cancel;
        }
        GameMasterScript.Instance.SelectionHandler.ClearSelect1();
    }

    public void Cancel()
    {
        selectedCountry = null;
        GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectConfirm -= AddTroops;
        GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectCancel -= Cancel;
    }

    public void AddTroops(int num)
    {
        remainingTroops -= num;
        selectedCountry.AddTroops(num);
        selectedCountry = null;
        
        if (remainingTroops == 0)
        {
            EndDraft();
        }
        GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectConfirm -= AddTroops;
    }
}
