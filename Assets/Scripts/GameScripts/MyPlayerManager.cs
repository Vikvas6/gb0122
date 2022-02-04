using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class MyPlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameObject LocalPlayerInstance;
    public Dictionary<string, int> HpPots = new Dictionary<string, int>();
    
    [SerializeField] private GameObject _beams;
    [SerializeField] private GameObject _playerUiPrefab;
    
    public bool IsFiring;
    public float Health = 1f;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
    }

    private void Start()
    {
        MyCameraWork _cameraWork = gameObject.GetComponent<MyCameraWork>();

        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("Missing CameraWork Component on playerPrefab.", this);
        }
        
        if (_playerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(_playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
            if (Health <= 0f)
            {
                MyGameManager.Instance.LeaveRoom();
            }
        }
        if (_beams != null && IsFiring != _beams.activeInHierarchy)
        {
            _beams.SetActive(IsFiring);
        }
    }
    
    private void ProcessInputs()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!IsFiring)
            {
                IsFiring = true;
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (IsFiring)
            {
                IsFiring = false;
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            ConsumeHpPot("20");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(IsFiring);
            stream.SendNext(Health);
        }
        else
        {
            IsFiring = (bool)stream.ReceiveNext();
            Health = (float)stream.ReceiveNext();
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (!other.name.Contains("Beam"))
        {
            return;
        }

        Health -= 0.1f;
    }
    
    public void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (!other.name.Contains("Beam"))
        {
            return;
        }

        Health -= 0.1f*Time.deltaTime;
    }

    public void AddConsumable(string type, string power, int count)
    {
        Debug.Log($"Consumable {type} {power} {count} added.");
        if (type == "hp")
        {
            if (HpPots.ContainsKey(power))
            {
                HpPots[power] += count;
            }
            else
            {
                HpPots.Add(power, count);
            }
            Debug.Log(HpPots[power]);
        }
    }

    public void ConsumeHpPot(string power)
    {
        Debug.Log($"Consuming hp pot with power {power}");
        if (HpPots.ContainsKey(power))
        {
            Debug.Log($"Healing for {(float) int.Parse(power) / 100}");
            
            HpPots[power] -= 1;
            Health += (float) int.Parse(power) / 100;
            
            if (HpPots[power] <= 0)
            {
                HpPots.Remove(power);
            }
        }
    }
}
