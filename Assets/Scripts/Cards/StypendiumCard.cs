using System.Collections;
using UnityEngine;

public class StypendiumCard : Card
{
    BoardField boardField;
    Player player;
    int stypendiumIdx = 3;
    int stypendiumSize = 10000;

    public override void RunCardSetup(BoardField currentField, Player currentPlayer)
    {
        boardField = currentField;
        player = currentPlayer;
        player.ownedCards.Add(this);
    }

    public override void RunCardEventOnPlayerLand()
    {
        if(stypendiumIdx > 0)
        {
            player.money += stypendiumSize;
        }

        stypendiumIdx--;

        if(stypendiumIdx <= 0)
        {
            player.ownedCards.Remove(this);
        }
    }

    public override void RunCardDirectEvent()
    {

    }
}
