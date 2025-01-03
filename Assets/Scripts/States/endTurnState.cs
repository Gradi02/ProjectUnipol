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
        gameManager.cardCanva.SetActive(false);
        gameManager.upgradeCanva.SetActive(false);
        gameManager.canvaManager.SetCanvaActivity(false, false, false, false, false, false, "");

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
