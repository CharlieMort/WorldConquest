using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackHandlerScript : MonoBehaviour
{
    private int playersTurn; // Current players turn IDX
    private CountryScript attackingCountry = null;
    private CountryScript defendingCountry = null;
    private CountryScript[] attackableCountrys; // Used to store countries that can be attacked

    private bool playerGetsCard = false; // TODO

    public GameObject attackUI;

    int aDiceNum = 0; // Amount of attacker dice chosen

    private void Start()
    {
        // Subscribe to events (makes sure to like and hit that notification bell)
        GameMasterScript.Instance.ActionAfterPhaseChange += BeginAttack;
        GameMasterScript.Instance.ActionBeforePhaseChange += EndAttack;
    }

    // Called everytime the phase changes
    // Checks if the phase is the attack phase and if so sets up everything including whos attack phase it is and highlighting their countries
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

    // Called at the end of the whole attack phase
    // Triggers the next phase event and resets all the variables ready for the next time it comes around
    // Also un-subscribes from the troop selector because fortify will use it next
    private void EndAttack()
    {
        if (GameMasterScript.Instance.getGameState() == GAME_STATE.ATTACK)
        {
            CancelAttack();
            GameMasterScript.Instance.SelectionHandler.ActionAfterCountrySelect1 -= SelectCountry;
        }
    }

    // Highlights all countries are both the attackers countries and have more than 1 troop
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

    // Called by the event click (everytime a country is clicked)
    // If allowed to be clicked then if its the first to be selected this means its the attacking country
    // - highlight its enemy neighbours
    // If attacking country already selected it must be a defending country
    // - show the dice depending on how many troops are in both attacking and defending coutnries
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
                    if (c == cs || attackableCountrys.Contains(c) && c.ownerID != playersTurn)
                    {
                        c.Highlight();
                    }
                    else
                    {
                        c.Darken();
                    }
                }

                // TODO: THE ARROWS THAT SHOW WHICH COUNTRIES IT CAN ATTACK
            }
        }
        else
        {
            if (attackableCountrys.Contains(cs) && cs.ownerID != playersTurn)
            {
                defendingCountry = cs;
                attackUI.SetActive(true);
                attackUI.transform.GetChild(0).gameObject.SetActive(true);
                attackUI.transform.GetChild(1).gameObject.SetActive(true);
                attackUI.transform.GetChild(2).gameObject.SetActive(true);

                // TODO: THE BIG ARROW THAT SIGNIFIES ITS ATTACKING ONE COUNTRY FROM ANOTHER

                ShowUI();
            }
        }
    }

    // Toggles the UI elements depedning on how many troops are in the attacking country
    void ShowUI()
    {
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

    // Starts the attack
    // Can be used by AI who dont click as they dont have hands... yet
    // Sets the number of dice - triggers them to be rolled - then waits for outcome
    // Improvements can be made to skip animation in need of time or performance
    public void Attack(int numOfDice)
    {
        aDiceNum = numOfDice;
        int dDiceNum = defendingCountry.troopCount;
        if (dDiceNum > 1) dDiceNum = 2;
        else dDiceNum = 1;
        GameMasterScript.Instance.DiceHandlerScript.ThrowDice(numOfDice, dDiceNum);
        GameMasterScript.Instance.DiceHandlerScript.ActionAfterDiceRoll += HandleAttack;
        foreach(CountryScript cunt in attackableCountrys) // CoUNTry
        {
            if (cunt != defendingCountry)
            {
                cunt.Darken();
            }
        }
        attackUI.SetActive(false);
    }

    // Triggers after the dice roll animation has concluded
    // Parses the outcomes array into acutal effects eg removes troops from countries that won/lost and updates the map
    public void HandleAttack(int[] outcomes)
    {
        attackUI.SetActive(true);
        GameMasterScript.Instance.DiceHandlerScript.ActionAfterDiceRoll -= HandleAttack;
        foreach(int outcome in outcomes)
        {
            if (outcome == 0) // Attacker wins that roll
            {
                defendingCountry.AddTroops(-1);
            }
            else if (outcome == 1) // Defender wins
            {
                attackingCountry.AddTroops(-1);
                ShowUI();
            }
        }
        if (defendingCountry.troopCount <= 0) // Attacker won the battle
        {
            attackUI.SetActive(false);
            defendingCountry.ChangeOwner(attackingCountry.ownerID);
            GameMasterScript.Instance.TroopSelectScript.SetTroopMinMax(aDiceNum, attackingCountry.troopCount - 1);
            GameMasterScript.Instance.TroopSelectScript.Show();
            GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectConfirm += MoveTroopsToInvaded;
        }

        if (attackingCountry.troopCount <= 1) // Attacker cant continue the battle
        {
            attackUI.SetActive(false);
            //attackingCountry.ChangeOwner(defendingCountry.ownerID);
            //GameMasterScript.Instance.TroopSelectScript.SetTroopMinMax(aDiceNum, attackingCountry.troopCount + 1);
            CancelAttack();
        }
    }

    // Easy method to move x amount of troops that has been recently conquered
    public void MoveTroopsToInvaded(int num)
    {
        attackingCountry.AddTroops(-num);
        defendingCountry.AddTroops(num);

        defendingCountry.transform.parent.GetComponent<ContinentScript>().CheckForOwner();
        GameMasterScript.Instance.TroopSelectScript.ActionAfterTroopSelectConfirm -= MoveTroopsToInvaded;
        GameMasterScript.Instance.TroopSelectScript.Hide();
        playerGetsCard = true;
        CancelAttack();
        UpdateCountries();
    }

    // Cancels the attack and resets the variables
    public void CancelAttack()
    {
        attackingCountry = null;
        defendingCountry = null;
        attackUI.SetActive(false);
        UpdateCountries();
    }
}
