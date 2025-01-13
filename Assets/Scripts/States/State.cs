using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public abstract class State : NetworkBehaviour
{
    protected GameManager gameManager => GameManager.instance;
    protected Player currentPlayer => gameManager.players[gameManager.currentPlayerIndex];
    protected BoardField currentField => gameManager.board.fields[currentPlayer.currentPosition];

    public abstract void Execute();
}
