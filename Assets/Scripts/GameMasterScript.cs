using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_STATE
{
    DRAFT,
    ATTACK,
    FORTIFY
}

public class GameMasterScript : MonoBehaviour
{
    private GAME_STATE gameState = GAME_STATE.DRAFT;
    private int playersTurn = 0;
    private PlayerScript[] playerArr;

    private void Start()
    {
        playerArr = transform.Find("Players").GetComponentsInChildren<PlayerScript>();
    }

    public Material GetPlayerMat(int playerID)
    {
        return playerArr[playerID].playerMat;
    }
}
