using System.Collections;
using UnityEngine;

public class endTurnState : State
{

    public override void Execute()
    {
        HandleNextTurn();
    }



    private void HandleNextTurn()
    {
        gameManager.playerTurnCanva.SetActive(false);
        gameManager.cardCanva.SetActive(false);
        gameManager.buyCanva.SetActive(false);
        gameManager.upgradeCanva.SetActive(false);

        if (gameManager.dublet)
        {
            gameManager.dublet = false;
            gameManager.isStateEnded = true;
            gameManager.AddEvent(gameManager.awaitRollDecS);
        }
        else
        {
            gameManager.NextPlayer();
        }
    }
}
