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
    }

    public Material GetPlayerMat(int playerID) { return playerArr[playerID].playerMat; }

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
