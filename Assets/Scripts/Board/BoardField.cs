using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BoardField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI visitPrice, fname;
    [SerializeField] private Image bg, ramka;
    [SerializeField] private Transform[] buildingsTransforms;
    [SerializeField] private GameObject[] buildingsPrefs;


    private const int maxLevel = 5;
    private GameObject[] buildingsReferences = new GameObject[4];
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
    private GameManager gameManager => GameManager.instance;

    private void Start()
    {
        fname.text = property.fieldname;

        if(property.ftype == FieldsType.Normal)
            visitPrice.text = "";
    }




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

    public void OnPlayerLand(Player pl)
    {
        Debug.Log($"Gracz {pl.playerName} stoi na polu {property.fieldname}");
        
        if(property.ftype == FieldsType.Normal)
        {
            if(property.owner == null)
            {
                if(pl.money >= property.price)
                {
                    gameManager.AddEvent(gameManager.awaitBuyS);
                    return;
                }
                else
                {
                    string t = $"Player {pl.playerName} Have Not Enought Money To Buy This Tile!";
                    gameManager.AddEvent(t);
                    gameManager.AddEvent(gameManager.endTurnS);
                    return;
                }
            }
            else if(property.owner == pl)
            {
                if(property.level < maxLevel - 1)
                {
                    if (pl.money >= property.upgradePrices[property.level-1])
                    {
                        gameManager.AddEvent(gameManager.awaitUpgrDecS);
                        return;
                    }
                    else
                    {
                        string t = $"Player {pl.playerName} Have Not Enought Money To Upgrade This Tile!";
                        gameManager.AddEvent(t);
                        gameManager.AddEvent(gameManager.endTurnS);
                        return; 
                    }
                }
                else
                {
                    gameManager.AddEvent(gameManager.endTurnS);
                    return;
                }
            }
            else
            {
                if(pl.money >= property.currentVisitPrice)
                {
                    gameManager.AddEvent(gameManager.awaitPayPlayerState);
                    return;
                }
                else
                {
                    string t = $"Player {pl.playerName} Have Not Enought Money To Pay To Player {property.owner.playerName}!";
                    gameManager.AddEvent(t);
                    gameManager.AddEvent(gameManager.endTurnS);
                    return;
                }
            }
        }
        else
        {
            //los lub karta specjalna
            gameManager.AddEvent("TODO: karta specjalna");
            return;
        }
    }





    public void ResetField()
    {
        property.owner = null;
        property.level = 0;
        visitPrice.text = "";
        ChangeBuildingCount();
    }

    public void OnBuy(Player pl)
    {
        property.owner = pl;
        property.level = 1;

        ChangeBuildingCount();
        FormatString();
    }

    public void OnUpgrade(int upgrIdx)
    {
        int previousLevel = property.level;
        property.level = upgrIdx+1;

        int prc = 0;
        for(int i=previousLevel; i<property.level; i++)
        {
            prc += property.upgradePrices[i-1];
        }
        property.owner.money -= prc;

        ChangeBuildingCount();
        FormatString();
    }


    private void ChangeBuildingCount()
    {
        if(property.level == maxLevel-1)
        {
            for (int i = 0; i < maxLevel-2; i++)
            {
                if (buildingsReferences[i] != null)
                {
                    Destroy(buildingsReferences[i]);
                    buildingsReferences[i] = null;
                }
            }

            buildingsReferences[maxLevel - 2] = Instantiate(buildingsPrefs[maxLevel - 2], buildingsTransforms[maxLevel - 2].position, transform.rotation);
            buildingsReferences[maxLevel - 2].GetComponent<MeshRenderer>().material = property.owner.GetComponent<MeshRenderer>().material;
            buildingsReferences[maxLevel - 2].transform.SetParent(transform, true);

            return;
        }

        for(int i=0; i<maxLevel-1; i++)
        {
            if (property.level > i)
            {
                if (buildingsReferences[i] == null)
                {
                    buildingsReferences[i] = Instantiate(buildingsPrefs[i], buildingsTransforms[i].position, transform.rotation);
                    buildingsReferences[i].GetComponent<MeshRenderer>().material = property.owner.GetComponent<MeshRenderer>().material;
                    buildingsReferences[i].transform.SetParent(transform, true);
                }
            }
            else
            {
                if(buildingsReferences[i] != null)
                {
                    Destroy(buildingsReferences[i]);
                    buildingsReferences[i] = null;
                }
            }
        }
    }

    private void FormatString()
    {
        if (property.currentVisitPrice < 1000000)
        {
            visitPrice.text = $"{property.currentVisitPrice / 1000}K";
        }
        else
        {
            visitPrice.text = $"{property.currentVisitPrice / 1000000}M";
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

    public int level = 0;
    public int currentVisitPrice => visitPrices[level-1];
    public int[] upgradePrices = { 25000, 50000, 200000 };
    private int[] visitPrices = { 5000, 20000, 50000, 100000 };
}