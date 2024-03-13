using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhaseInfoScript : MonoBehaviour
{
    Image draftPanel;
    Image attackPanel;
    Image fortifyPanel;

    public GameObject PlayerIcons;

    private void Start()
    {
        GameMasterScript.Instance.ActionAfterPhaseChange += PhaseChange;
        GameMasterScript.Instance.ActionAfterTurnChange += TurnChange;

        draftPanel = transform.Find("Draft").GetComponent<Image>();
        attackPanel = transform.Find("Attack").GetComponent<Image>();
        fortifyPanel = transform.Find("Fortify").GetComponent<Image>();

        PhaseChange();
        TurnChange();
    }

    private void PhaseChange()
    {
        ResetUI();
        Color playerColor = GameMasterScript.Instance.GetPlayerColor(GameMasterScript.Instance.getPlayersTurn());
        transform.Find("Background").GetComponent<Image>().color = playerColor;
        transform.Find("NextPhase").GetComponent<Image>().color = playerColor;
        transform.Find("NextPhase").GetChild(0).GetComponent<Image>().color = playerColor;
        switch(GameMasterScript.Instance.getGameState())
        {
            case GAME_STATE.DRAFT:
                draftPanel.color = playerColor;
                transform.Find("PhaseInfo").GetComponent<TextMeshProUGUI>().text = "DRAFT";
                break;
            case GAME_STATE.ATTACK:
                attackPanel.color = playerColor;
                transform.Find("PhaseInfo").GetComponent<TextMeshProUGUI>().text = "ATTACK";
                break;
            case GAME_STATE.FORTIFY:
                fortifyPanel.color = playerColor;
                transform.Find("PhaseInfo").GetComponent<TextMeshProUGUI>().text = "FORTIFY";
                break;
        }
    }

    private void ResetUI()
    {
        draftPanel.color = Color.white;
        attackPanel.color = Color.white;
        fortifyPanel.color = Color.white;
    }

    private void TurnChange()
    {
        for (int i = 0; i<PlayerIcons.transform.childCount; i++)
        {
            PlayerIcons.transform.GetChild(i).localPosition = new Vector3(
                    0,
                    PlayerIcons.transform.GetChild(i).localPosition.y,
                    PlayerIcons.transform.GetChild(i).localPosition.z
                );
        }

        Transform currPlayer = PlayerIcons.transform.GetChild(GameMasterScript.Instance.getPlayersTurn());
        PlayerIcons.transform.GetChild(PlayerIcons.transform.childCount - 1).localPosition = currPlayer.localPosition + Vector3.right*25;
        currPlayer.localPosition = new Vector3(
                -50,
                currPlayer.localPosition.y,
                currPlayer.localPosition.z
            );

        transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = GameMasterScript.Instance.GetCurrentPlayer().playerName;
        transform.Find("PlayerIcon").GetChild(0).GetComponent<Image>().sprite = GameMasterScript.Instance.GetCurrentPlayer().playerSprite;
    }
}
