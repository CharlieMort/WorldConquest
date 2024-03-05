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

    private void Start()
    {
        nameTextObject = transform.Find("Canvas").Find("CountryName").GetComponent<TextMeshProUGUI>();
        nameTextObject.text = countryName;
    }

    private void Update()
    {
        // TESTING
        if (ownerID != -1)
        {
            transform.Find("CountryModel").GetComponent<MeshRenderer>().material = transform.root.GetComponent<GameMasterScript>().GetPlayerMat(ownerID);
        }
    }

    public void ChangeOwner(int playerID)
    {
        ownerID = playerID;
        transform.Find("CountryModel").GetComponent<MeshRenderer>().material = transform.root.GetComponent<GameMasterScript>().GetPlayerMat(ownerID);
    }
}
