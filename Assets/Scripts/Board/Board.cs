using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static CardsPricings pricingsInstance;
    public BoardField[] fields;
    private Vector3[] offsets = {
        new Vector3(0.15f, 0.25f, 0.15f),
        new Vector3(0.15f, 0.25f, -0.15f),
        new Vector3(-0.15f, 0.25f, -0.15f),
        new Vector3(-0.15f, 0.25f, 0.15f),
    };

    private void Awake()
    {
        if(pricingsInstance == null)
        {
            pricingsInstance = new CardsPricings();
        }
    }

    public void SetupPlayers(List<Player> pl)
    {
        for(int i = 0; i < pl.Count; i++)
        {
            pl[i].gameObject.transform.position = fields[0].transform.position + offsets[i];
        }
    }

    public void MovePlayer(Player pl, int num)
    {
        StartCoroutine(IEMovePlayer(pl, num));
    }

    private IEnumerator IEMovePlayer(Player pl, int num)
    {
        for(int i=0; i<num; i++)
        {
            fields[pl.currentPosition].OnPlayerVisitExit(pl);
            pl.currentPosition++;
            if(pl.currentPosition >= fields.Length)
                pl.currentPosition = 0;
            fields[pl.currentPosition].OnPlayerVisitEnter(pl);
            pl.OnMove();
            yield return new WaitForSeconds(0.2f);
        }

        if (pl.ownedCards.Count > 0)
        {
            foreach (Card card in pl.ownedCards)
            {
                card.RunCardEventOnPlayerLand();
            }
        }

        GameManager.instance.isStateEnded = true;
        fields[pl.currentPosition].OnPlayerLand(pl);
    }
}
public class CardsProperties
{
    public int price;//1
    public int[] upgradePreices;//3
    public int[] visitPrice;//4
}
public class CardsPricings
{
    public Dictionary<string, CardsProperties> prices = new Dictionary<string, CardsProperties>
    {
        {
            "Dworzec MPK",
            new CardsProperties
            {
                price = 50000,
                upgradePreices = new int[] { 0, 0, 0 },
                visitPrice = new int[] { 10000, 20000, 30000, 40000 }
            }
        },

        {
            "Olimp",
            new CardsProperties
            {
                price = 5000,
                upgradePreices = new int[] { 25000, 60000, 150000 },
                visitPrice = new int[] { 15000, 45000, 70000, 180000 }
            }
        },

        {
            "Balbina",
            new CardsProperties
            {
                price = 6000,
                upgradePreices = new int[] { 27000, 65000, 160000 },
                visitPrice = new int[] { 18000, 50000, 75000, 185000 }
            }
        },

        {
            "Lumumby",
            new CardsProperties
            {
                price = 8000,
                upgradePreices = new int[] { 30000, 70000, 170000 },
                visitPrice = new int[] { 20000, 45000, 80000, 200000 }
            }
        },

        {
            "Botanik",
            new CardsProperties
            {
                price = 10000,
                upgradePreices = new int[] { 40000, 80000, 180000 },
                visitPrice = new int[] { 25000, 60000, 100000, 210000 }
            }
        },
                
        {
            "Administracja",
            new CardsProperties
            {
                price = 11000,
                upgradePreices = new int[] { 45000, 85000, 1850000 },
                visitPrice = new int[] { 26000, 65000, 110000, 220000 }
            }
        },
                
        {
            "Planetarium",
            new CardsProperties
            {
                price = 12000,
                upgradePreices = new int[] { 55000, 90000, 190000 },
                visitPrice = new int[] { 28000, 70000, 120000, 230000 }
            }
        },

        {
            "Archiwum",
            new CardsProperties
            {
                price = 13000,
                upgradePreices = new int[] { 60000, 100000, 200000 },
                visitPrice = new int[] { 30000, 80000, 140000, 255000 }
            }
        },

        {
            "Matematyka",
            new CardsProperties
            {
                price = 15000,
                upgradePreices = new int[] { 70000, 115000, 220000 },
                visitPrice = new int[] { 40000, 90000, 140000, 275000 }
            }
        },       
        
        {
            "Fizyka",
            new CardsProperties
            {
                price = 17000,
                upgradePreices = new int[] { 75000, 130000, 230000 },
                visitPrice = new int[] { 45000, 95000, 145000, 285000 }
            }
        },

        {
            "Informatyka",
            new CardsProperties
            {
                price = 19000,
                upgradePreices = new int[] { 80000, 145000, 250000 },
                visitPrice = new int[] { 50000, 100000, 150000, 295000 }
            }
        },

        {
            "Biologia",
            new CardsProperties
            {
                price = 20000,
                upgradePreices = new int[] { 90000, 150000, 280000 },
                visitPrice = new int[] { 60000, 120000, 190000, 335000 }
            }
        },

        {
            "Manu",
            new CardsProperties
            {
                price = 25000,
                upgradePreices = new int[] { 95000, 160000, 320000 },
                visitPrice = new int[] { 70000, 145000, 220000, 405000 }
            }
        },

        {
            "Biedronka",
            new CardsProperties
            {
                price = 28000,
                upgradePreices = new int[] { 98000, 180000, 340000 },
                visitPrice = new int[] { 80000, 150000, 230000, 425000 }
            }
        },

        {
            "Zahir",
            new CardsProperties
            {
                price = 30000,
                upgradePreices = new int[] { 100000, 200000, 380000 },
                visitPrice = new int[] { 95000, 155000, 250000, 450000 }
            }
        },

        {
            "Ekonomia",
            new CardsProperties
            {
                price = 35000,
                upgradePreices = new int[] { 120000, 220000, 410000 },
                visitPrice = new int[] { 105000, 180000, 280000, 470000 }
            }
        },

        {
            "Socjologia",
            new CardsProperties
            {
                price = 37000,
                upgradePreices = new int[] { 130000, 230000, 420000 },
                visitPrice = new int[] { 110000, 190000, 290000, 480000 }
            }
        },

        {
            "Zarz¹dzanie",
            new CardsProperties
            {
                price = 40000,
                upgradePreices = new int[] { 140000, 240000, 440000 },
                visitPrice = new int[] { 120000, 210000, 310000, 520000 }
            }
        },

        {
            "Filologia",
            new CardsProperties
            {
                price = 50000,
                upgradePreices = new int[] { 150000, 250000, 450000 },
                visitPrice = new int[] { 140000, 230000, 330000, 550000 }
            }
        },

        {
            "Anglistyka",
            new CardsProperties
            {
                price = 55000,
                upgradePreices = new int[] { 160000, 260000, 470000 },
                visitPrice = new int[] { 150000, 240000, 340000, 560000 }
            }
        },

        {
            "Psychologia",
            new CardsProperties
            {
                price = 60000,
                upgradePreices = new int[] { 175000, 290000, 485000 },
                visitPrice = new int[] { 160000, 250000, 350000, 585000 }
            }
        },

        {
            "Historia",
            new CardsProperties
            {
                price = 65000,
                upgradePreices = new int[] { 190000, 300000, 510000 },
                visitPrice = new int[] { 170000, 270000, 405000, 600000 }
            }
        },

        {
            "Patio",
            new CardsProperties
            {
                price = 75000,
                upgradePreices = new int[] { 220000, 350000, 580000 },
                visitPrice = new int[] { 190000, 310000, 455000, 690000 }
            }
        },

        {
            "Pracownia",
            new CardsProperties
            {
                price = 85000,
                upgradePreices = new int[] { 240000, 385000, 610000 },
                visitPrice = new int[] { 195000, 330000, 475000, 705000 }
            }
        },

        {
            "Piwnica",
            new CardsProperties
            {
                price = 95000,
                upgradePreices = new int[] { 250000, 400000, 650000 },
                visitPrice = new int[] { 200000, 350000, 500000, 740000 }
            }
        },

        {
            "Aula",
            new CardsProperties
            {
                price = 105000,
                upgradePreices = new int[] { 270000, 450000, 690000 },
                visitPrice = new int[] { 210000, 380000, 550000, 820000 }
            }
        },

        {
            "BU£",
            new CardsProperties
            {
                price = 120000,
                upgradePreices = new int[] { 310000, 515000, 720000 },
                visitPrice = new int[] { 225000, 450000, 650000, 915000 }
            }
        },

        {
            "Rektorat",
            new CardsProperties
            {
                price = 130000,
                upgradePreices = new int[] { 350000, 550000, 800000 },
                visitPrice = new int[] { 240000, 470000, 685000, 990000 }
            }
        },

        {
            "Artefakty",
            new CardsProperties
            {
                price = 150000,
                upgradePreices = new int[] { 400000, 650000, 950000 },
                visitPrice = new int[] { 255000, 490000, 760000, 1100000 }
            }
        },
    };
}

