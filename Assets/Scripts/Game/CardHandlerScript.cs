using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHandlerScript : MonoBehaviour
{
    public GameObject cards;
    public TextMeshProUGUI iCardText; // Infantry Cards Text
    public TextMeshProUGUI cCardText; // Cavalry Cards Text
    public TextMeshProUGUI aCardText; // Artillery Cards Text
    public Sprite[] cardSprites;

    public GameObject toggleButton;

    private int numOfRedeems = 0;
    private int cardBonus = 4;
    private bool canRedeem = false;

    public void ShowGUI()
    {
        if (GameMasterScript.Instance.getGameState() == GAME_STATE.DRAFT)
        {
            toggleButton.transform.GetChild(0).gameObject.SetActive(false);
            toggleButton.transform.GetChild(1).gameObject.SetActive(true);
            gameObject.SetActive(true);
            foreach (Image card in cards.GetComponentsInChildren<Image>())
            {
                card.color = Color.black;
            }
            PlayerScript player = GameMasterScript.Instance.GetCurrentPlayer();
            int[] cardArr = player.GetCardsArr();
            iCardText.text = cardArr[0].ToString();
            cCardText.text = cardArr[1].ToString();
            aCardText.text = cardArr[2].ToString();

            canRedeem = false;
            if (cardArr[0] > 0 && cardArr[1] > 0 && cardArr[2] > 0)
            {
                canRedeem = true;
                Image[] cardImgs = cards.GetComponentsInChildren<Image>();
                for (int i = 0; i < cardArr.Length; i++)
                {
                    cardImgs[i].sprite = cardSprites[i];
                }
            }
            else
            {
                for (int i = 0; i < cardArr.Length; i++)
                {
                    if (cardArr[i] >= 3)
                    {
                        canRedeem = true;
                        foreach (Image card in cards.GetComponentsInChildren<Image>())
                        {
                            card.sprite = cardSprites[i];
                            card.color = Color.white;
                        }
                        break;
                    }
                }
            }
            if (canRedeem)
            {
                transform.Find("Redeem").GetComponent<Button>().interactable = true;
                transform.Find("Redeem").GetChild(0).GetComponent<TextMeshProUGUI>().text = "redeem " + cardBonus;
            }
            else
            {
                transform.Find("Redeem").GetComponent<Button>().interactable = false;
                TextMeshProUGUI redeemTxt = transform.Find("Redeem").GetChild(0).GetComponent<TextMeshProUGUI>();
                redeemTxt.text = "redeem 0";
                redeemTxt.color = new Color(redeemTxt.color.r, redeemTxt.color.g, redeemTxt.color.b, 165f);
            }
        }
    }

    public void HideGUI()
    {
        toggleButton.transform.GetChild(0).gameObject.SetActive(true);
        toggleButton.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        toggleButton.transform.GetChild(0).Find("Counter").GetComponent<TextMeshProUGUI>().text = 
            GameMasterScript.Instance.GetCurrentPlayer().GetNumOfCards().ToString();
    }

    public void Redeem()
    {
        PlayerScript player = GameMasterScript.Instance.GetCurrentPlayer();
        int[] cardArr = player.GetCardsArr();
        if (cardArr[0] > 0 && cardArr[1] > 0 && cardArr[2] > 0)
        {
            player.RemoveCardFrom(0, 1);
            player.RemoveCardFrom(1, 1);
            player.RemoveCardFrom(2, 1);
        }
        else
        {
            for (int i = 0; i < cardArr.Length; i++)
            {
                if (cardArr[i] >= 3)
                {
                    player.RemoveCardFrom(i, 3);
                }
            }
        }
        GameMasterScript.Instance.gameObject.GetComponent<DraftHandlerScript>().AddTroopsToDraft(cardBonus);
        numOfRedeems++;
        if (numOfRedeems < 5)
        {
            cardBonus += 2;
        }
        HideGUI();
    }
}
