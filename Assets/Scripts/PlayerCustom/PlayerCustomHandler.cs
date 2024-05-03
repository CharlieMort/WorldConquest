using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCustomHandler : MonoBehaviour
{
    public Sprite[] playerIcons;
    private int[] playerIdxs = new int[4] { 0, 1, 2, 3 };
    private Image[] icons;

    private void Awake()
    {
        icons = new Image[playerIdxs.Length];
        for (int i = 0; i<playerIdxs.Length; i++)
        {
            // Pages - Page 2 - Icons - Custom - Icon
            icons[i] = transform.GetChild(1).GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>();
        }
    }

    public void NextIcon(int idx)
    {
        playerIdxs[idx]++;
        if (playerIdxs[idx] >= playerIcons.Length) playerIdxs[idx] = 0;
        icons[idx].sprite = playerIcons[playerIdxs[idx]];
    }

    public void PrevIcon(int idx)
    {
        playerIdxs[idx]--;
        if (playerIdxs[idx] < 0) playerIdxs[idx] = playerIcons.Length - 1;
        icons[idx].sprite = playerIcons[playerIdxs[idx]];
    }

    public void StartGame()
    {
        PlayerInfoStaticScript.playerIconIdxs = playerIdxs;
        string[] playerNames = new string[4];
        for (int i = 0; i<playerNames.Length;i++)
        {
            playerNames[i] = transform.GetChild(1).GetChild(0).GetChild(i).GetChild(4).GetComponent<TMP_InputField>().text;
        }
        PlayerInfoStaticScript.playerNames = playerNames;
        SceneManager.LoadScene(2);
    }

    public void SetGameType(string gameType)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        PlayerInfoStaticScript.gameType = gameType;
    }
}
