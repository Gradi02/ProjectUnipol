using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class NormalField : BoardField, IOwnableProperty
{
    public TextMeshProUGUI visitPrice;
    public Image bg;
    public Transform buildingsTransform;
    public GameObject[] buildingsPrefs;
    private const int maxLevel = 5;
    private GameObject buildingsReference;

    [Header("Juwenalia")]
    [SerializeField] private GameObject juwenaliaModel;
    [SerializeField] private ParticleSystem fireworks;


    private void Start()
    {
        fname.text = property.fieldname;
        visitPrice.text = "";
        selectedBox.SetActive(false);
    }


    public override void OnPlayerLand(Player pl)
    {
        if (property.owner == null)
        {
            if (pl.money >= property.price)
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
        else if (property.owner == pl)
        {
            if (property.level < maxLevel - 1)
            {
                if (pl.money >= property.upgradePrices[property.level - 1])
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
            if (pl.money >= GetCurrentVisitPrice())
            {
                gameManager.AddEvent(gameManager.awaitPayPlayerState);
                return;
            }
            else
            {
                string t = $"Player {pl.playerName} Have Not Enought Money To Pay To Player {property.owner.playerName}!";
                gameManager.AddEvent(t);

                int maxMoney = pl.money;
                foreach(BoardField bf in pl.ownedProperties)
                {
                    maxMoney += bf.property.currentValue;
                }

                if(maxMoney >= GetCurrentVisitPrice())
                {
                    gameManager.AddEvent(gameManager.awaitingSellState);
                    return;
                }
                else
                {
                    pl.Surrender();
                    gameManager.AddEvent(gameManager.endTurnS);
                    return;
                }
            }
        }
    }



    public int GetCurrentVisitPrice()
    {
        if(gameManager.juwenaliaField == this)
            return Mathf.RoundToInt(property.visitPrices[property.level - 1] * gameManager.juwenaliaPricing[gameManager.juwenaliaPricePointer]);

        return property.visitPrices[property.level - 1];
    }

    public void ResetField()
    {
        property.owner = null;
        property.level = 0;
        visitPrice.text = "";
        UpdateBuildingCount();

        if (gameManager.juwenaliaField == this)
        {
            gameManager.juwenaliaField = null;
            EndJuwenalia();
        }
    }
    public void OnBuy(Player pl)
    {
        property.currentValue += property.price/2;
        property.owner = pl;
        property.level = 1;

        UpdateBuildingCount();
        FormatString();
    }

    public void OnUpgrade(int upgrIdx)
    {
        int previousLevel = property.level;
        property.level = upgrIdx + 1;

        int prc = 0;
        for (int i = previousLevel; i < property.level; i++)
        {
            prc += property.upgradePrices[i - 1];
        }
        property.owner.money -= prc;
        property.currentValue += prc / 2;

        UpdateBuildingCount();
        FormatString();
    }


    public void UpdateBuildingCount()
    {
        if (buildingsReference != null)
        {
            Destroy(buildingsReference);
            buildingsReference = null;
        }

        if (property.level - 1 >= 0)
        {
            buildingsReference = Instantiate(buildingsPrefs[property.level - 1], buildingsTransform.position, transform.rotation);
            buildingsReference.transform.SetParent(transform, true);

            RoofMarker[] roofs = buildingsReference.GetComponentsInChildren<RoofMarker>();
            foreach (RoofMarker roofMarker in roofs)
            {
                roofMarker.GetComponent<MeshRenderer>().material = property.owner.GetComponent<MeshRenderer>().material;
            }
        }
    }

    public void FormatString()
    {
        if (property.owner != null)
        {
            if (GetCurrentVisitPrice() < 1000000)
            {
                visitPrice.text = $"{GetCurrentVisitPrice() / 1000}K";
            }
            else
            {
                visitPrice.text = $"{GetCurrentVisitPrice() / 1000000}M";
            }
        }
    }



    public void StartJuwenalia()
    {
        if (gameManager.juwenaliaField != null)
        {
            NormalField end = gameManager.juwenaliaField;
            gameManager.juwenaliaField = null;
            end.EndJuwenalia();
        }

        gameManager.juwenaliaField = this;
        gameManager.juwenaliaPricePointer++;
        if (gameManager.juwenaliaPricePointer >= gameManager.juwenaliaPricing.Length)
            gameManager.juwenaliaPricePointer = gameManager.juwenaliaPricing.Length - 1;

        juwenaliaModel.SetActive(true);
        fireworks.Play();

        FormatString();
    }

    public void EndJuwenalia()
    {
        juwenaliaModel.SetActive(false);
        FormatString();
    }
}
