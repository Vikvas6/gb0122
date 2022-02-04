using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _roomItem;
    [SerializeField] private InputField _roomName;
    [SerializeField] private GameObject _userItem;

    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _roomStartButton;
    
    private Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();
    private Dictionary<string, GameObject> _roomListEntries = new Dictionary<string, GameObject>();
    private Dictionary<int, GameObject> _playerListEntries;

    #region UnityMethods

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion

    #region CustomMethods

    private void ClearRoomListView()
    {
        foreach (GameObject entry in _roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        _roomListEntries.Clear();
    }
    
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (_cachedRoomList.ContainsKey(info.Name))
                {
                    _cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (_cachedRoomList.ContainsKey(info.Name))
            {
                _cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                _cachedRoomList.Add(info.Name, info);
            }
        }
    }
    
    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in _cachedRoomList.Values)
        {
            GameObject entry = Instantiate(_roomItem, _roomItem.transform.parent);
            entry.transform.localScale = Vector3.one;
            entry.SetActive(true);
            entry.GetComponent<RoomItem>().InitializeItem(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

            _roomListEntries.Add(info.Name, entry);
        }
    }
    
    private void SetRoomPanelActive()
    {
        _lobbyPanel.SetActive(false);
        _roomPanel.SetActive(true);
    }
    
    private void SetLobbyPanelActive()
    {
        _lobbyPanel.SetActive(true);
        _roomPanel.SetActive(false);
    }

    
    #endregion

    #region PhotonEvents

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate called");
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Photon Disconnected");
    }
    
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Photon Success");
    }
    
    public override void OnJoinedLobby()
    {
        _cachedRoomList.Clear();
        ClearRoomListView();
    }
    
    public override void OnLeftLobby()
    {
        _cachedRoomList.Clear();
        ClearRoomListView();
    }
    
    public override void OnJoinedRoom()
    {
        _cachedRoomList.Clear();
        SetRoomPanelActive();

        if (_playerListEntries == null)
        {
            _playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(_userItem, _userItem.transform.parent);
            entry.transform.localScale = Vector3.one;
            entry.SetActive(true);
            entry.GetComponent<PlayerItem>().InitializeItem(p.ActorNumber, p.UserId);
            _playerListEntries.Add(p.ActorNumber, entry);
        }
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions options = new RoomOptions {MaxPlayers = 8};
        PhotonNetwork.CreateRoom(roomName, options, null);
    }
    
    public override void OnLeftRoom()
    {
        SetLobbyPanelActive();

        foreach (GameObject entry in _playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        _playerListEntries.Clear();
        _playerListEntries = null;
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(_userItem, _userItem.transform.parent);
        entry.transform.localScale = Vector3.one;
        entry.SetActive(true);
        entry.GetComponent<PlayerItem>().InitializeItem(newPlayer.ActorNumber, newPlayer.UserId);

        _playerListEntries.Add(newPlayer.ActorNumber, entry);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(_playerListEntries[otherPlayer.ActorNumber].gameObject);
        _playerListEntries.Remove(otherPlayer.ActorNumber);
    }
    
    #endregion

    #region ButtonEvents
    
    public void OnCreateRoomButtonClicked()
    {
        string roomName = _roomName.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;
        RoomOptions options = new RoomOptions {MaxPlayers = 16, PlayerTtl = 10000 };
        PhotonNetwork.CreateRoom(roomName, options, null);
    }
    
    public void OnCreateHiddenRoomButtonClicked()
    {
        string roomName = _roomName.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;
        RoomOptions options = new RoomOptions {MaxPlayers = 16, PlayerTtl = 10000, IsVisible = false};
        PhotonNetwork.CreateRoom(roomName, options, null);
    }
    
    public void OnJoinByNameButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(_roomName.text);
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    
    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    public void OnPrepareGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        Debug.Log($"Room IsOpen = {PhotonNetwork.CurrentRoom.IsOpen}");
        _roomStartButton.SetActive(true);
    }
    
    public void OnStartGameButtonClicked()
    {
        Debug.Log("Start game!");
    }

    #endregion
}
