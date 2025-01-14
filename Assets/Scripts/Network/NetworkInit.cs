using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class NetworkInit : NetworkBehaviour
{
    [SerializeField] private UIManager uimanager;

    [SerializeField] private GameObject inputCode;
    [SerializeField] private TextMeshProUGUI code;
    [SerializeField] private GameObject inputUsername;

    private string joincode;
    public static int MaxPlayer = 4;



    public async void Autentication()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (AuthenticationService.Instance.IsSignedIn) return;

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            uimanager.SwitchCanva("MultiplayerStartCanva");
            GameManager.instance.isMultiplayer = true;
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e);
        }
    }

    private async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayer);

            joincode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            code.text = joincode;

            var relayServerData = allocation.ToRelayServerData("udp");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            if (NetworkManager.Singleton.StartHost())
            {
                uimanager.SwitchCanva("MultiplayerSelectPlayer");
            }
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }


    private async void JoinRelay(string join_code)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(join_code);

            var relayServerData = joinAllocation.ToRelayServerData("udp");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);


            if (NetworkManager.Singleton.StartClient())
            {
                uimanager.SwitchCanva("MultiplayerSelectPlayer");
            }
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void HostGame()
    {
        CreateRelay();

        NetworkGameManager.instance.username = inputUsername.GetComponent<TMP_InputField>().text;
        inputCode.SetActive(false);
    }

    public void JoinGame()
    {
        joincode = inputCode.GetComponent<TMP_InputField>().text;
        if (joincode.Length != 6)
        {
            Debug.Log("zly kod");
            return;
        }

        JoinRelay(joincode);

        NetworkGameManager.instance.username = inputUsername.GetComponent<TMP_InputField>().text;
        inputCode.SetActive(false);
        code.gameObject.SetActive(false);
    }
}