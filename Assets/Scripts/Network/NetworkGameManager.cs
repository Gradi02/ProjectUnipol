using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager instance;
    public List<PlayerInfo> clients = new List<PlayerInfo> ();
    [SerializeField] private PlayerUICard[] multiCards;
    [HideInInspector] public string username;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientJoin;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientLeave;
        NetworkManager.Singleton.OnServerStarted += OnHostStart;
        NetworkManager.Singleton.OnServerStopped += OnHostLeave;
    }

    private void OnApplicationQuit()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.Shutdown();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientJoin;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientLeave;
        NetworkManager.Singleton.OnServerStarted -= OnHostStart;
        NetworkManager.Singleton.OnServerStopped -= OnHostLeave;
    }



    private void OnHostStart()
    {

    }

    private void OnHostLeave(bool b)
    {

    }

    private void OnClientJoin(ulong clientId)
    {
        if(IsHost)
        {
            AddPlayerToListServerRpc(clientId);
        }
    }

    private void OnClientLeave(ulong clientId)
    {

    }




    [ClientRpc]
    private void UpdateCardClientRpc(ulong i)
    {
        multiCards[i].multiUsername.text = i.ToString();
    }

    [ServerRpc]
    private void AddPlayerToListServerRpc(ulong id)
    {
        AddPlayerToListClientRpc(id);

        for (int i = 0; i < clients.Count; i++)
        {
            UpdateCardClientRpc(clients[i].clientId);
        }
    }

    [ClientRpc]
    private void AddPlayerToListClientRpc(ulong id)
    {
        PlayerInfo p = new PlayerInfo();
        p.clientId = id;
        clients.Add(p);
    }


    [ServerRpc(RequireOwnership = false)]
    public void SwitchPlayerReadyServerRpc(ulong id)
    {
        SwitchPlayerReadyClientRpc(id);
    }

    [ClientRpc]
    private void SwitchPlayerReadyClientRpc(ulong id)
    {
        for(int i = 0; i < clients.Count; i++)
        {
            if (clients[i].clientId == id)
            {
                clients[i].isReady = !clients[i].isReady;
                multiCards[i].SwitchMultiReady(clients[i].isReady);
                break;
            }
        }
    }
}

[System.Serializable]
public class PlayerInfo
{
    public bool isReady = false;
    public ulong clientId;
    public string username;
}
