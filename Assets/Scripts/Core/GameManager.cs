using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerOverlayCard[] uiPlayersCards;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Shader shader;
    

    public static GameManager instance { get; private set; }
    public List<Player> players { get; private set; } = new List<Player>();
    public int currentPlayerIndex { get; private set; } = 0;
    
    
    public Board board;



    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartGame(PlayerUICard[] p)
    {
        int idx = 0;
        for (int i = 0; i < p.Length; i++)
        {
            if (p[i].ready)
            {
                GameObject newPlayer = Instantiate(playerPrefab);
                Player newPlayerScript = newPlayer.GetComponent<Player>();
                Material mat = new Material(shader);
                mat.color = p[i].bgc;
                newPlayer.GetComponent<MeshRenderer>().material = mat;

                uiPlayersCards[idx].Setup(p[i]);
                newPlayerScript.SetUp(uiPlayersCards[idx]);
                idx++;

                players.Add(newPlayerScript);
            }
        }

        for(int i = idx; i < uiPlayersCards.Length; i++)
        {
            uiPlayersCards[i].Disabled();
        }

        board.SetupPlayers(players);
    }



    public void NextTurn()
    {
        int k1 = Random.Range(1, 6);
        int k2 = Random.Range(1, 6);
        Debug.Log($"k1: {k1}, k2: {k2}");

        int num = k1 + k2;
        board.MovePlayer(players[currentPlayerIndex], num);

        if (k1 != k2)
            currentPlayerIndex++;

        if (currentPlayerIndex >= players.Count)
            currentPlayerIndex = 0;
    }
}
