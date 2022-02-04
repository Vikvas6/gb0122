using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using PlayFab.SharedModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyGameManager : MonoBehaviourPunCallbacks
{
    static public MyGameManager Instance;
    
    [SerializeField] private GameObject _playerPrefab;
    
    private void Start()
    {
        Instance = this;

        if (_playerPrefab == null) {
            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        } else {


            if (MyPlayerManager.LocalPlayerInstance==null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
            GetInventory();
        }
        
        
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
    
    public override void OnPlayerLeftRoom( Player other  )
    {
        Debug.Log( "OnPlayerLeftRoom() " + other.NickName ); // seen when other disconnects

        if ( PhotonNetwork.IsMasterClient )
        {
            Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // called before OnPlayerLeftRoom

            RecreateArenaForUsers(); 
        }
    }
    
    private void RecreateArenaForUsers()
    {
        if ( ! PhotonNetwork.IsMasterClient )
        {
            Debug.LogError( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
        }
        Debug.LogFormat( "PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount );
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PhotonNetwork.LoadLevel("Game");
        }
        else
        {
            Victory();
        }
    }

    private void Victory()
    {
        Debug.Log("You won!");
        PhotonNetwork.LeaveRoom();
    }
    
    private void GetInventory() {
        Debug.Log("Getting inventory");
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), LogSuccess, LogFailure);
    }
    
    private void LogSuccess(GetUserInventoryResult result) {
        Debug.Log("Getting inventory success");
        foreach (var itemInstance in result.Inventory)
        {
            if (itemInstance.ItemClass == "consumable")
            {
                Debug.Log("Consumable found");
                if (itemInstance.RemainingUses != null)
                {
                    MyPlayerManager.LocalPlayerInstance.GetComponent<MyPlayerManager>().AddConsumable(
                        "hp", "20", itemInstance.RemainingUses.Value
                    );
                }
            }
        }
        var requestName = result.Request.GetType().Name;
        Debug.Log(requestName + " successful");
    }

    private void LogFailure(PlayFabError error) {
        Debug.LogError(error.GenerateErrorReport());
    }
}
