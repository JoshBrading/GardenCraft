using System;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using TMPro;
using System.Data;


/// <summary>
/// A simple sample showing how to use the Relay Allocation package. As the host, you can authenticate, request a relay allocation, get a join code and join the allocation.
/// </summary>
/// <remarks>
/// The sample is limited to calling the Relay Allocation Service and does not cover connecting the host game client to the relay using Unity Transport Protocol.
/// This will cause the allocation to be reclaimed about 10 seconds after creating it.
/// </remarks>
public class SimpleRelay : MonoBehaviour
{

    /// <summary>
    /// The textbox displaying the Player Id.
    /// </summary>
    public Text PlayerIdText;

    /// <summary>
    /// The dropdown displaying the region.
    /// </summary>
    public Dropdown RegionsDropdown;

    /// <summary>
    /// The textbox displaying the Allocation Id.
    /// </summary>
    public Text HostAllocationIdText;

    /// <summary>
    /// The textbox displaying the Join Code.
    /// </summary>
    public TMP_InputField joinCodeField;

    /// <summary>
    /// The textbox displaying the Allocation Id of the joined allocation.
    /// </summary>
    public Text PlayerAllocationIdText;

    public Transform hostSpawn;
    public Transform clientSpawn;

    public Canvas canvas;

    public Button joinButton;


    Guid hostAllocationId;
    Guid playerAllocationId;
    string allocationRegion = "";
    string joinCode = "n/a";
    //string playerId = "Not signed in";
    string autoSelectRegionName = "auto-select (QoS)";
    int regionAutoSelectIndex = 0;
    List<Region> regions = new List<Region>();
    List<string> regionOptions = new List<string>();


    async void Start()
    {
        await UnityServices.InitializeAsync();
        Cursor.lockState = CursorLockMode.Confined;
        UpdateUI();
    }



    void UpdateUI()
    {
        //.text = playerId;
        /*
        RegionsDropdown.interactable = regions.Count > 0;
        RegionsDropdown.options?.Clear();
        RegionsDropdown.AddOptions(new List<string> {autoSelectRegionName});  // index 0 is always auto-select (use QoS)
        RegionsDropdown.AddOptions(regionOptions);
        if (!String.IsNullOrEmpty(allocationRegion))
        {
            if (regionOptions.Count == 0)
            {
                RegionsDropdown.AddOptions(new List<String>(new[] { allocationRegion }));
            }
            RegionsDropdown.value = RegionsDropdown.options.FindIndex(option => option.text == allocationRegion);
        }
        HostAllocationIdText.text = hostAllocationId.ToString();
        */
        joinCodeField.text = joinCode;
        //PlayerAllocationIdText.text = playerAllocationId.ToString();
    }

    /// <summary>
    /// Event handler for when the Sign In button is clicked.
    /// </summary>
    public async void OnSignIn()
    {
        if (NetworkManager.Singleton.IsServer) return;
        if (NetworkManager.Singleton.IsClient) return;
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        joinCode = NetworkManager.Singleton.StartHost() ? joinCode : "N/A";
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //playerId = AuthenticationService.Instance.PlayerId;
        joinCodeField.enabled = false;

        if( NetworkManager.Singleton.IsServer )
        {
            Debug.Log("IsServer");
            Debug.Log(joinCode);
            NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = hostSpawn.position;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        }
        else
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = clientSpawn.position;
        }
        //canvas.gameObject.SetActive(false);
        //
        //Debug.Log($"Signed in. Player ID: {playerId}");
        UpdateUI();
    }

    public async void OnQuickSignIn()
    {
        if (NetworkManager.Singleton.IsServer) return;
        if (NetworkManager.Singleton.IsClient) return;
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        joinCode = NetworkManager.Singleton.StartHost() ? joinCode : "N/A";
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //playerId = AuthenticationService.Instance.PlayerId;
        joinCodeField.enabled = false;

        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("IsServer");
            Debug.Log(joinCode);
            NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = hostSpawn.position;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        }
        else
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = clientSpawn.position;
        }

        canvas.gameObject.SetActive(false);
        Cursor.visible = false;

        UpdateUI();
    }

    /// <summary>
    /// Event handler for when the Get Regions button is clicked.
    /// </summary>
    public async void OnRegion()
    {
        Debug.Log("Host - Getting regions.");
        var allRegions = await RelayService.Instance.ListRegionsAsync();
        regions.Clear();
        regionOptions.Clear();
        foreach (var region in allRegions)
        {
            Debug.Log(region.Id + ": " + region.Description);
            regionOptions.Add(region.Id);
            regions.Add(region);
        }
        UpdateUI();
    }

    /// <summary>
    /// Event handler for when the Allocate button is clicked.
    /// </summary>
    public async void OnAllocate()
    {
        Debug.Log("Host - Creating an allocation.");

        // Determine region to use (user-selected or auto-select/QoS)
        string region = GetRegionOrQosDefault();
        Debug.Log($"The chosen region is: {region ?? autoSelectRegionName}");

        // Important: Once the allocation is created, you have ten seconds to BIND
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4, region);
        hostAllocationId = allocation.AllocationId;
        allocationRegion = allocation.Region;

        Debug.Log($"Host Allocation ID: {hostAllocationId}, region: {allocationRegion}");

        UpdateUI();
    }

    string GetRegionOrQosDefault()
    {
        // Return null (indicating to auto-select the region/QoS) if regions list is empty OR auto-select/QoS is chosen
        if (!regions.Any() || RegionsDropdown.value == regionAutoSelectIndex)
        {
            return null;
        }
        // else use chosen region (offset -1 in dropdown due to first option being auto-select/QoS)
        return regions[RegionsDropdown.value - 1].Id;
    }

    /// <summary>
    /// Event handler for when the Get Join Code button is clicked.
    /// </summary>
    public async void OnJoinCode()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        joinCode = joinCodeField.text;
        Debug.Log($"Join code: {joinCode}");

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        NetworkManager.Singleton.StartClient();
        //playerId = AuthenticationService.Instance.PlayerId;
        joinCodeField.enabled = false;
        canvas.gameObject.SetActive(false);
        NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = clientSpawn.position;

        Cursor.visible = false;
        //  try
        //  {
        //      joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocationId);
        //      Debug.Log("Host - Got join code: " + joinCode);
        //  }
        //  catch (RelayServiceException ex)
        //  {
        //      Debug.LogError(ex.Message + "\n" + ex.StackTrace);
        //  }

        UpdateUI();
    }

    /// <summary>
    /// Event handler for when the Join button is clicked.
    /// </summary>
    public async void OnJoin()
    {

        Debug.Log("Player - Joining host allocation using join code.");

        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            playerAllocationId = joinAllocation.AllocationId;
            Debug.Log("Player Allocation ID: " + playerAllocationId);
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message + "\n" + ex.StackTrace);
        }
        
        UpdateUI();
    }

    public async Task<string> StartHostWithRelay(int maxConnections = 5)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    private void OnDestroy()
    {
        // Since the NetworkManager can potentially be destroyed before this component, only
        // remove the subscriptions if that singleton still exists.
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"Connected: {clientId}");
        canvas.gameObject.SetActive(false);
        Cursor.visible = false;
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"Disconnected: {clientId}");
    }

    private void Update()
    {
        if( !NetworkManager.Singleton.IsServer)
        {
            if (joinCodeField.text.Length == 6)
            {
                joinButton.gameObject.SetActive(true);
            }
            else
            {
                joinButton.gameObject.SetActive(false);
            }

        }
    }

}
