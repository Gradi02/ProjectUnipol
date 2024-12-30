using UnityEngine;

public class endGameState : State
{

    public override void Execute()
    {
        //StartCoroutine(IECardAnim());
        Debug.Log("WIN!");
        gameManager.isGameStarted = false;
    }
}
