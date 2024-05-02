using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class AIHandlerScript : MonoBehaviour
{
    public bool useAllAI = false;
    private DraftHandlerScript dhs;
    private AttackHandlerScript ahs;
    private FortifyHandlerScript fhs;
    private TroopSelectScript tss;
    public float delay = 0.5f;

    private CountryScript GetRandomOwnedCountry()
    {
        int currentPlayer = GameMasterScript.Instance.getPlayersTurn();
        List<CountryScript> countries = new List<CountryScript>();
        foreach(CountryScript c in GameMasterScript.Instance.getAllCountries())
        {
            if (c.ownerID == currentPlayer)
            {
                countries.Add(c);
            }
        }

        return countries[Random.Range(0, countries.Count)];
    }

    private CountryScript GetRandomAttackableOwnedCountry()
    {
        int currentPlayer = GameMasterScript.Instance.getPlayersTurn();
        List<CountryScript> countries = new List<CountryScript>();
        foreach (CountryScript c in GameMasterScript.Instance.getAllCountries())
        {
            if (c.ownerID == currentPlayer && c.troopCount >= 3)
            {
                countries.Add(c);
            }
        }
        if (countries.Count == 0) return null;
        return countries[Random.Range(0, countries.Count)];
    }

    private void Start()
    {
        dhs = GameMasterScript.Instance.GetComponent<DraftHandlerScript>();
        ahs = GameMasterScript.Instance.GetComponent<AttackHandlerScript>();
        fhs = GameMasterScript.Instance.GetComponent<FortifyHandlerScript>();
        tss = GameMasterScript.Instance.TroopSelectScript;
        
        if (useAllAI)
        {
            GameMasterScript.Instance.DiceHandlerScript.skipAnim = true;
            Invoke("GameStart", 2f);
            Invoke("Draft", 2.5f);
            GameMasterScript.Instance.ActionAfterPhaseChange += NextPhase;
        }
    }

    void GameStart()
    {
        dhs.popupDelay = delay / 2;
        GameMasterScript.Instance.GameStart();
    }

    void NextPhase()
    {
        switch(GameMasterScript.Instance.getGameState())
        {
            case GAME_STATE.DRAFT:
                Invoke("Draft", delay);
                break;
            case GAME_STATE.ATTACK:
                Invoke("Attack", delay);
                break;
            case GAME_STATE.FORTIFY:
                Invoke("Fortify", delay);
                break;
        }
    }

    void Draft()
    {
        print("DRAFT PHASE :" + GameMasterScript.Instance.getPlayersTurn());
        dhs.SelectCountry(GetRandomOwnedCountry().gameObject);
        dhs.AddTroops(dhs.remainingTroops);
        print(dhs.remainingTroops);
        tss.Hide();
        GameMasterScript.Instance.NextPhaseMethod();
    }

    void Attack()
    {
        print("ATTACK PHASE");
        int numOfAttacks = Random.Range(1, 10);
        for (int k = 0; k<numOfAttacks; k++)
        {
            CountryScript attackingCountry = null;
            CountryScript defendingCountry = null;
            int iCounter = 0;
            while (defendingCountry == null)
            {
                attackingCountry = GetRandomAttackableOwnedCountry();
                if (attackingCountry == null) break;
                for (int i = 0; i < attackingCountry.neighbourArr.Length; i++)
                {
                    if (attackingCountry.neighbourArr[i].ownerID != attackingCountry.ownerID)
                    {
                        defendingCountry = attackingCountry.neighbourArr[i];
                        break;
                    }
                }
                iCounter++;
                if (iCounter > 100)
                {
                    attackingCountry = null;
                    break;
                }
            }
            if (attackingCountry == null) break;
            ahs.SelectCountry(attackingCountry.gameObject);
            ahs.SelectCountry(defendingCountry.gameObject);
            ahs.Attack(2);
            if (ahs.movingTroops)
            {
                ahs.MoveTroopsToInvaded(attackingCountry.troopCount - 1);
            }
            ahs.CancelAttack();
        }

        GameMasterScript.Instance.NextPhaseMethod();
    }

    void Fortify()
    {
        print("FORIFY PHASE LOL");
        GameMasterScript.Instance.NextPhaseMethod();
    }
}
