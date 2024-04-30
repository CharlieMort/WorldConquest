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
    public string countriesOwned;


    private void Start()
    {
        playerUI.transform.Find("Background").GetComponent<Image>().color = playerColor;

    }
}
