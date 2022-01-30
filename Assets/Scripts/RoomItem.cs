using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;


public class RoomItem : MonoBehaviour
{
    [SerializeField] private Text _name;

    private string _roomName;
    private byte _currentPlayers;
    private int _maxPlayers;

    public void InitializeItem(string roomName, byte currentPlayers, int maxPlayers)
    {
        _roomName = roomName;
        _currentPlayers = currentPlayers;
        _maxPlayers = maxPlayers;
        
        _name.text = _roomName;
    }
    
    public void JoinRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(_roomName);
    }
}
