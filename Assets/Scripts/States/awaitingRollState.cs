using System.Collections;
using UnityEngine;

public class awaitingRollState : State
{
    public GameObject kostka1;
    public GameObject kostka2;
    public override void Execute()
    {
        StartCoroutine(IERollAnimation());
    }

    IEnumerator Roll(int k1, int k2)
    {
        GameObject roll1 = Instantiate(kostka1, Vector3.zero, Quaternion.identity);
        Destroy(roll1, 3);
        roll1.GetComponent<KostkaManager>().rolled.text = k1.ToString();
        LeanTween.scale(roll1, Vector3.zero, 0.3f).setDelay(2.5f).setEase(LeanTweenType.easeInBack);
        yield return new WaitForSeconds(0.2f);

        GameObject roll2 = Instantiate(kostka2, Vector3.zero, Quaternion.identity);
        Destroy(roll2, 3);
        roll2.GetComponent<KostkaManager>().rolled.text = k2.ToString();
        LeanTween.scale(roll2, Vector3.zero, 0.3f).setDelay(2.5f).setEase(LeanTweenType.easeInBack);
        yield return new WaitForSeconds(1.8f);
    }

    private IEnumerator IERollAnimation()
    {
        gameManager.canvaManager.SetCanvaActivity(false, true, false, false, false, true, "");

        int k1 = UnityEngine.Random.Range(1, 6);
        int k2 = UnityEngine.Random.Range(1, 6);

        yield return StartCoroutine(Roll(k1, k2));

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


        gameManager.board.MovePlayer(gameManager.players[gameManager.currentPlayerIndex], num);

        yield return null;
    }
}
