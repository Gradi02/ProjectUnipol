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
        card.RunCardSetup(currentField, currentPlayer);

        gameManager.cardSprite.sprite = card.cardImage;
        gameManager.cardTitle.text = card.cardName;
        gameManager.cardDesc.text = card.desc;
        gameManager.cardCanva.SetActive(true);
        yield return new WaitForSeconds(6f);
        gameManager.cardCanva.SetActive(false);

        IDirectEvent de = card.GetComponent<IDirectEvent>();
        if (de == null)
            gameManager.isStateEnded = true;
        else
            de.RunCardDirectEvent();

        yield return null;
    }
}
