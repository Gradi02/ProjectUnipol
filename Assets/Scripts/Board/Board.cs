using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    public BoardField[] fields;
    private Vector3[] offsets = {
        new Vector3(0.15f, 0.25f, 0.15f),
        new Vector3(0.15f, 0.25f, -0.15f),
        new Vector3(-0.15f, 0.25f, -0.15f),
        new Vector3(-0.15f, 0.25f, 0.15f),
    };

    public void SetupPlayers(List<Player> pl)
    {
        for(int i = 0; i < pl.Count; i++)
        {
            pl[i].gameObject.transform.position = fields[0].transform.position + offsets[i];
        }
    }

    public void MovePlayer(Player pl, int num)
    {
        StartCoroutine(IEMovePlayer(pl, num));
    }

    private IEnumerator IEMovePlayer(Player pl, int num)
    {
        for(int i=0; i<num; i++)
        {
            fields[pl.currentPosition].OnPlayerVisitExit(pl);
            pl.currentPosition++;
            if(pl.currentPosition >= fields.Length)
                pl.currentPosition = 0;
            fields[pl.currentPosition].OnPlayerVisitEnter(pl);
            yield return new WaitForSeconds(0.2f);
        }

        GameManager.instance.isStateEnded = true;
        fields[pl.currentPosition].OnPlayerLand(pl);
    }
}
