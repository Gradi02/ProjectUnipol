using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    private Queue<GameEvent> events = new Queue<GameEvent>();
    public bool isEventEnded = true;
    public State currentState { get; private set; } = null;
    public bool isStateEnded = true;

    [Header("States")]
    public State endTurnS;
    public State awaitRollS, awaitRollDecS, awaitBuyS, awaitUpgrDecS, awaitPayPlayerState, awaitingSellState, awaitCard, awaitSelectS, endGameS;

    [Header("References")]
    [SerializeField] private PlayerOverlayCard[] uiPlayersCards;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Card> cards = new List<Card>();
    [SerializeField] private Material[] playersMaterials;

    [Header("UI")]
    public CanvaManager canvaManager;
    public Image cardSprite;
    public GameObject transitionCanva, cardCanva, upgradeCanva;
    public TextMeshProUGUI transitionText, upgradeText, cardDesc, cardTitle;
    public Button[] upgrades;
    public Color defaultColor, selectedColor;
    public TextMeshProUGUI[] upgradesText;

    [Header("juwe")]
    public NormalField juwenaliaField;
    public float[] juwenaliaPricing = {1.25f, 1.3f, 1.4f, 1.5f, 1.75f, 2.0f, 2.25f, 2.5f};
    public int juwenaliaPricePointer { get; set; } = 0;


    public static GameManager instance { get; private set; }
    public bool isGameStarted { get; set; } = false;

    public bool isMultiplayer { get; set; } = false;
    public List<Player> players { get; private set; } = new List<Player>();
    public int currentPlayerIndex { get; private set; } = 0;
    public bool dublet { get; set; } = false;


    public Board board;



    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartGame(PlayerUICard[] p)
    {
        if (isMultiplayer)
        {
            Debug.Log("1");
            int idx = 0;
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i].ready)
                {
                    Debug.Log("2");
                    SetupPlayerClientRpc(i, idx, p[i].usernameText, p[i].bgc);
                    idx++;
                }
            }

            DisableRestClientRpc(idx);
        }
        else
        {
            int idx = 0;
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i].ready)
                {
                    GameObject newPlayer = Instantiate(playerPrefab);
                    Player newPlayerScript = newPlayer.GetComponent<Player>();
                    players.Add(newPlayerScript);
                    newPlayer.GetComponent<MeshRenderer>().material = playersMaterials[i];
                    uiPlayersCards[idx].Setup(p[i]);

                    newPlayerScript.SetUp(uiPlayersCards[idx], p[i].usernameText);
                    idx++;
                }
            }

            for (int i = idx; i < uiPlayersCards.Length; i++)
            {
                uiPlayersCards[i].Disabled();
            }

            board.SetupPlayers(players);
        }

        currentPlayerIndex = Random.Range(0, players.Count);
        isGameStarted = true;
        AddEvent(endTurnS);
    }

    [ClientRpc]
    private void SetupPlayerClientRpc(int i, int idx, string username, Color bgc)
    {
        Debug.Log("3");
        GameObject newPlayer = Instantiate(playerPrefab);
        Player newPlayerScript = newPlayer.GetComponent<Player>();
        players.Add(newPlayerScript);
        newPlayer.GetComponent<MeshRenderer>().material = playersMaterials[i];
        uiPlayersCards[idx].SetupNetworkVersion(username, bgc);

        newPlayerScript.SetUp(uiPlayersCards[idx], username);
    }

    [ClientRpc]
    private void DisableRestClientRpc(int idx)
    {
        Debug.Log("4");
        for (int i = idx; i < uiPlayersCards.Length; i++)
        {
            uiPlayersCards[i].Disabled();
        }

        board.SetupPlayers(players);
    }

    public void CheckForWin()
    {
        int activePlayers = 0;
        foreach(Player p in players)
        {
            if (p.isActive)
                activePlayers++;
        }

        if(activePlayers == 1)
        {
            //WIN
            AddEvent(endGameS, true);
        }
    }

    private void Update()
    {
        if (isMultiplayer && !IsHost) return;

        if(isEventEnded && events.Count > 0)
        {
            isEventEnded = false;
            events.Dequeue().RunEvent();
        }
        
        if(events.Count == 0 && isEventEnded && isStateEnded && isGameStarted)
        {
            AddEvent(endTurnS);
        }
    }

    public void AddEvent(string t)
    {
        events.Enqueue(new ShowInfoEvent(t));
    }

    public void AddEvent(State s, bool force = false)
    {
        if(force)
        {
            isStateEnded = false;
            events.Clear();
            isStateEnded = true;
        }

        events.Enqueue(new ChangeStateEvent(s));
    }



    public Card GetRandomCard()
    {
        return cards[Random.Range(0, cards.Count)];
    }



    public void NextPlayer()
    {
        do
        {
            currentPlayerIndex++;
            if (currentPlayerIndex >= players.Count)
                currentPlayerIndex = 0;

            if(players[currentPlayerIndex].stopTurns > 0)
            {
                players[currentPlayerIndex].stopTurns--;

                if (players[currentPlayerIndex].stopTurns > 0)
                {
                    int trn = players[currentPlayerIndex].stopTurns;
                    string strend = (trn == 1) ? $"{trn} ture!" : $"{trn} tury!";

                    AddEvent($"Gracz {players[currentPlayerIndex].playerName} jest jeszcze uziemiony przez " + strend);
                }
                else
                {
                    AddEvent($"Gracz {players[currentPlayerIndex].playerName} zdo³a³ ju¿ op³aciæ warunek!");
                }
            }
        } 
        while (!players[currentPlayerIndex].isActive || players[currentPlayerIndex].stopTurns > 0);
        

        string t = $"Kolej gracza {players[currentPlayerIndex].playerName}!";
        AddEvent(t);

        //change state
        isStateEnded = true;
        AddEvent(awaitRollDecS);
    }

    public void ChangeState(State newState)
    {
        if (isStateEnded)
        {
            isStateEnded = false;
            currentState = newState;
            currentState.Execute();
        }
    }





    //ENUMERATORS
    public IEnumerator IEShowInfo(string title)
    {
        transitionText.text = title;
        transitionCanva.SetActive(true);
        yield return new WaitForSeconds(2f);
        transitionCanva.SetActive(false);

        isEventEnded = true;
        yield return null;
    } 
}

public abstract class GameEvent
{
    public abstract void RunEvent();
}
public class ShowInfoEvent : GameEvent
{
    private string text;
    public ShowInfoEvent(string t)
    {
        text = t;
    }
    public override void RunEvent()
    {
        GameManager.instance.StartCoroutine(GameManager.instance.IEShowInfo(text));
    }
}

public class ChangeStateEvent : GameEvent
{
    State state;
    public ChangeStateEvent(State nstate)
    {
        state = nstate;
    }
    public override void RunEvent()
    {
        GameManager.instance.ChangeState(state);
        GameManager.instance.isEventEnded = true;
    }
}