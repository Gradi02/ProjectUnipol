using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class BoardField : MonoBehaviour
{
    protected GameManager gameManager => GameManager.instance;
    [SerializeField] protected TextMeshProUGUI fname;
    [SerializeField] protected Image priceBG;
    public GameObject selectedBox;
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



    public void SetCountryColor(Color c)
    {
        if(priceBG != null)
        {
            priceBG.color = c;
        }
    }


    public void OnPlayerVisitEnter(Player pl, float time)
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
            //phere[i].gameObject.transform.position = transform.position + offsets[phere.Count - 1][i];


            //animacja move graczy 
            Vector3 end = transform.position + offsets[phere.Count - 1][i];
            GameObject playerTemp = phere[i].gameObject;


            LeanTween.moveY(gameObject, gameObject.transform.position.y - 0.06f, time/2).setDelay(time/2).setEase(LeanTweenType.easeInOutSine);
            LeanTween.moveY(gameObject, gameObject.transform.position.y, time/2).setDelay(time).setEase(LeanTweenType.easeInOutSine);

            LeanTween.scale(playerTemp, new Vector3(0.25f, 0.25f, 0.25f), time/2).setEase(LeanTweenType.easeInOutSine);
            LeanTween.scale(playerTemp, new Vector3(0.2f, 0.2f, 0.2f), time / 2).setDelay(time / 2).setEase(LeanTweenType.easeInOutSine);

            LeanTween.moveX(playerTemp, end.x, time).setEase(LeanTweenType.easeInOutSine);
            LeanTween.moveZ(playerTemp, end.z, time).setEase(LeanTweenType.easeInOutSine);
            LeanTween.moveY(playerTemp, end.y + 0.3f, time / 2).setEase(LeanTweenType.easeInOutSine);
            LeanTween.moveY(playerTemp, end.y, time / 2).setDelay(time / 2).setEase(LeanTweenType.easeInOutSine);

        }

        if (this is StartField)
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
}

[System.Serializable]
public class PropertyField
{
    public string fieldname;
    public Player owner = null;
    [HideInInspector] public int currentValue = 0;
    [HideInInspector] public int level = 0;
    [HideInInspector] public int price => Board.pricingsInstance.prices[fieldname].price;
    [HideInInspector] public int[] upgradePrices => Board.pricingsInstance.prices[fieldname].upgradePreices;
    [HideInInspector] public int[] visitPrices => Board.pricingsInstance.prices[fieldname].visitPrice;
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