using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Queue<GameEvent> events = new Queue<GameEvent>();
    public bool isEventEnded = true;
    public GameState currentState { get; private set; } = GameState.idle;
    public bool isStateEnded = true;

    [Header("References")]
    [SerializeField] private PlayerOverlayCard[] uiPlayersCards;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Shader shader;
    [SerializeField] private GameObject transitionCanva, playerTurnCanva, cardCanva, buyCanva, upgradeCanva;
    [SerializeField] private TextMeshProUGUI transitionText, buyText, upgradeText;
    [SerializeField] private Button rollButton, surrenderButton;
    [SerializeField] private Button[] upgrades;
    [SerializeField] private Color defaultColor, selectedColor;


    public static GameManager instance { get; private set; }
    public bool isGameStarted { get; private set; } = false;
    public List<Player> players { get; private set; } = new List<Player>();
    public int currentPlayerIndex { get; private set; } = 0;

    public List<Card> cards = new List<Card>();

    public Board board;

    private bool dublet = false;
    private int upgradeSelectionIndex = -1;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        transitionCanva.SetActive(false);
        playerTurnCanva.SetActive(false);
        cardCanva.SetActive(false);
        buyCanva.SetActive(false);
        upgradeCanva.SetActive(false);
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
        AddEvent(GameState.endTurn);
    }

    private void Update()
    {
        if(isEventEnded && events.Count > 0)
        {
            isEventEnded = false;
            events.Dequeue().RunEvent();
        }
        
        if(events.Count == 0 && isEventEnded && isStateEnded && isGameStarted)
        {
            AddEvent(GameState.endTurn);
        }
    }

    public void AddEvent(string t)
    {
        events.Enqueue(new ShowInfoEvent(t));
    }

    public void AddEvent(GameState s)
    {
        events.Enqueue(new ChangeStateEvent(s));
    }
    public void NextPlayer()
    {
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
        AddEvent(t);

        //change state
        isStateEnded = true;
        AddEvent(GameState.awaitingRollDecision);
    }


    #region BUTTONS

    public void RollButton()
    {
        if (currentState == GameState.awaitingRollDecision)
        {
            isStateEnded = true;
            AddEvent(GameState.awaitingRoll);
        }
    }

    public void SurrenderButton()
    {
        if (currentState == GameState.awaitingRollDecision)
        {
            players[currentPlayerIndex].Surrender();
            isStateEnded = true;
        }
    }

    public void BuyButtonYes()
    {
        if (currentState == GameState.awaitingBuyDecision)
        {
            Player currentPlayer = players[currentPlayerIndex];
            BoardField field = board.fields[currentPlayer.currentPosition];

            field.property.owner = currentPlayer;
            currentPlayer.money -= field.property.price;

            currentPlayer.AddFieldToList(field);
            field.OnBuy(currentPlayer);

            isStateEnded = true;
        }

    }

    public void BuyButtonNo()
    {
        if (currentState == GameState.awaitingBuyDecision)
        {
            isStateEnded = true;
        }
    }


    public void ConfirmUpgradeSelection()
    {
        if (currentState == GameState.awaitingUpgradeDecision)
        {
            if (upgradeSelectionIndex == -1)
            {
                isStateEnded = true;
                return;
            }
            else
            {
                Player currentPlayer = players[currentPlayerIndex];
                BoardField field = board.fields[currentPlayer.currentPosition];
                
                currentPlayer.money -= field.property.upgradePrice[field.property.level];
                field.property.level++;

                field.OnUpgrade();
            }

            isStateEnded = true;
        }
    }

    public void SwitchUpgradeSelection(int idx)
    {
        if (currentState == GameState.awaitingUpgradeDecision)
        {
            if (upgradeSelectionIndex == idx)
            {
                upgradeSelectionIndex = -1;
                upgrades[idx].GetComponent<Image>().color = defaultColor;
            }
            else
            {
                if(upgradeSelectionIndex != -1)
                    upgrades[upgradeSelectionIndex].GetComponent<Image>().color = defaultColor;

                upgradeSelectionIndex = idx;
                upgrades[idx].GetComponent<Image>().color = selectedColor;
            }
        }
    }

    #endregion

    #region States

    public void ChangeState(GameState newState)
    {
        if (isStateEnded)
        {
            isStateEnded = false;
            currentState = newState;
            OnStateEnter(currentState);
        }
    }

    private void OnStateEnter(GameState state)
    {
        switch (state)
        {
            case GameState.endTurn:
                {
                    HandleNextTurn();
                    break;
                }
            case GameState.awaitingRollDecision:
                {
                    HandleRollDecision();
                    break;
                }
            case GameState.awaitingRoll:
                {
                    HandleRoll();
                    break;
                }
            case GameState.awaitingBuyDecision:
                {
                    BuyHandle();
                    break;
                }
            case GameState.awaitingUpgradeDecision:
                {
                    BuyHandle(false);
                    break;
                }
            case GameState.awaitingPayPlayer:
                {
                    PayPlayerHandle();
                    break;
                }
            case GameState.awaitingSpecialCard:
                {
                    //TODO
                    break;
                }
        }
    }
    private void HandleNextTurn()
    {
        playerTurnCanva.SetActive(false);
        cardCanva.SetActive(false);
        buyCanva.SetActive(false);
        upgradeCanva.SetActive(false);

        if (dublet)
        {
            dublet = false;
            isStateEnded = true;
            AddEvent(GameState.awaitingRollDecision);
        }
        else
        {
            NextPlayer();
        }
    }

    private void HandleRollDecision()
    {
        rollButton.interactable = true;
        surrenderButton.interactable = true;
        playerTurnCanva.SetActive(true);
    }

    private void HandleRoll()
    {
        playerTurnCanva.SetActive(false);

        int k1 = Random.Range(1, 6);
        int k2 = Random.Range(1, 6);

        int num = k1 + k2;
        if (k1 == k2)
        {
            AddEvent("Double Throw!");
            dublet = true;
        }
        else
        {
            AddEvent($"{k1} : {k2}");
        }

        board.MovePlayer(players[currentPlayerIndex], num);
    }

    private void BuyHandle(bool buy = true)
    {
        Player cp = players[currentPlayerIndex];
        BoardField bf = board.fields[cp.currentPosition];
        
        if (buy)
        {
            string s = $"Do You Want To Buy {bf.property.fieldname} For {bf.property.price}?";
            buyText.text = s;
            buyCanva.SetActive(true);
            return;
        }

        string s2 = $"Do You Want To Upgrade {bf.property.fieldname}?";
        upgradeText.text = s2;

        upgradeSelectionIndex = -1;
        foreach(Button b in upgrades)
        {
            b.GetComponent<Image>().color = defaultColor;
        }

        upgradeCanva.SetActive(true);
    }

    private void PayPlayerHandle()
    {
        Player cp = players[currentPlayerIndex];
        BoardField bf = board.fields[cp.currentPosition];
        Player owner = bf.property.owner;

        string t = $"Player {cp.playerName} Paid ${bf.property.currentVisitPrice} To {bf.property.owner.playerName}!";
        StartCoroutine(IEPayPlayerEvent(t, cp, owner, bf.property.currentVisitPrice));
    }

    #endregion 


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

    public IEnumerator IEPayPlayerEvent(string ask, Player payer, Player owner, int price)
    {
        payer.money -= price;
        owner.money += price;

        transitionText.text = ask;
        transitionCanva.SetActive(true);
        yield return new WaitForSeconds(2f);
        transitionCanva.SetActive(false);

        isStateEnded = true;
        isEventEnded = true;
        yield return null;
    }
}

public enum GameState
{
    idle,
    awaitingRoll,
    awaitingRollDecision,
    awaitingBuyDecision,
    awaitingUpgradeDecision,
    awaitingPayPlayer,
    awaitingSpecialCard,
    endTurn
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
    GameState state;
    public ChangeStateEvent(GameState nstate)
    {
        state = nstate;
    }
    public override void RunEvent()
    {
        GameManager.instance.ChangeState(state);
        GameManager.instance.isEventEnded = true;
    }
}