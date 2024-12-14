using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class awaitingBuyDecisionState : State
{

    public override void Execute()
    {
        BuyHandle();
    }




    private void BuyHandle()
    {
        string s = $"Do You Want To Buy {currentField.property.fieldname} For {currentField.property.price}?";
        gameManager.buyText.text = s;
        gameManager.buyCanva.SetActive(true);
    }


    public void BuyButtonYes()
    {
        if (gameManager.currentState == this)
        {
            currentPlayer.money -= currentField.property.price;
            currentPlayer.AddFieldToList(currentField);
            currentField.OnBuy(currentPlayer);

            gameManager.isStateEnded = true;
        }

    }

    public void BuyButtonNo()
    {
        if (gameManager.currentState == this)
        {
            gameManager.isStateEnded = true;
        }
    }
}