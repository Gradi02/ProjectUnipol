using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BoardField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI visitPrice, fname;
    [SerializeField] private Image bg, ramka;

    private const int maxLevel = 4;
    public PropertyField property;

    private readonly Vector3[][] offsets =
    {
        new[] 
        {
            new Vector3(0, 0.25f, 0) 
        },
        new[]
        {
            new Vector3(0, 0.25f, 0.15f),
            new Vector3(0, 0.25f, -0.15f)
        },
        new[]
        {
            new Vector3(0.15f, 0.25f, 0.15f),
            new Vector3(0.15f, 0.25f, -0.15f),
            new Vector3(-0.15f, 0.25f, 0)
        },
        new[] 
        { 
            new Vector3(0.15f, 0.25f, 0.15f),
            new Vector3(0.15f, 0.25f, -0.15f),
            new Vector3(-0.15f, 0.25f, -0.15f),
            new Vector3(-0.15f, 0.25f, 0.15f)
        }
    };

    private void Start()
    {
        fname.text = property.fieldname;

        if(property.ftype == FieldsType.Normal)
            visitPrice.text = "";
    }




    public void OnPlayerVisitEnter(Player pl)
    {
        List<Player> phere = new List<Player>();

        foreach(Player p in GameManager.instance.players)
        {
            if(p.currentPosition == pl.currentPosition)
            {
                phere.Add(p);
            }
        }

        for(int i=0; i<phere.Count; i++)
        {
            phere[i].gameObject.transform.position = transform.position + offsets[phere.Count - 1][i];
        }
    }

    public void OnPlayerVisitExit(Player pl)
    {
        List<Player> phere = new List<Player>();

        foreach (Player p in GameManager.instance.players)
        {
            if (p.currentPosition == pl.currentPosition && p != pl)
            {
                phere.Add(p);
            }
        }

        for (int i = 0; i < phere.Count; i++)
        {
            phere[i].gameObject.transform.position = transform.position + offsets[phere.Count - 1][i];
        }
    }

    public GameEvent OnPlayerLand(Player pl)
    {
        Debug.Log($"Gracz {pl.playerName} stoi na polu {property.fieldname}");
        
        if(property.ftype == FieldsType.Normal)
        {
            if(property.owner == null)
            {
                if(pl.money >= property.price)
                {
                    string s = $"Do You Want To Buy {property.fieldname} For {property.price}?";
                    return new BuyEvent(s);
                }
                else
                {
                    string t = $"Player {pl.playerName} Have Not Enought Money To Buy This Tile!";
                    return new InfoEvent(t);
                }
            }
            else if(property.owner == pl)
            {
                if(property.level < maxLevel)
                {
                    if (pl.money >= property.upgradePrice[property.level])
                    {
                        string s = $"Do You Want To Upgrade {property.fieldname} For {property.price} To Level {property.level+2}?";
                        return new BuyEvent(s);
                    }
                    else
                    {
                        string t = $"Player {pl.playerName} Have Not Enought Money To Upgrade This Tile!";
                        return new InfoEvent(t);
                    }
                }
            }
            else
            {
                if(pl.money >= property.currentVisitPrice)
                {
                    string t = $"Player {pl.playerName} Paid ${property.currentVisitPrice} To {property.owner.playerName}!";
                    return new PayToPlayerEvent(t, pl, property.owner, property.currentVisitPrice);
                }
                else
                {
                    string t = $"Player {pl.playerName} Have Not Enought Money To Pay To Player {property.owner.playerName}!";
                    return new InfoEvent(t);
                }
            }
        }
        else
        {
            //los lub karta specjalna
            return new InfoEvent("TODO: karta specjalna");
        }

        return null;
    }





    public void ResetField()
    {
        property.owner = null;
    }

    public void OnBuy(Player pl)
    {
        if(property.currentVisitPrice < 1000000)
        {
            visitPrice.text = $"{property.currentVisitPrice/1000}K";
        }
        else
        {
            visitPrice.text = $"{property.currentVisitPrice/1000000}M";
        }
    }

    public enum FieldsType
    {
        Normal,
        Card,
        Special
    }
}

[System.Serializable]
public class PropertyField
{
    public BoardField.FieldsType ftype = BoardField.FieldsType.Normal;
    public string fieldname;
    public bool canBuy = true;
    public int price = 10000;
    public Player owner = null;
    public int currentVisitPrice = 10000;

    public int level = 0;
    public int[] upgradePrice;

}