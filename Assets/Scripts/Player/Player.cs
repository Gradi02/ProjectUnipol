using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerOverlayCard overlayCard { get; private set; }
    public string playerName { get; set; }
    public int money { get; set; } = 1000000;
    public List<Card> ownedProperties { get; private set; }
    public bool isActive { get; set; } = true;
    public int currentPosition { get; set; } = 0;
    public int offsetIndex { get; set; }


    public void SetUp(PlayerOverlayCard c)
    {
        overlayCard = c;
    }
}
