using System.Collections;
using UnityEngine;

public class awaitingRollState : State
{

    public override void Execute()
    {
        StartCoroutine(IERollAnimation());
    }


    private IEnumerator IERollAnimation()
    {
        //roll animation here

        gameManager.playerTurnCanva.SetActive(false);

        int k1 = Random.Range(1, 6);
        int k2 = Random.Range(1, 6);

        int num = k1 + k2;
        if (k1 == k2)
        {
            gameManager.AddEvent("Double Throw!");
            gameManager.dublet = true;
        }
        else
        {
            gameManager.AddEvent($"{k1} : {k2}");
        }

        //animacja kostki

        //yield return
        gameManager.board.MovePlayer(gameManager.players[gameManager.currentPlayerIndex], num);

        yield return null;
    }
}
