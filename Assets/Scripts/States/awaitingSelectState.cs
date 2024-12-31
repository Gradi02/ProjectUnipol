using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class awaitingSelectState : State
{
    [SerializeField] private GameObject plane;
    [SerializeField] private LayerMask mask;
    private bool isRayActive = false;
    private List<BoardField> movedFields = new List<BoardField>();
    private float nextClick = 0;

    public override void Execute()
    {
        StartCoroutine(IESelectField());
    }


    private IEnumerator IESelectField()
    {
        List<BoardField> fieldsToMove = currentPlayer.ownedProperties;

        plane.SetActive(true);
        LeanTween.alpha(plane, 0.9f, 0.5f).setEase(LeanTweenType.easeInOutQuad);

        for (int i = 0; i < fieldsToMove.Count; i++)
        {
            if (fieldsToMove[i] != null && fieldsToMove[i] is not MPKField)
            {
                movedFields.Add(fieldsToMove[i]);
                LeanTween.moveLocalY(fieldsToMove[i].gameObject, 0.125f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            }
        }

        isRayActive = true;

        yield return null;
    }


    private void Update()
    {
        if (!isRayActive) return;

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
                NormalField fl;
                hit.collider.TryGetComponent<NormalField>(out fl);

                if (fl != null)
                {
                    fl.StartJuwenalia();
                    StartCoroutine(EndSelectState());
                }
            }
        }
    }


    private IEnumerator EndSelectState()
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
        

        gameManager.isStateEnded = true;
    }
}
