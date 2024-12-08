using UnityEngine;

public class BoardField : MonoBehaviour
{
    public PropertyField property;

    

    public void OnPlayerLand(Player pl)
    {
        Debug.Log($"Gracz {pl.playerName} stoi na polu {property.fieldname}");
    }
}

[System.Serializable]
public class PropertyField
{
    public string fieldname;
    public int price;
    public Player owner;


}