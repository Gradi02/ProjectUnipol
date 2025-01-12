using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class PlayerUICard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private string titleText;
    [SerializeField] private Color m_Color, disabledColor;
    [SerializeField] private Image colorImage;
    [SerializeField] private Image readyButton;
    [SerializeField] private Button readyButtonInteraction;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TMP_InputField username;
    public TextMeshProUGUI multiUsername;

    public bool ready { get; private set; } = false;
    public string usernameText { get; set; }
    public Color bgc { get; private set; }


    private void Start()
    {
        colorImage.color = disabledColor;
        buttonText.text = "Not Ready!";
        readyButton.color = Color.red;
        title.text = titleText;
        bgc = m_Color;
    }

    private void Update()
    {
        if (GameManager.instance.isMultiplayer) return;

        if(username.text != string.Empty)
        {
            readyButtonInteraction.interactable = true;
        }
        else
        {
            readyButtonInteraction.interactable = false;

            if(ready)
            {
                SwitchReady();
            }
        }
    }

    public void SwitchReady()
    {
        if (GameManager.instance.isMultiplayer)
        {
            NetworkGameManager.instance.SwitchPlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            ready = !ready;

            if (ready)
            {
                bgc = m_Color;
                colorImage.color = m_Color;
                buttonText.text = "Ready!";
                readyButton.color = Color.green;
                usernameText = username.text;
            }
            else
            {
                buttonText.text = "Not Ready!";
                readyButton.color = Color.red;
                colorImage.color = disabledColor;
            }
        }
    }




    public void SwitchMultiReady(bool b)
    {
        ready = b;
        if (b)
        {
            bgc = m_Color;
            colorImage.color = m_Color;
            buttonText.text = "Ready!";
            readyButton.color = Color.green;
        }
        else
        {
            buttonText.text = "Not Ready!";
            readyButton.color = Color.red;
            colorImage.color = disabledColor;
        }
    }
}
