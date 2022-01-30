using UnityEngine;
using UnityEngine.UI;


public class PlayerItem : MonoBehaviour
{
    [SerializeField] private Text _name;

    private int _playerNumber;
    private string _playerName;

    public void InitializeItem(int number, string playerName)
    {
        _playerNumber = number;
        _playerName = playerName;
        
        _name.text = _playerName;
    }
}
