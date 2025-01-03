using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class awaitingSellState : State
{
    [SerializeField] private GameObject plane;
    [SerializeField] private LayerMask mask;
    private bool isRayActive = false;
    private List<BoardField> movedFields = new List<BoardField>();

    private int selectedValue = 0, requiredValue = 0;
    private List<BoardField> selectedFields = new List<BoardField>();
    private float nextClick = 0;

    public override void Execute()
    {
        selectedValue = 0;
        requiredValue = currentField.GetComponent<IOwnableProperty>().GetCurrentVisitPrice();

        StartCoroutine(IEChooseSellAnimation());
    }


    private IEnumerator IEChooseSellAnimation()
    {
        List<BoardField> fieldsToMove = currentPlayer.ownedProperties;       

        plane.SetActive(true);
        LeanTween.alpha(plane, 0.9f, 0.5f).setEase(LeanTweenType.easeInOutQuad);

        for (int i = 0; i < fieldsToMove.Count; i++)
        {
            if(fieldsToMove[i] != null)
            {
                movedFields.Add(fieldsToMove[i]);

                LeanTween.moveLocalY(fieldsToMove[i].gameObject, 0.125f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            }
        }

        string s = $"Otrzymasz\n${selectedValue}\nza wybrane pola!";
        gameManager.canvaManager.SetCanvaActivity(true, false, true, false, false, true, s);
        isRayActive = true;

        yield return null;
    }


    private void Update()
    {
        if (!isRayActive) return;
        gameManager.canvaManager.ConfirmButtonInteractable((selectedValue + currentPlayer.money) >= requiredValue);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Time.time > nextClick)
            {
                nextClick = Time.time + 0.15f;
            }
            else
            {
                return;
            }

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                BoardField fl;
                hit.collider.TryGetComponent<BoardField>(out fl);

                if (fl != null)
                {
                    IOwnableProperty own;
                    fl.TryGetComponent<IOwnableProperty>(out own);

                    if (fl != null && own != null)
                    {
                        if (selectedFields.Contains(fl))
                        {
                            selectedFields.Remove(fl);
                            selectedValue -= fl.property.currentValue;
                            fl.selectedBox.SetActive(false);
                        }
                        else
                        {
                            selectedFields.Add(fl);
                            selectedValue += fl.property.currentValue;
                            fl.selectedBox.SetActive(true);
                        }

                        string s = $"Otrzymasz\n${selectedValue}\nza wybrane pola!";
                        gameManager.canvaManager.SetCanvaActivity(true, false, true, false, false, true, s);
                    }
                }
            }
        }
    }


    private IEnumerator EndSellState(bool surrender = false)
    {
        LeanTween.alpha(plane, 0f, 0.5f).setEase(LeanTweenType.easeInOutQuad);

        for (int i = 0; i < movedFields.Count; i++)
        {
            if (movedFields[i] != null)
            {
                LeanTween.moveLocalY(movedFields[i].gameObject, 0f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            }
        }

        yield return new WaitForSeconds(0.5f);
        movedFields.Clear();
        isRayActive = false;
        plane.SetActive(false);
        gameManager.canvaManager.SetCanvaActivity(false, true, false, false, false, true, "");

        if (surrender)
        {
            if (currentField.GetComponent<IOwnableProperty>() != null)
            {
                currentField.property.owner.money += requiredValue;
            }
        }
        else
        {
            foreach (BoardField field in selectedFields)
            {
                field.GetComponent<IOwnableProperty>().ResetField();
                field.selectedBox.SetActive(false);
            }

            //usuwanie z player owned properties
            currentPlayer.ownedProperties.RemoveAll(x => selectedFields.Contains(x));

            foreach (BoardField field in currentPlayer.ownedProperties)
            {
                field.GetComponent<IOwnableProperty>().FormatString();
            }

            currentPlayer.money += selectedValue - requiredValue;

            if(currentField.GetComponent<IOwnableProperty>() != null)
            {
                currentField.property.owner.money += requiredValue;
            }
        }

        gameManager.isStateEnded = true;
    }



    public void OnSurrender()
    {
        if (gameManager.currentState == this)
        {
            gameManager.players[gameManager.currentPlayerIndex].Surrender();
            StartCoroutine(EndSellState(true));
        }
    }

    public void OnConfirm()
    {
        if (gameManager.currentState == this)
        {
            StartCoroutine(EndSellState());
        }
    }
}
