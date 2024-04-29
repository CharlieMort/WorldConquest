using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSetupScript : MonoBehaviour
{
    public string allocationType = "random"; // or manual

    // Does a random setup
    public void AutoSetup(CountryScript[] countries, int playerCount)
    {
        int turn = 0;
        int maxTroopAlloc = -1;
        int[] playerTroopAlloc = new int[playerCount];
        switch(playerCount)
        {
            case 2:
                maxTroopAlloc = 40;
                break;
            case 3:
                maxTroopAlloc = 35;
                break;
            case 4:
                maxTroopAlloc = 30;
                break;
            case 5:
                maxTroopAlloc = 25;
                break;
            case 6:
                maxTroopAlloc = 20;
                break;
        }
        for (int i = 0; i<playerCount; i++)
        {
            playerTroopAlloc[i] = maxTroopAlloc;
        }
        foreach(CountryScript country in countries)
        {
            country.ChangeOwner(turn);
            country.AddTroops(1);
            playerTroopAlloc[turn]--;
            turn++;
            turn %= playerCount;
        }
        while(playerTroopAlloc.Sum() > 0)
        {
            int countryIdx = (int)Random.Range(0, countries.Length);
            CountryScript c = countries[countryIdx];
            while(c.ownerID != turn)
            {
                countryIdx = (int)Random.Range(0, countries.Length);
                c = countries[countryIdx];
            }
            c.AddTroops(1);
            playerTroopAlloc[turn]--;
            turn++;
            turn %= playerCount;
        }
    }
}
