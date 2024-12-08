using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] canvas;


    private void Start()
    {
        SwitchCanva("StartCanva");
    }



    public void Singleplayer()
    {
        SwitchCanva("SelectPlayer");
    }

    public void StartGame()
    {
        SwitchCanva("GameOverlay");
    }

    public void GoBack()
    {
        SwitchCanva("StartCanva");
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
