using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class awaitingUpgrDecisionState : State
{
    private int upgradeSelectionIndex = -1;
    public override void Execute()
    {
        BuyHandle();
    }


    private void BuyHandle()
    {
        string s2 = $"Do You Want To Upgrade {currentField.property.fieldname}?";
        gameManager.upgradeText.text = s2;

        upgradeSelectionIndex = -1;
        foreach (Button b in gameManager.upgrades)
        {
            b.GetComponent<Image>().color = gameManager.defaultColor;
            b.interactable = true;
        }

        gameManager.upgradesText[0].text = "Owned!";
        int off = currentField.property.level - 1;
        int idx = 1 + off;
        for (int i = 0 + off; i < 3; i++)
        {
            int prc = 0;
            for (int j = off; j < idx; j++)
            {
                prc += currentField.property.upgradePrices[j];
            }
            idx++;
            gameManager.upgradesText[i + 1].text = "Buy For: $" + prc;
            gameManager.upgrades[i + 1].interactable = currentPlayer.money >= prc;
        }

        for (int i = 0; i < 4; i++)
        {
            if (currentField.property.level > i)
            {
                gameManager.upgrades[i].interactable = false;
                gameManager.upgradesText[i].text = "Owned!";
            }
        }

        if (currentField.property.level < 3)
        {
            gameManager.upgrades[3].interactable = false;
        }

        gameManager.upgradeCanva.SetActive(true);
    }



    public void ConfirmUpgradeSelection()
    {
        if (gameManager.currentState == this)
        {
            if (upgradeSelectionIndex == -1)
            {
                gameManager.isStateEnded = true;
                return;
            }
            else
            {
                IOwnableProperty pr = currentField.GetComponent<IOwnableProperty>();
                if (pr != null)
                {
                    pr.OnUpgrade(upgradeSelectionIndex);
                }
                else
                {
                    Debug.LogWarning($"{currentField} nie implementuje IOwnableProperty!");
                }
            }

            gameManager.isStateEnded = true;
        }
    }

    public void SwitchUpgradeSelection(int idx)
    {
        if (gameManager.currentState == this)
        {
            if (upgradeSelectionIndex == idx)
            {
                upgradeSelectionIndex = -1;
                gameManager.upgrades[idx].GetComponent<Image>().color = gameManager.defaultColor;
            }
            else
            {
                if (upgradeSelectionIndex != -1)
                    gameManager.upgrades[upgradeSelectionIndex].GetComponent<Image>().color = gameManager.defaultColor;

                upgradeSelectionIndex = idx;
                gameManager.upgrades[idx].GetComponent<Image>().color = gameManager.selectedColor;
            }
        }
    }
}
