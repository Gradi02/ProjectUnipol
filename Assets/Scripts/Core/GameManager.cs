using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Queue<GameEvent> events = new Queue<GameEvent>();
    private bool isEventEnded = true;

    [Header("References")]
    [SerializeField] private PlayerOverlayCard[] uiPlayersCards;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Shader shader;
    [SerializeField] private GameObject transitionCanva, playerTurnCanva, cardCanva, buyCanva;
    [SerializeField] private TextMeshProUGUI transitionText, buyText;
    [SerializeField] private Button rollButton, surrenderButton;


    public static GameManager instance { get; private set; }
    public List<Player> players { get; private set; } = new List<Player>();
    public int currentPlayerIndex { get; private set; } = 0;

    public List<Card> cards = new List<Card>();

    public Board board;

    private bool dublet = false, canStartNextTurn = true, isGameStarted = false;



    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        transitionCanva.SetActive(false);
        playerTurnCanva.SetActive(false);
        cardCanva.SetActive(false);
        buyCanva.SetActive(false);
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
                newPlayerScript.SetUp(uiPlayersCards[idx], p[i].usernameText);
                idx++;

                players.Add(newPlayerScript);
            }
        }

        for (int i = idx; i < uiPlayersCards.Length; i++)
        {
            uiPlayersCards[i].Disabled();
        }

        board.SetupPlayers(players);

        currentPlayerIndex = Random.Range(0, players.Count);
        isGameStarted = true;
    }

    private void Update()
    {
        //events
        if (events.Count > 0)
        {
            if (isEventEnded)
            {
                isEventEnded = false;
                events.Dequeue().RunEvent();
            }
        }
        else
        {
            if (isGameStarted && canStartNextTurn)
            {
                canStartNextTurn = false;
                if (dublet)
                {
                    dublet = false;
                    ShowPlayerState(true);
                }
                else
                {
                    NextPlayer();
                }
            }
        }
    }


    #region BUTTONS

    public void RollButton()
    {
        playerTurnCanva.SetActive(false);

        int k1 = Random.Range(1, 6);
        int k2 = Random.Range(1, 6);
        Debug.Log($"k1: {k1}, k2: {k2}");

        int num = k1 + k2;
        if (k1 == k2)
        {
            events.Enqueue(new InfoEvent("Double Throw!"));
            dublet = true;
        }
        else
        {
            events.Enqueue(new InfoEvent($"{k1} : {k2}"));
        }

        board.MovePlayer(players[currentPlayerIndex], num);
    }

    public void SurrenderButton()
    {
        playerTurnCanva.SetActive(false);

        players[currentPlayerIndex].Surrender();
        NextPlayer();
    }

    public void BuyButtonYes()
    {
        Player currentPlayer = players[currentPlayerIndex];
        BoardField field = board.fields[currentPlayer.currentPosition];

        if (field.property.owner == null)
        {
            field.property.owner = currentPlayer;
            currentPlayer.money -= field.property.price;
            field.gameObject.GetComponent<MeshRenderer>().material = currentPlayer.gameObject.GetComponent<MeshRenderer>().material;
        }
        else
        {
            currentPlayer.money -= field.property.upgradePrice[field.property.level];
            field.property.level++;
        }

        currentPlayer.AddFieldToList(field);
        field.OnBuy(currentPlayer);
        buyCanva.SetActive(false);

        isEventEnded = true;
        canStartNextTurn = true;
    }

    public void BuyButtonNo()
    {
        buyCanva.SetActive(false);

        isEventEnded = true;
        canStartNextTurn = true;
    }

    #endregion


    private void NextPlayer()
    {
        canStartNextTurn = false;

        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count)
            currentPlayerIndex = 0;

        while (!players[currentPlayerIndex].isActive)
        {
            currentPlayerIndex++;
            if (currentPlayerIndex >= players.Count)
                currentPlayerIndex = 0;
        }
        
        string t = $"{players[currentPlayerIndex].playerName}'s Turn!";
        AddGameEvent(new InfoEvent(t, true));
    }

    private void ShowPlayerState(bool show)
    {
        rollButton.interactable = true;
        surrenderButton.interactable = true;
        playerTurnCanva.SetActive(show);
    }

    public void AddGameEvent(GameEvent ev)
    {
        events.Enqueue(ev);
    }

    public void ElseState()
    {
        canStartNextTurn = true;
    }



    //Pamiêta
    //Na konieæ ka¿dego eventu w grze ustaw isEventEnded na true ORAZ jeœli gracz nie podejmuje docelowo ¿adnej decyzji po evencie 
    //to ustawiæ canStartNextTurn na true -> inaczej mo¿e dojœæ do wiecznej pauzy w GameLoopie 
    
    public IEnumerator IEShowEvent(string title, bool nextIE = false)
    {
        transitionText.text = title;
        transitionCanva.SetActive(true);
        yield return new WaitForSeconds(2f);
        transitionCanva.SetActive(false);

        if (nextIE)
        {
            ShowPlayerState(true);
            isEventEnded = true;
            yield break;
        }

        canStartNextTurn = true;
        isEventEnded = true;
        yield return null;
    }

    public IEnumerator IEBuildEvent(string ask)
    {
        buyText.text = ask;
        buyCanva.SetActive(true);

        canStartNextTurn = false;
        yield return null;
    }

    public IEnumerator IEPayPlayerEvent(string ask, Player payer, Player owner, int price)
    {
        payer.money -= price;
        owner.money += price;

        transitionText.text = ask;
        transitionCanva.SetActive(true);
        yield return new WaitForSeconds(2f);
        transitionCanva.SetActive(false);

        canStartNextTurn = true;
        isEventEnded = true;
        yield return null;
    }
}




public abstract class GameEvent
{
    public abstract void RunEvent();
}

public class InfoEvent : GameEvent
{
    private string titleText;
    bool nextIE = false;

    public InfoEvent(string title, bool next = false)
    {
        titleText = title;
        nextIE = next;
    }

    public override void RunEvent()
    {
        GameManager.instance.StartCoroutine(GameManager.instance.IEShowEvent(titleText, nextIE));
    }
}

public class BuyEvent : GameEvent
{
    private string titleText;
    
    public BuyEvent(string title)
    {
        titleText = title;
    }

    public override void RunEvent()
    {
        GameManager.instance.StartCoroutine(GameManager.instance.IEBuildEvent(titleText));
    }
}

public class PayToPlayerEvent : GameEvent
{
    private string titleText;
    private Player payer;
    private Player owner;
    private int price;

    public PayToPlayerEvent(string txt, Player payer, Player owner, int price)
    {
        titleText = txt;

        this.payer = payer;
        this.owner = owner;
        this.price = price;
    }

    public override void RunEvent()
    {
        GameManager.instance.StartCoroutine(GameManager.instance.IEPayPlayerEvent(titleText, payer, owner, price));
    }
}