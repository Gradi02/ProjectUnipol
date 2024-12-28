using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class BoardField : MonoBehaviour
{
    protected GameManager gameManager => GameManager.instance;
    [SerializeField] protected TextMeshProUGUI fname;
    [SerializeField] protected Image ramka;
    private int startMoneyBonus = 100000;
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




    public void OnPlayerVisitEnter(Player pl)
    {
        List<Player> phere = new List<Player>();

        foreach(Player p in gameManager.players)
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

        if(this is StartField)
        {
            pl.money += startMoneyBonus;
        }
    }

    public void OnPlayerVisitExit(Player pl)
    {
        List<Player> phere = new List<Player>();

        foreach (Player p in gameManager.players)
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

    public abstract void OnPlayerLand(Player pl); 


    public void StartJuwenalia()
    {
        if(this is not NormalField)
        {

        }
    }
}

[System.Serializable]
public class PropertyField
{
    public string fieldname;
    public Player owner = null;
    [HideInInspector] public int level = 0;
    [HideInInspector] public int price => Board.pricingsInstance.prices[fieldname].price;
    [HideInInspector] public int[] upgradePrices => Board.pricingsInstance.prices[fieldname].upgradePreices;
    [HideInInspector] public int[] visitPrices => Board.pricingsInstance.prices[fieldname].visitPrice;
    [HideInInspector] public int currentValue = 0;
}


public interface IOwnableProperty
{
    public void OnBuy(Player pl);
    public void OnUpgrade(int upgrIdx);
    public void UpdateBuildingCount();
    public void FormatString();
    public void ResetField();
    public int GetCurrentVisitPrice();
}