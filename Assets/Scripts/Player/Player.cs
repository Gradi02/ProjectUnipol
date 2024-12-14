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

    [SerializeField] private ParticleSystem ps;

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

    public void OnMove()
    {
        ps.Play();
    }

    public int GetMpksNumber()
    {
        int i = 0;
        foreach(BoardField field in ownedProperties)
        {
            if(field.property.ftype == BoardField.FieldsType.MPK)
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
