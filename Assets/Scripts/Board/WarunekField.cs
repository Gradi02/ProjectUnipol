using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class WarunekField : BoardField
{
    void Start()
    {
        fname.text = property.fieldname;
    }

    public override void OnPlayerLand(Player pl)
    {
        pl.stopTurns = 3;
        gameManager.dublet = false;
        gameManager.AddEvent($"Gracz {pl.playerName} jest uziemiony na 3 kolejne tury!");
    }
}
