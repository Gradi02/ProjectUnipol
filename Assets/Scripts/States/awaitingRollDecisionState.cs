using System.Collections;
using UnityEngine;

public class awaitingRollDecisionState : State
{

    public override void Execute()
    {
        StartCoroutine(IERollAnimation());
    }



    private IEnumerator IERollAnimation()
    {
        HandleRollDecision();
        yield return null;
    }

    private void HandleRollDecision()
    {
        gameManager.rollButton.interactable = true;
        gameManager.surrenderButton.interactable = true;
        gameManager.playerTurnCanva.SetActive(true);
    }




    public void RollButton()
    {
        if (gameManager.currentState == this)
        {
            gameManager.isStateEnded = true;
            gameManager.AddEvent(gameManager.awaitRollS);
        }
    }

    public void SurrenderButton()
    {
        if (gameManager.currentState == this)
        {
            gameManager.players[gameManager.currentPlayerIndex].Surrender();
            gameManager.isStateEnded = true;
        }
    }
}
