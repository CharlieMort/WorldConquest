using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GAME_STATE
{
    DRAFT,
    ATTACK,
    FORTIFY
}

public class GameMasterScript : MonoBehaviour
{
    public static GameMasterScript Instance { get; private set; }

    private GAME_STATE gameState = GAME_STATE.DRAFT;
    private int playersTurn = 0;
    private PlayerScript[] playerArr;
    private CountryScript[] countryArr;

    public SelectionHandler SelectionHandler;
    public TroopSelectScript TroopSelectScript;
    public DiceHandlerScript DiceHandlerScript;
    private GameSetupScript gameSetupScript;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        playerArr = transform.Find("Players").GetComponentsInChildren<PlayerScript>();
        List<CountryScript> temp = new List<CountryScript>();
        temp.AddRange(transform.Find("Continents").GetComponentsInChildren<CountryScript>());
        countryArr = temp.ToArray();
        print(countryArr.Length);

        SelectionHandler = GetComponent<SelectionHandler>();
        TroopSelectScript = GetComponent<TroopSelectScript>();
        gameSetupScript = GetComponent<GameSetupScript>();
    }

    public void GameStart()
    {
        gameSetupScript.AutoSetup(countryArr, playerArr.Length);
        if (ActionAfterTurnChange != null) ActionAfterTurnChange();
    }

    public CountryScript[] getAllCountries()
    {
        return countryArr;
    }

    public Color GetPlayerColor(int playerID) { return playerArr[playerID].playerColor; }

    public PlayerScript GetCurrentPlayer() { return playerArr[playersTurn]; }

    public GAME_STATE getGameState() { return gameState; }

    public int getPlayersTurn() {  return playersTurn; }

    public Action ActionAfterPhaseChange;
    public Action ActionBeforePhaseChange;
    public void NextPhaseMethod()
    {
        if (ActionBeforePhaseChange != null) ActionBeforePhaseChange();

        switch(gameState)
        {
            case GAME_STATE.DRAFT:
                gameState = GAME_STATE.ATTACK;
                break;
            case GAME_STATE.ATTACK:
                gameState = GAME_STATE.FORTIFY;
                break;
            case GAME_STATE.FORTIFY:
                gameState = GAME_STATE.DRAFT;
                NextTurn();
                break;
        }
        if (ActionAfterPhaseChange != null) ActionAfterPhaseChange();
    }

    public Action ActionAfterTurnChange;
    public void NextTurn()
    {
        playersTurn++;
        if (playersTurn == playerArr.Length) playersTurn = 0;

        if (ActionAfterTurnChange != null) ActionAfterTurnChange();
    }
}
