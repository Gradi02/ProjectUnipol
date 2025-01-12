using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class PlayerOverlayCard : MonoBehaviour
{
    [SerializeField] private Color disabledColor;
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI username, cash;


    public void Setup(PlayerUICard uic)
    {
        username.text = uic.usernameText;
        img.color = uic.bgc;
        cash.text = "$1.000.000";
    }

    public void Disabled()
    {
        img.color = disabledColor;
        username.text = "Empty :(";
        cash.text = "$0";
    }

    public void SetCashText(string s)
    {
        cash.text = s;
    }



    public void SetupNetworkVersion(string n, Color c)
    {
        username.text = n;
        img.color = c;
        cash.text = "$1.000.000";
    }
}
