using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager instance;
    public List<PlayerInfo> clients = new List<PlayerInfo> ();
    [SerializeField] private PlayerUICard[] multiCards;
    [HideInInspector] public string username;
    [SerializeField] private Button backButton, startButton;
    [SerializeField] private UIManager uimanager;

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

    private void Update()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.ServerIsHost) return;

        if (!IsHost)
        {
            startButton.interactable = false;
            return;
        }

        bool allReady = true;
        foreach(var c in clients)
        {
            if(!c.isReady)
                allReady = false;
        }
        startButton.interactable = allReady && clients.Count > 1;
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
        if(NetworkManager.Singleton.LocalClientId == clientId)
        {
            AddPlayerToListServerRpc(clientId, username);
        }
    }

    private void OnClientLeave(ulong clientId)
    {

    }

    public void StartNetworkGame()
    {
        if (IsServer)
        {
            Debug.Log("Start: " + NetworkManager.Singleton.ConnectedClientsList.Count);
            uimanager.SwitchCanvaServerRpc("GameOverlay");
            GameManager.instance.StartGame(multiCards);
        }
    }



    

    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerToListServerRpc(ulong id, string un)
    {
        PlayerInfo p = new PlayerInfo();
        p.clientId = id;
        p.username = un;
        clients.Add(p);

        for (int i = 0; i < clients.Count; i++)
        {
            AddPlayerToListClientRpc(clients[i].clientId, un);
            UpdateCardClientRpc(i, clients[i].username);
        }
    }

    [ClientRpc]
    private void AddPlayerToListClientRpc(ulong id, string un)
    {
        foreach(var c in clients)
        {
            if (c.clientId == id)
                return;
        }

        PlayerInfo p = new PlayerInfo();
        p.clientId = id;
        p.username = un;
        clients.Add(p);
    }

    [ClientRpc]
    private void UpdateCardClientRpc(int i, string un)
    {
        multiCards[i].multiUsername.text = un;
        multiCards[i].usernameText = un;
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
