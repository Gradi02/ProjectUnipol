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
    public bool isActive { get; set; } = true;
    public int currentPosition { get; set; } = 0;


    public void SetUp(PlayerOverlayCard c, string usn)
    {
        overlayCard = c;
        playerName = usn;
        ownedProperties = new List<BoardField>();
    }

    public void Surrender()
    {
        isActive = false;
        money = 0;

        if (ownedProperties.Count > 0)
        {
            foreach (BoardField field in ownedProperties)
            {
                field.ResetField();
            }
        }

        overlayCard.SetCashText("FAILED");
    }

    public void AddFieldToList(BoardField f)
    {
        ownedProperties.Add(f);
    }

    string FormatMoney(int amount)
    {
        return "$" + amount.ToString("N0", System.Globalization.CultureInfo.InvariantCulture).Replace(',', '.');
    }
}
