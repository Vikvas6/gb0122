using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class PhotonLogin : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _photonLoginButton;
    [SerializeField] private Button _photonLogoutButton;
    [SerializeField] private Text _infoText;
    
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        _photonLoginButton.onClick.AddListener(Connect);
        _photonLogoutButton.onClick.AddListener(Disconnect);
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            _infoText.text = "Photon: You are not connected";
            _infoText.color = Color.yellow;
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Photon Success");
        _infoText.text = "Photon: success login!";
        _infoText.color = Color.green;
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        base.OnErrorInfo(errorInfo);
        Debug.Log("Photon Failed");
        _infoText.text = "Photon: error login!\n" + errorInfo.Info;
        _infoText.color = Color.red;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Photon Disconnected");
        _infoText.text = "Photon: disconnected";
        _infoText.color = Color.green;
    }
}
