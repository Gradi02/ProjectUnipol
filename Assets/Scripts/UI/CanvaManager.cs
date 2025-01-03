using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvaManager : MonoBehaviour
{
    [SerializeField] private GameObject canva;
    [SerializeField] protected TextMeshProUGUI canvaText;
    [SerializeField] private Button roll, surrender, buyY, buyN, confirmSell;

    public void SetCanvaActivity(bool canvaActivity, bool rollButton, bool surrButton, bool buyYes, bool buyNo, bool cSell, string t)
    {
        if(canvaActivity)
        {
            confirmSell.interactable = true;
            roll.gameObject.SetActive(rollButton);
            surrender.gameObject.SetActive(surrButton);
            buyY.gameObject.SetActive(buyYes);
            buyN.gameObject.SetActive(buyNo);
            confirmSell.gameObject.SetActive(cSell);

            canvaText.text = t;
        }

        canva.SetActive(canvaActivity);
    }

    public void ConfirmButtonInteractable(bool b)
    {
        confirmSell.interactable = b;
    }
}
