using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class State : MonoBehaviour
{
    protected GameManager gameManager => GameManager.instance;
    protected Player currentPlayer => gameManager.players[gameManager.currentPlayerIndex];
    protected BoardField currentField => gameManager.board.fields[currentPlayer.currentPosition];

    public abstract void Execute();
}
