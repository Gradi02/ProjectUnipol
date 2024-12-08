using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] canvas;
    [SerializeField] private PlayerUICard[] uicards;

    private void Start()
    {
        SwitchCanva("StartCanva");
    }



    public void Singleplayer()
    {
        SwitchCanva("SelectPlayer");
    }


    public void GoBack()
    {
        SwitchCanva("StartCanva");
    }

    public void StartGame()
    {
        SwitchCanva("GameOverlay");
        GameManager.instance.StartGame(uicards);
    }


    private void SwitchCanva(string canvaName)
    {
        foreach(GameObject go in canvas)
        {
            if(go.name == canvaName)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }
    }
}
