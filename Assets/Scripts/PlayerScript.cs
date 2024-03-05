using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public string playerName;
    public Material playerMat;
    public GameObject playerUI;
    public Sprite playerSprite;

    private void Start()
    {
        playerUI.transform.Find("Background").GetComponent<Image>().color = playerMat.color;
    }
}
