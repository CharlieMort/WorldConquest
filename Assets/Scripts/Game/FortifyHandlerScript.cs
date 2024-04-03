using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortifyHandlerScript : MonoBehaviour
{
    private int playersTurn;
    private List<List<CountryScript>> selectableCountries;
    private CountryScript fromCountry;
    private CountryScript toCountry;

    private void Start()
    {
        GameMasterScript.Instance.ActionAfterPhaseChange += BeginFortify;
    }

    public void BeginFortify()
    {
        if (GameMasterScript.Instance.getGameState() == GAME_STATE.FORTIFY)
        {
            /*
             * how do we highlight all connected countries (ive dreamt about this)
             * - firstly create a list with all the currents players countries called notvisited
             * - start at any country
             * - add that country to list inside the selectableCountries list (list sception)
             * - add the countrys neighbours to the list that are of the current players turn
             * - IF THE LIST IS OF LENGTH 1 (the country has no friends) then we remove it as you cant fortify to yourself LOSER
             * - do a cheeky depth first search
             * - set counter to the second ele 
             *     - get country at counter
             *     - add countrys neighbours to the list that arent already in the list
             *     - increment counter
             *     - if counter == length of list
             *     - no more to add and we can return
             * - REMOVE ALL VISITED COUNTRIES FROM notvisited
             * - repeat until notvisited is empty
             * then do fortify shit
            */

            GameMasterScript.Instance.SelectionHandler.ActionAfterCountrySelect1 += OnCountrySelect;

            selectableCountries = new List<List<CountryScript>>();
            List<CountryScript> notvisited = new List<CountryScript>();
            playersTurn = GameMasterScript.Instance.getPlayersTurn();

            foreach(CountryScript cs in GameMasterScript.Instance.getAllCountries())
            {
                if (cs.ownerID == playersTurn)
                {
                    notvisited.Add(cs);
                }
            }

            while(notvisited.Count > 0)
            {
                CountryScript cs = notvisited[0];
                notvisited.RemoveAt(0);

                List<CountryScript> connected = new List<CountryScript>() { cs };
                foreach (CountryScript neighbour in cs.neighbourArr)
                {
                    if (neighbour.ownerID == playersTurn)
                    {
                        connected.Add(neighbour);
                        notvisited.Remove(neighbour);
                    }
                }
                if (connected.Count != 1)
                {
                    int x = 1;
                    while (x < connected.Count)
                    {
                        foreach(CountryScript neighbour in connected[x].neighbourArr)
                        {
                            if (neighbour.ownerID == playersTurn && !connected.Contains(neighbour))
                            {
                                connected.Add(neighbour);
                                notvisited.Remove(neighbour);
                            }
                        }
                        x++;
                    }
                    selectableCountries.Add(connected);
                }
            }

            foreach(CountryScript cs in GameMasterScript.Instance.getAllCountries())
            {
                bool found = false;
                for (int i = 0; i<selectableCountries.Count; i++)
                {
                    if (selectableCountries[i].Contains(cs))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    if (cs.troopCount > 1)
                    {
                        cs.Highlight();
                    }
                    else
                    {
                        cs.Darken();
                    }
                }
                else
                {
                    cs.Darken();
                }
            }
        }
    }

    private List<CountryScript> path;
    public void OnCountrySelect(GameObject c)
    {
        CountryScript cs = c.GetComponent<CountryScript>();
        if (fromCountry == null)
        {
            bool found = false;
            for (int i = 0; i < selectableCountries.Count; i++)
            {
                if (selectableCountries[i].Contains(cs) && cs.troopCount > 1)
                {
                    found = true;
                    path = selectableCountries[i];
                    break;
                }
            }
            if (found)
            {
                fromCountry = cs;
                foreach (CountryScript x in GameMasterScript.Instance.getAllCountries())
                {
                    if (path.Contains(x) && x != cs)
                    {
                        x.Highlight();
                    }
                    else
                    {
                        x.Darken();
                    }
                }
            }
        }
        else
        {
            if (path.Contains(cs) && cs != fromCountry)
            {
                toCountry = cs;
                GameMasterScript.Instance.TroopSelectScript.SetTroopMinMax(1, fromCountry.troopCount - 1);
                GameMasterScript.Instance.TroopSelectScript.Show();
                GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectConfirm += MoveTroops;
            }
        }
    }

    public void MoveTroops(int num)
    {
        fromCountry.AddTroops(-num);
        toCountry.AddTroops(num);

        EndFortify();
    }

    public void CancelFortify()
    {
        fromCountry = null;
        toCountry = null;
        GameMasterScript.Instance.TroopSelectScript.Hide();
        GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectConfirm -= MoveTroops;
    }

    public void EndFortify()
    {
        CancelFortify();
        GameMasterScript.Instance.SelectionHandler.ActionAfterCountrySelect1 -= OnCountrySelect;
        GameMasterScript.Instance.NextPhaseMethod();
    }
}
