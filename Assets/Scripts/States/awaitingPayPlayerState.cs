using System.Collections;
using UnityEngine;

public class awaitingPayPlayerState : State
{

    public override void Execute()
    {
        StartCoroutine(IERollAnimation());
    }




    private IEnumerator IERollAnimation()
    {
        Player owner = currentField.property.owner;

        string t = $"Player {currentPlayer.playerName} Paid ${currentField.property.currentVisitPrice} To {currentField.property.owner.playerName}!";

        currentPlayer.money -= currentField.property.currentVisitPrice;
        owner.money += currentField.property.currentVisitPrice;

        gameManager.transitionText.text = t;
        gameManager.transitionCanva.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameManager.transitionCanva.SetActive(false);

        gameManager.isStateEnded = true;
        gameManager.isEventEnded = true;

        yield return null;
    }
}
