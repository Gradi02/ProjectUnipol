using System.Collections;
using UnityEngine;

public class awaitCardEvent : State
{
    public override void Execute()
    {
        StartCoroutine(IECardAnim());
    }

    private IEnumerator IECardAnim()
    {

        yield return null;
    }
}
