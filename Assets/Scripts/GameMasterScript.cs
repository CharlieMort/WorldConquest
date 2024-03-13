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
        Transform conts = transform.Find("Continents");
        for (int i = 0; i<conts.childCount; i++)
        {
            temp.AddRange(conts.GetComponentsInChildren<CountryScript>());
        }
        countryArr = temp.ToArray();

        SelectionHandler = GetComponent<SelectionHandler>();
        TroopSelectScript = GetComponent<TroopSelectScript>();
    }

    public void GameStart()
    {
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
    public void NextPhaseMethod()
    {
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
