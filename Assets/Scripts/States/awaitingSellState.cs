using System.Collections;
using UnityEngine;

public class awaitingSellState : State
{
    public override void Execute()
    {
        StartCoroutine(IEChooseSellAnimation());
    }

    private void EndState()
    {
        //po wybraniu i zatwierdzeniu
    }

    private IEnumerator IEChooseSellAnimation()
    {
        //animacja wysuniêcia klocków i w³¹czenie raycasta
        yield return null;
    } 
}
