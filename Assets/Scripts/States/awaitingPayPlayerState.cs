using System.Collections;
using UnityEngine;

public class awaitingPayPlayerState : State
{

    public override void Execute()
    {
        StartCoroutine(IEPayAnimation());
    }




    private IEnumerator IEPayAnimation()
    {
        Player owner = currentField.property.owner;

        int currentVisitPrice = 0;
        IOwnableProperty pr = currentField.GetComponent<IOwnableProperty>();
        if (pr != null)
        {
            currentVisitPrice = pr.GetCurrentVisitPrice();
        }
        else
        {
            Debug.LogWarning($"{currentField} nie implementuje IOwnableProperty!");
        }

        string t = $"Player {currentPlayer.playerName} Paid ${currentVisitPrice} To {currentField.property.owner.playerName}!";

        currentPlayer.money -= currentVisitPrice;
        owner.money += currentVisitPrice;

        gameManager.transitionText.text = t;
        gameManager.transitionCanva.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameManager.transitionCanva.SetActive(false);

        gameManager.isStateEnded = true;
        gameManager.isEventEnded = true;

        yield return null;
    }
}
