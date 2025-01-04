using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MPKField : BoardField, IOwnableProperty
{
    public TextMeshProUGUI visitPrice;
    public Image bg;
    public Transform buildingTransforms;
    public GameObject buildingsPrefs;
    private const int maxLevel = 5;
    private GameObject buildingsReferences;


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
            gameManager.AddEvent(gameManager.endTurnS);
            return;
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
                foreach (BoardField bf in pl.ownedProperties)
                {
                    maxMoney += bf.property.currentValue;
                }

                if (maxMoney >= GetCurrentVisitPrice())
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
        return property.visitPrices[property.owner.GetMpksNumber() - 1];
    }

    public void ResetField()
    {
        property.owner = null;
        property.level = 0;
        UpdateBuildingCount();
        visitPrice.text = "";
    }

    public void OnBuy(Player pl)
    {
        property.currentValue += property.price / 2;
        property.owner = pl;
        property.level = maxLevel - 1;

        buildingsReferences = Instantiate(buildingsPrefs, buildingTransforms.position, transform.rotation);
        buildingsReferences.GetComponent<MeshRenderer>().material = property.owner.GetComponent<MeshRenderer>().material;
        buildingsReferences.transform.SetParent(transform, true);

        foreach (BoardField f in pl.ownedProperties)
        {
            IOwnableProperty pr = f.GetComponent<IOwnableProperty>();
            if (pr != null)
            {
                pr.FormatString();
            }
            else
            {
                Debug.LogWarning($"{f} nie implementuje IOwnableProperty!");
            }
        }
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
        if (property.level == maxLevel - 1)
        {
            for (int i = 0; i < maxLevel - 2; i++)
            {
                if (buildingsReferences != null)
                {
                    Destroy(buildingsReferences);
                    buildingsReferences = null;
                }
            }

            buildingsReferences = Instantiate(buildingsPrefs, buildingTransforms.position, transform.rotation);
            buildingsReferences.GetComponent<MeshRenderer>().material = property.owner.GetComponent<MeshRenderer>().material;
            buildingsReferences.transform.SetParent(transform, true);

            return;
        }

        for (int i = 0; i < maxLevel - 1; i++)
        {
            if (property.level > i)
            {
                if (buildingsReferences == null)
                {
                    buildingsReferences = Instantiate(buildingsPrefs, buildingTransforms.position, transform.rotation);
                    buildingsReferences.GetComponent<MeshRenderer>().material = property.owner.GetComponent<MeshRenderer>().material;
                    buildingsReferences.transform.SetParent(transform, true);
                }
            }
            else
            {
                if (buildingsReferences != null)
                {
                    Destroy(buildingsReferences);
                    buildingsReferences = null;
                }
            }
        }
    }

    public void FormatString()
    {
        if (property.owner != null)
        {
            float visitPriceValue = GetCurrentVisitPrice();
            if (visitPriceValue < 1000000)
            {
                visitPrice.text = $"{visitPriceValue / 1000f}K";
            }
            else
            {
                visitPrice.text = $"{visitPriceValue / 1000000f:F1}M";
            }
        }
    }
}
