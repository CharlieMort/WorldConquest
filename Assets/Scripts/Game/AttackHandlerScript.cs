using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackHandlerScript : MonoBehaviour
{
    private int playersTurn;
    private CountryScript attackingCountry = null;
    private CountryScript defendingCountry = null;
    private CountryScript[] attackableCountrys;

    private bool playerGetsCard = false;

    public GameObject attackUI;

    private void Start()
    {
        GameMasterScript.Instance.ActionAfterPhaseChange += BeginAttack;
        GameMasterScript.Instance.ActionBeforePhaseChange += EndAttack;
    }

    private void BeginAttack()
    {
        if (GameMasterScript.Instance.getGameState() == GAME_STATE.ATTACK)
        {
            playersTurn = GameMasterScript.Instance.getPlayersTurn();
            GameMasterScript.Instance.SelectionHandler.ActionAfterCountrySelect1 += SelectCountry;
            UpdateCountries();
            playerGetsCard = false;
        }
    }

    private void EndAttack()
    {
        if (GameMasterScript.Instance.getGameState() == GAME_STATE.ATTACK)
        {
            CancelAttack();
            GameMasterScript.Instance.SelectionHandler.ActionAfterCountrySelect1 -= SelectCountry;
        }
    }

    public void UpdateCountries()
    {
        foreach (CountryScript cs in GameMasterScript.Instance.getAllCountries())
        {
            if (cs.ownerID == playersTurn && cs.troopCount > 1)
            {
                cs.Highlight();
            }
            else cs.Darken();
        }
    }

    private void SelectCountry(GameObject country)
    {
        CountryScript cs = country.GetComponent<CountryScript>();
        if (attackingCountry == null)
        {
            if (cs.ownerID == playersTurn && cs.troopCount > 1)
            {
                attackingCountry = cs;
                attackableCountrys = cs.neighbourArr;
                foreach (CountryScript c in GameMasterScript.Instance.getAllCountries())
                {
                    if (c == cs || attackableCountrys.Contains(c))
                    {
                        c.Highlight();
                    }
                    else
                    {
                        c.Darken();
                    }
                }

                // DO THE ARROWS THAT SHOW WHICH COUNTRIES IT CAN ATTACK
            }
        }
        else
        {
            if (attackableCountrys.Contains(cs))
            {
                defendingCountry = cs;
                attackUI.SetActive(true);
                attackUI.transform.GetChild(0).gameObject.SetActive(true);
                attackUI.transform.GetChild(1).gameObject.SetActive(true);
                attackUI.transform.GetChild(2).gameObject.SetActive(true);

                // DO THE BIG ARROW THAT SIGNIFIES ITS ATTACKING ONE COUNTRY FROM ANOTHER

                switch (attackingCountry.troopCount)
                {
                    case 0:
                        Debug.LogError("COUNTRY SELECTED HAS ZERO TROOPS.... this shouldnt happen");
                        break;
                    case 1:
                        Debug.LogError("COUNTRY SELECTED HAS ONE TROOP.... this shouldnt happen");
                        break;
                    case 2:
                        attackUI.transform.GetChild(1).gameObject.SetActive(false);
                        attackUI.transform.GetChild(2).gameObject.SetActive(false);
                        break;
                    case 3:
                        attackUI.transform.GetChild(2).gameObject.SetActive(true);
                        break;
                }
            }
        }
    }

    public void Attack(int numOfDice)
    {
        if (defendingCountry != null)
        {
            List<int> attackDice = new List<int>();
            List<int> defendingDice = new List<int>();
            for (int i = 0; i < numOfDice; i++) { 
                attackDice.Add(Random.Range(1, 7));
            }
            for (int i = 0; i < (defendingCountry.troopCount >= 2 ? 2 : defendingCountry.troopCount); i++) { 
                defendingDice.Add(Random.Range(1, 7)); 
            }

            attackDice.Sort();
            attackDice.Reverse();
            defendingDice.Sort();
            defendingDice.Reverse();
            print("Rolling...");
            for (int i = 0; i < defendingDice.Count; i++)
            {
                if (defendingDice[i] >= attackDice[i])
                {
                    print("Defender Wins " + defendingDice[i] + " - " + attackDice[i]);
                    attackingCountry.AddTroops(-1);
                    switch(attackingCountry.troopCount)
                    {
                        case 1:
                            CancelAttack();
                            break;
                        case 2:
                            attackUI.transform.GetChild(1).gameObject.SetActive(false);
                            break;
                        case 3:
                            attackUI.transform.GetChild(2).gameObject.SetActive(true);
                            break;
                    }
                }
                else
                {
                    print("Attacker Wins " + attackDice[i] + " - " + defendingDice[i]);
                    defendingCountry.AddTroops(-1);
                    
                    // ATTACKER WON THE BATTLE
                    if (defendingCountry.troopCount <= 0)
                    {
                        attackUI.SetActive(false);
                        defendingCountry.ChangeOwner(attackingCountry.ownerID);
                        GameMasterScript.Instance.TroopSelectScript.SetTroopMinMax(numOfDice, attackingCountry.troopCount-1);
                        GameMasterScript.Instance.TroopSelectScript.Show();
                        GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectConfirm += MoveTroopsToInvaded;
                    }
                }
            }
        }
    }

    public void MoveTroopsToInvaded(int num)
    {
        attackingCountry.AddTroops(-num);
        defendingCountry.AddTroops(num);
        GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectConfirm -= MoveTroopsToInvaded;
        GameMasterScript.Instance.TroopSelectScript.Hide();
        playerGetsCard = true;
        CancelAttack();
        UpdateCountries();
    }

    public void CancelAttack()
    {
        attackingCountry = null;
        defendingCountry = null;
        attackUI.SetActive(false);
        UpdateCountries();
    }
}
