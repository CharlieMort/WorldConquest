using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CountryScript : MonoBehaviour
{
    public string countryName; // place name
    public int troopCount = 0; // how many troops are currently on the country
    public int ownerID = -1; // who owns the country
    public CountryScript[] neighbourArr = new CountryScript[0]; // List of all this country's neighbour -- has to be updated manually in editor

    private TextMeshProUGUI nameTextObject;
    private TextMeshProUGUI troopCountText;
    private Material countryMat;

    private void Start()
    {
        nameTextObject = transform.Find("Canvas").Find("CountryName").GetComponent<TextMeshProUGUI>();
        nameTextObject.text = countryName;

        troopCountText = transform.Find("Canvas").Find("TroopCount").GetChild(0).GetComponent<TextMeshProUGUI>();
        AddTroops(Mathf.FloorToInt(Random.Range(1f, 10f)));

        countryMat = transform.Find("CountryModel").GetComponent<MeshRenderer>().material;
        if (ownerID != -1) ChangeOwner(ownerID);
    }

    public void ChangeOwner(int playerID)
    {
        ownerID = playerID;
        countryMat.color = GameMasterScript.Instance.GetPlayerColor(playerID);
    }

    public void AddTroops(int num)
    {
        troopCount += num;
        troopCountText.text = troopCount.ToString();
    }

    public void Highlight()
    {
        countryMat.SetColor("_EmissionColor", countryMat.color);
    }

    public void Darken()
    {
        countryMat.color = GameMasterScript.Instance.GetPlayerColor(ownerID);
        countryMat.SetColor("_EmissionColor", countryMat.color * -.5f);
    }
}
