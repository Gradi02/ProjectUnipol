using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerOverlayCard overlayCard { get; private set; }
    public string playerName { get; set; }

    private int _money = 1000000;
    public int money 
    {
        get => _money;
        set
        {
            if (_money != value) 
            {
                _money = value;
                overlayCard.SetCashText(FormatMoney(money));
            }
        }
    }
    public List<BoardField> ownedProperties { get; private set; }
    public List<Card> ownedCards { get; private set; } = new List<Card>();
    public bool isActive { get; set; } = true;
    public int currentPosition { get; set; } = 0;
    public int stopTurns { get; set; } = 0;
    public ulong clientId { get; set; }

    public void SetUp(PlayerOverlayCard c, string usn)
    {
        overlayCard = c;
        playerName = usn;
        ownedProperties = new List<BoardField>();
    }

    public void Surrender()
    {
        string t = $"Gracz {playerName} zosta³ wyeliminowany!";
        GameManager.instance.AddEvent(t);

        isActive = false;
        money = 0;

        if (ownedProperties.Count > 0)
        {
            foreach (BoardField f in ownedProperties)
            {
                IOwnableProperty pr = f.GetComponent<IOwnableProperty>();
                if (pr != null)
                {
                    pr.ResetField();
                }
                else
                {
                    Debug.LogWarning($"{f} nie implementuje IOwnableProperty!");
                }
            }
        }

        ownedProperties.Clear();
        overlayCard.SetCashText("FAILED");

        GameManager.instance.CheckForWin();
    }

    public void AddFieldToList(BoardField f)
    {
        ownedProperties.Add(f);
    }


    public int GetMpksNumber()
    {
        int i = 0;
        foreach(BoardField field in ownedProperties)
        {     
            if (field is MPKField)
            {
                i++;
            }
        }
        return i;
    }

    string FormatMoney(int amount)
    {
        return "$" + amount.ToString("N0", System.Globalization.CultureInfo.InvariantCulture).Replace(',', '.');
    }
}
