using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DraftHandlerScript : MonoBehaviour
{
    private int TroopsAwarded = 5;
    private int remainingTroops;

    private int playerNum;

    private CountryScript selectedCountry;
    public Button nextPhaseButton;

    public Transform popup;
    private TextMeshProUGUI popupName;
    private TextMeshProUGUI popupDesc;
    private TextMeshProUGUI troopsLeftText;

    private void Start()
    {
        GameMasterScript.Instance.ActionAfterTurnChange += BeginDraft;
        popupName = popup.Find("PlayerTurn").GetComponent<TextMeshProUGUI>();
        popupDesc = popup.Find("TroopDesc").GetComponent<TextMeshProUGUI>();
        troopsLeftText = nextPhaseButton.transform.Find("TroopsLeft").GetComponent<TextMeshProUGUI>();
    }

    public void BeginDraft() 
    {
        playerNum = GameMasterScript.Instance.getPlayersTurn();

        // TODO: if player dead lol

        int cAmt = 0;
        foreach (CountryScript country in GameMasterScript.Instance.getAllCountries()) 
        {
            if (country.ownerID == playerNum)
            {
                country.Highlight();
                cAmt++;
            }
            else country.Darken();
        }

        int numOfCountries = Mathf.Max((int)Mathf.Floor(cAmt / 3), 3);
        TroopsAwarded = numOfCountries;
        // TODO: contintent bonus

        popupName.text = "Player " + (playerNum + 1) + "'s Turn";
        popupDesc.text = "+"+numOfCountries+" from owning "+cAmt+" countries";
        troopsLeftText.gameObject.SetActive(true);
        troopsLeftText.text = TroopsAwarded.ToString();
        nextPhaseButton.transform.GetChild(0).gameObject.SetActive(false);

        popup.gameObject.SetActive(true);
        StartCoroutine(popupClose());

        remainingTroops = TroopsAwarded;
        nextPhaseButton.interactable = false;

        GameMasterScript.Instance.SelectionHandler.ActionAfterCountrySelect1 += SelectCountry;
    }

    IEnumerator popupClose()
    {
        yield return new WaitForSeconds(3f);
        popup.gameObject.SetActive(false);
    }

    public void EndDraft()
    {
        GameMasterScript.Instance.SelectionHandler.ActionAfterCountrySelect1 -= SelectCountry;
        nextPhaseButton.interactable = true;
        troopsLeftText.gameObject.SetActive(false);
        nextPhaseButton.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void SelectCountry(GameObject country)
    {
        if (selectedCountry != null) return;

        if (country.GetComponent<CountryScript>().ownerID == playerNum)
        {
            selectedCountry = country.GetComponent<CountryScript>();
            GameMasterScript.Instance.TroopSelectScript.SetTroopMinMax(1, remainingTroops);
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
        troopsLeftText.text = remainingTroops.ToString();
        selectedCountry = null;
        
        if (remainingTroops == 0)
        {
            EndDraft();
        }
        GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectConfirm -= AddTroops;
    }
}
