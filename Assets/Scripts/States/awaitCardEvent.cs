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
        Card card = gameManager.GetRandomCard();

        gameManager.cardSprite.sprite = card.cardImage;
        gameManager.cardTitle.text = card.cardName;
        gameManager.cardDesc.text = card.desc;
        gameManager.cardCanva.SetActive(true);
        yield return new WaitForSeconds(6f);

        card.RunCardSetup(currentField, currentPlayer);
        gameManager.cardCanva.SetActive(false);

        gameManager.isStateEnded = true;
        yield return null;
    }
}
