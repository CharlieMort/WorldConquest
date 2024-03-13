using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryHighlightScript : MonoBehaviour
{
    private void Start()
    {
        GameMasterScript.Instance.ActionAfterPhaseChange += DraftHighlight;
    }

    public void DraftHighlight()
    {
        foreach(CountryScript country in GameMasterScript.Instance.getAllCountries())
        {
            if (country.ownerID == GameMasterScript.Instance.getPlayersTurn())
            {
                country.Highlight();
            }
            else
            {
                country.Darken();
            }
        }
    }
}
