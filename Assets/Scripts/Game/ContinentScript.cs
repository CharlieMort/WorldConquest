using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentScript : MonoBehaviour
{
    public string continentName;
    public CountryScript[] countryArr = new CountryScript[0];
    public int ownerID = -1;
    public int troopBonus = 0;

    private void Start()
    {
        countryArr = transform.GetComponentsInChildren<CountryScript>();
    }

    // Check For New Owner Method
    // - Executes once after every time a new country has been captured
    // - Updates the owner status of the continent
    // - TODO: make it highlight the continent in the owners color to signal what color player owns it
    public void CheckForOwner()
    {
        int currentOwner = countryArr[1].ownerID;
        for (int i = 1; i < countryArr.Length; i++)
        {
            // if someone else owns a country then whole continent hasn't been captured
            if (countryArr[i].ownerID != currentOwner) ownerID = -1;
        }
        ownerID = currentOwner;
    }

    // Give Troop Bonus Method
    // - Executes at the start of any players draft phase
    // - If the player whos turn it is currently, owns the territory then add the appropriate troops
    public int GiveTroopBonus(int playerID)
    {
        if (playerID == ownerID) return troopBonus;
        return 0;
    }
}
