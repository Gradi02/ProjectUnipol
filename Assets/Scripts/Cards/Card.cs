using UnityEngine;

public abstract class Card : MonoBehaviour
{
    public bool readyToDestroy = false;
    public Sprite cardImage;
    public string cardName;
    public string desc;
    protected GameManager gameManager => GameManager.instance;
    public abstract void RunCardSetup(BoardField currentField, Player currentPlayer);
    public abstract void RunCardEventOnPlayerLand();
    public abstract void RunCardDirectEvent();
}
