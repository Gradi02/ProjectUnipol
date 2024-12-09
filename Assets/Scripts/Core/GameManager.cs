using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
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
        if(isGameStarted && canStartNextTurn)
        {
            if (dublet)
            {
                dublet = false;
                canStartNextTurn = false;
                ShowPlayerState(true);
            }
            else
            {
                NextPlayer();
            }
        }
    }


    //BUTTONS

    public void RollButton()
    {
        playerTurnCanva.SetActive(false);

        int k1 = Random.Range(1, 6);
        int k2 = Random.Range(1, 6);
        Debug.Log($"k1: {k1}, k2: {k2}");

        int num = k1 + k2;
        if (k1 == k2)
        {
            StartCoroutine(IETransitionState("Double Throw!"));
            dublet = true;
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
        buyCanva.SetActive(false);
        canStartNextTurn = true;
    }

    public void BuyButtonNo()
    {
        buyCanva.SetActive(false);
        canStartNextTurn = true;
    }



    //GameStates

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
        StartCoroutine(IETransitionState(t, true));
    }

    private void ShowPlayerState(bool show)
    {
        rollButton.interactable = true;
        surrenderButton.interactable = true;
        playerTurnCanva.SetActive(show);
    }


    //after landed on new tail returns function
    public void AskForBuyOrUpgrade(string ask)
    {
        buyText.text = ask;
        buyCanva.SetActive(true);
    }

    public void NotEnoughtMoneyInfo(string ask)
    {
        StartCoroutine(IETransitionState(ask, false, true));
    }

    public void StandOnSpecialCard()
    {
        string t = $"Karta Specjalne TODO: EVENT";
        StartCoroutine(IETransitionState(t, false, true));
    }

    public void PaidAction(Player payer, Player owner, int price, string p)
    {
        StartCoroutine(IETransitionState(p, false, true));
        payer.money -= price;
        owner.money += price;
    }

    public void ElseState()
    {
        canStartNextTurn = true;
    }



    private IEnumerator IETransitionState(string title, bool nextIE = false, bool nextPlayer = false)
    {
        transitionText.text = title;
        transitionCanva.SetActive(true);
        yield return new WaitForSeconds(2f);
        transitionCanva.SetActive(false);

        if (nextIE)
            ShowPlayerState(true);
        else if(nextPlayer)
            canStartNextTurn = true;
    }
}
