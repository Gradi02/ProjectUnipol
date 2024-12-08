using UnityEngine;
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
    private bool ready = false;


    private void Start()
    {
        colorImage.color = disabledColor;
        buttonText.text = "Not Ready!";
        readyButton.color = Color.red;
        title.text = titleText;
    }

    private void Update()
    {
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
        ready = !ready;

        if(ready)
        {
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
