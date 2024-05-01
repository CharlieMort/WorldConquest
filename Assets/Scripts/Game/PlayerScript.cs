using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public string playerName;
    public Color playerColor;
    public GameObject playerUI;
    public Sprite playerSprite;

    private int[] cardsArr = new int[3] { 0, 0, 0 };

    private void Start()
    {
        playerUI.transform.Find("Background").GetComponent<Image>().color = playerColor;
    }

    public int[] GetCardsArr()
    {
        return cardsArr;
    }

    public void AddRandomCard()
    {
        cardsArr[Random.Range(0, cardsArr.Length)] += 1;
        GameMasterScript.Instance.CardHandlerScript.UpdateUI();
    }

    public void RemoveCardFrom(int idx, int amt)
    {
        cardsArr[idx] -= amt;
    }

    public int GetNumOfCards()
    {
        int count = 0;
        foreach (int i in cardsArr)
        {
            count += i;
        }
        return count;
    }
}
