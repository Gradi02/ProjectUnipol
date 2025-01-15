using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class endTurnState : State
{

    public override void Execute()
    {
        HandleNextTurn();
    }



    private void HandleNextTurn()
    {
        gameManager.cardCanva.SetActive(false);
        gameManager.upgradeCanva.SetActive(false);
        gameManager.canvaManager.SetCanvaActivity(false, false, false, false, false, false, "");

        if (gameManager.isMultiplayer)
        {
            if (gameManager.networkDublet.Value)
            {
                SetDubletServerRpc();               
            }
            else
            {
                NextPlayerServerRpc();
            }
        }
        else
        {
            if (gameManager.dublet)
            {
                gameManager.dublet = false;
                gameManager.isStateEnded = true;
                gameManager.AddEvent(gameManager.awaitRollDecS);
            }
            else
            {
                gameManager.NextPlayer();
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetDubletServerRpc()
    {
        gameManager.networkDublet.Value = false;
        gameManager.isStateEnded = true;
        gameManager.AddStateEventClientRpc(gameManager.awaitRollDecS.name, currentPlayer.clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NextPlayerServerRpc()
    {
        gameManager.NextPlayer();
    }
}
