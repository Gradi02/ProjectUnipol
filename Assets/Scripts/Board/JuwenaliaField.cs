using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class JuwenaliaField : BoardField
{

    private void Start()
    {
        fname.text = property.fieldname;
    }

    public override void OnPlayerLand(Player pl)
    {
        bool canSelect = false;
        if(pl.ownedProperties.Count > 0)
        {
            foreach(BoardField field in pl.ownedProperties)
            {
                if(field is not MPKField)
                {
                    canSelect = true;
                    break;
                }
            }
        }

        if (canSelect)
        {
            gameManager.AddEvent(gameManager.awaitSelectS);
        }
        else
        {
            gameManager.AddEvent($"Gracz {pl.playerName} nie posiada pola które mo¿e zorganizowaæ Juwenalia!");
            gameManager.AddEvent(gameManager.endTurnS);
        }
    }
}
