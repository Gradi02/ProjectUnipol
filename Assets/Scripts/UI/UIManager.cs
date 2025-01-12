using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class UIManager : NetworkBehaviour
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


    public void SwitchCanva(string canvaName)
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


    [ServerRpc(RequireOwnership = false)]
    public void SwitchCanvaServerRpc(string n)
    {
        Debug.Log("IsSpawned: " + GetComponent<NetworkObject>().IsSpawned);
        Debug.Log("IsOwner: " + GetComponent<NetworkObject>().IsOwner);
        SwitchCanvaClientRpc(n);
    }

    [ClientRpc]
    public void SwitchCanvaClientRpc(string n)
    {
        Debug.Log("canva");
        SwitchCanva(n);
    }
}
