using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public List<Player> players { get; private set; }
    public Board board { get; private set; }
    public int currentPlayerIndex { get; private set; }




    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    

    public void StartGame(List<Player> p)
    {
        players = p;
    }
}
