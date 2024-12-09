using UnityEngine;

public class BoardField : MonoBehaviour
{
    private const int maxLevel = 4;
    public PropertyField property;

    private void Start()
    {
        property.defaultMat = gameObject.GetComponent<MeshRenderer>().material;
    }




    public void OnPlayerVisitEnter()
    {

    }

    public void OnPlayerVisitExit()
    {

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
                    string s = $"Do You Want To Buy {property.fieldname} For {property.price}?";
                    GameManager.instance.AskForBuyOrUpgrade(s);
                    return;
                }
                else
                {
                    string t = $"Player {pl.playerName} Have Not Enought Money To Buy This Tile!";
                    GameManager.instance.NotEnoughtMoneyInfo(t);
                    return;
                }
            }
            else if(property.owner == pl)
            {
                if(property.level < maxLevel)
                {
                    if (pl.money >= property.upgradePrice[property.level])
                    {
                        string s = $"Do You Want To Upgrade {property.fieldname} For {property.price} To Level {property.level+2}?";
                        GameManager.instance.AskForBuyOrUpgrade(s);
                        return;
                    }
                    else
                    {
                        string t = $"Player {pl.playerName} Have Not Enought Money To Upgrade This Tile!";
                        GameManager.instance.NotEnoughtMoneyInfo(t);
                        return;
                    }
                }
            }
            else
            {
                if(pl.money >= property.currentVisitPrice)
                {
                    string t = $"Player {pl.playerName} Paid ${property.currentVisitPrice} To {property.owner.playerName}!";
                    GameManager.instance.PaidAction(pl, property.owner, property.currentVisitPrice, t);
                    return;
                }
                else
                {
                    string t = $"Player {pl.playerName} Have Not Enought Money To Pay To Player {property.owner.playerName}!";
                    GameManager.instance.NotEnoughtMoneyInfo(t);
                    return;
                }
            }
        }
        else
        {
            GameManager.instance.StandOnSpecialCard();
            return;
        }

        GameManager.instance.ElseState();
    }





    public void ResetField()
    {
        property.owner = null;
        gameObject.GetComponent<MeshRenderer>().material = property.defaultMat;
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
    public Material defaultMat { get; set; }



    public BoardField.FieldsType ftype = BoardField.FieldsType.Normal;
    public string fieldname;
    public bool canBuy = true;
    public int price = 10000;
    public Player owner = null;
    public int currentVisitPrice = 10000;

    public int level = 0;
    public int[] upgradePrice;

}