using UnityEngine;

public class GoStartCard : Card, IDirectEvent
{
    BoardField boardField;
    Player player;

    public override void RunCardSetup(BoardField currentField, Player currentPlayer)
    {
        boardField = currentField;
        player = currentPlayer;
        player.ownedCards.Add(this);
    }

    public override void RunCardEventOnPlayerLand()
    {}

    public void RunCardDirectEvent()
    {
        int num = gameManager.board.fields.Length - player.currentPosition;
        gameManager.board.MovePlayer(player, num);
    }
}
