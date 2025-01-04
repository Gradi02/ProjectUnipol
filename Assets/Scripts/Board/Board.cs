using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static CardsPricings pricingsInstance;
    public BoardField[] fields;
    public Country[] countries;
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

    private void Start()
    {
        foreach(var b in countries)
        {
            foreach(BoardField f in b.fields)
            {
                f.SetCountryColor(b.color);
            }
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
        float timeTick = 0.2f;
        for(int i=0; i<num; i++)
        {
            fields[pl.currentPosition].OnPlayerVisitExit(pl);
            pl.currentPosition++;

            if(pl.currentPosition >= fields.Length)
                pl.currentPosition = 0;

            fields[pl.currentPosition].OnPlayerVisitEnter(pl, timeTick);


            //Debug.Log(fields[pl.currentPosition].transform.position);

            yield return new WaitForSeconds(timeTick);
        }

        if (pl.ownedCards.Count > 0)
        {
            for(int i=0; i<pl.ownedCards.Count; i++)
            {
                if (pl.ownedCards[i] != null)
                {
                    pl.ownedCards[i].RunCardEventOnPlayerLand();
                }
            }

            pl.ownedCards.RemoveAll(card =>
            {
                if (card.readyToDestroy)
                {
                    return true;
                }
                return false;
            });
        }


        GameManager.instance.isStateEnded = true;
        fields[pl.currentPosition].OnPlayerLand(pl);
    }
}

[System.Serializable]
public class Country
{
    public Color color;
    public BoardField[] fields;
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
                visitPrice = new int[] { 15000, 45000, 1050000, 180000 }
            }
        },

        {
            "Balbina",
            new CardsProperties
            {
                price = 6000,
                upgradePreices = new int[] { 27000, 65000, 160000 },
                visitPrice = new int[] { 18000, 50000, 110000, 185000 }
            }
        },

        {
            "Lumumby",
            new CardsProperties
            {
                price = 8000,
                upgradePreices = new int[] { 30000, 70000, 170000 },
                visitPrice = new int[] { 20000, 45000, 115000, 200000 }
            }
        },

        {
            "Botanik",
            new CardsProperties
            {
                price = 10000,
                upgradePreices = new int[] { 40000, 80000, 180000 },
                visitPrice = new int[] { 25000, 60000, 120000, 210000 }
            }
        },
                
        {
            "Administracja",
            new CardsProperties
            {
                price = 11000,
                upgradePreices = new int[] { 45000, 85000, 1850000 },
                visitPrice = new int[] { 26000, 65000, 130000, 220000 }
            }
        },

        {
            "Archiwum",
            new CardsProperties
            {
                price = 12000,
                upgradePreices = new int[] { 60000, 100000, 200000 },
                visitPrice = new int[] { 30000, 80000, 140000, 255000 }
            }
        },

        {
            "Pracownia",
            new CardsProperties
            {
                price = 15000,
                upgradePreices = new int[] { 70000, 120000, 220000 },
                visitPrice = new int[] { 35000, 85000, 150000, 275000 }
            }
        },

        {
            "Aula",
            new CardsProperties
            {
                price = 16000,
                upgradePreices = new int[] { 75000, 130000, 230000 },
                visitPrice = new int[] { 38000, 88000, 160000, 285000 }
            }
        },

        {
            "Patio",
            new CardsProperties
            {
                price = 18000,
                upgradePreices = new int[] { 80000, 140000, 240000 },
                visitPrice = new int[] { 40000, 90000, 175000, 300000 }
            }
        },

        {
            "Manu",
            new CardsProperties
            {
                price = 20000,
                upgradePreices = new int[] { 85000, 150000, 250000 },
                visitPrice = new int[] { 46000, 96000, 185000, 325000 }
            }
        },

        {
            "Sukcesja",
            new CardsProperties
            {
                price = 22000,
                upgradePreices = new int[] { 90000, 160000, 260000 },
                visitPrice = new int[] { 50000, 100000, 195000, 340000 }
            }
        },

        {
            "BU£",
            new CardsProperties
            {
                price = 25000,
                upgradePreices = new int[] { 100000, 190000, 280000 },
                visitPrice = new int[] { 60000, 120000, 205000, 350000 }
            }
        },

        {
            "Rektorat",
            new CardsProperties
            {
                price = 28000,
                upgradePreices = new int[] { 110000, 200000, 295000 },
                visitPrice = new int[] { 65000, 130000, 225000, 360000 }
            }
        },

        {
            "Biedronka",
            new CardsProperties
            {
                price = 30000,
                upgradePreices = new int[] { 120000, 220000, 305000 },
                visitPrice = new int[] { 65000, 140000, 245000, 380000 }
            }
        },

        {
            "Lidl",
            new CardsProperties
            {
                price = 32000,
                upgradePreices = new int[] { 130000, 240000, 325000 },
                visitPrice = new int[] { 70000, 150000, 255000, 390000 }
            }
        },

        {
            "¯abka",
            new CardsProperties
            {
                price = 35000,
                upgradePreices = new int[] { 140000, 260000, 350000 },
                visitPrice = new int[] { 75000, 160000, 265000, 400000 }
            }
        },

        {
            "Prawo",
            new CardsProperties
            {
                price = 40000,
                upgradePreices = new int[] { 150000, 270000, 360000 },
                visitPrice = new int[] { 85000, 170000, 280000, 430000 }
            }
        },

        {
            "Eksoc",
            new CardsProperties
            {
                price = 42000,
                upgradePreices = new int[] { 165000, 285000, 375000 },
                visitPrice = new int[] { 90000, 180000, 290000, 450000 }
            }
        },

        {
            "WFiIS",
            new CardsProperties
            {
                price = 45000,
                upgradePreices = new int[] { 180000, 305000, 390000 },
                visitPrice = new int[] { 95000, 190000, 300000, 480000 }
            }
        },

        {
            "Nabla",
            new CardsProperties
            {
                price = 51000,
                upgradePreices = new int[] { 190000, 325000, 400000 },
                visitPrice = new int[] { 100000, 200000, 320000, 500000 }
            }
        },

        {
            "Artefakty",
            new CardsProperties
            {
                price = 55000,
                upgradePreices = new int[] { 200000, 350000, 420000 },
                visitPrice = new int[] { 105000, 220000, 350000, 550000 }
            }
        },
    };
}

