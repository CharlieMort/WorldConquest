using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    private GameObject select1;
    private GameObject select2;

    public void SelectCountry(GameObject country)
    {
        if (select1 != null)
        {
            SetSelect2(country);
        }
        else
        {
            SetSelect1(country);
        }
    }

    /// <summary>
    /// MIGHT DO A REWORK HERE LATER WHERE WE GET RID OF THE TWO COUNTRY SELECT THINGS DW ITS IN MY HEAD LOL 
    /// </summary>


    public Action<GameObject> ActionAfterCountrySelect1;
    public void SetSelect1(GameObject country)
    {
        // select1 = country;
        // //print(select1.name);
        if (ActionAfterCountrySelect1 != null) ActionAfterCountrySelect1(country);
    }

    public Action ActionAfterCountrySelect2;
    public void SetSelect2(GameObject country)
    {
        select2 = country;
        if (ActionAfterCountrySelect2 != null) ActionAfterCountrySelect2();
    }

    public void ClearSelect1()
    {
        select1 = null;
    }

    public void ClearSelect2()
    {
        select2 = null;
    }
}
