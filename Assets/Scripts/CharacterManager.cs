using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;


public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject playerCharacterSlot1_Empty;
    [SerializeField] private GameObject playerCharacterSlot2_Empty;
    [SerializeField] private GameObject playerCharacterSlot1;
    [SerializeField] private GameObject playerCharacterSlot2;
    [SerializeField] private GameObject newCharacterCreatePanel;
    
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject charactersPanel;

    [SerializeField] private Text _slot1TextName;
    [SerializeField] private Text _slot1TextLevel;
    [SerializeField] private Text _slot1TextXP;
    [SerializeField] private Text _slot1TextGold;
    [SerializeField] private Text _slot1TextHP;
    [SerializeField] private Text _slot1TextDmg;
    [SerializeField] private Text _slot2TextName;
    [SerializeField] private Text _slot2TextLevel;
    [SerializeField] private Text _slot2TextXP;
    [SerializeField] private Text _slot2TextGold;
    [SerializeField] private Text _slot2TextHP;
    [SerializeField] private Text _slot2TextDmg;
    
    private List<CharacterResult> _characters = new List<CharacterResult>();
    private string _characterName;

    private readonly string CHARACTER_STORE_ID = "store-character";
    private readonly string VIRTUAL_CURRENCY_KEY = "GD";
    public static readonly string PLAYER_PREFS_CHARACTER_KEY = "character-id";

    private void Start()
    {
        GetCharacters();
    }

    private void GetCharacters()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
            res =>
            {
                Debug.Log($"Characters owned: + {res.Characters.Count}");
                if (_characters.Count > 0)
                {
                    _characters.Clear();
                }

                foreach (var characterResult in res.Characters)
                {
                    _characters.Add(characterResult);
                }

                ShowCharacterSlotButtons();
            }, Debug.LogError);
    }
    
    private void ShowCharacterSlotButtons()
    {
        playerCharacterSlot1_Empty.SetActive(true);
        playerCharacterSlot2_Empty.SetActive(true);
        playerCharacterSlot1.SetActive(false);
        playerCharacterSlot2.SetActive(false);

        if (_characters.Count > 0)
        {
            playerCharacterSlot1_Empty.SetActive(false);
            playerCharacterSlot1.SetActive(true);
            PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
            {
                CharacterId = _characters[0].CharacterId
            }, result =>
            {
                _slot1TextName.text = _characters[0].CharacterName;
                _slot1TextLevel.text = result.CharacterStatistics["Level"].ToString();
                _slot1TextXP.text = result.CharacterStatistics["XP"].ToString();
                _slot1TextGold.text = result.CharacterStatistics["Gold"].ToString();
                _slot1TextDmg.text = $"{result.CharacterStatistics["DmgLow"].ToString()}-{result.CharacterStatistics["DmgHi"].ToString()}";
                _slot1TextHP.text = result.CharacterStatistics["HP"].ToString();
            }, Debug.LogError);
            
            if (_characters.Count > 1)
            {
                playerCharacterSlot2_Empty.SetActive(false);
                playerCharacterSlot2.SetActive(true);
                PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
                {
                    CharacterId = _characters[1].CharacterId
                }, result =>
                {
                    _slot2TextName.text = _characters[1].CharacterName;
                    _slot2TextLevel.text = result.CharacterStatistics["Level"].ToString();
                    _slot2TextXP.text = result.CharacterStatistics["XP"].ToString();
                    _slot2TextGold.text = result.CharacterStatistics["Gold"].ToString();
                    _slot2TextDmg.text = $"{result.CharacterStatistics["DmgLow"].ToString()}-{result.CharacterStatistics["DmgHi"].ToString()}";
                    _slot2TextHP.text = result.CharacterStatistics["HP"].ToString();
                }, Debug.LogError);
            }
        }
    }
    
    public void OpenCreateNewCharacterPrompt()
    {
        newCharacterCreatePanel.SetActive(true);
    }

    public void CloseCreateNewCharacterPrompt()
    {
        newCharacterCreatePanel.SetActive(false);
        GetCharacters();
    }
    
    public void OnNameChanged(string changedName)
    {
        _characterName = changedName;
    }

    public void OnCreateCharacterButtonClicked()
    {
        if (string.IsNullOrEmpty(_characterName))
        {
            Debug.LogError("Character name is empty, character cannot be created");
            return;
        }
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest
        {
            StoreId = CHARACTER_STORE_ID
        }, result =>
        {
            HandleStoreResult(result.Store);
        }, Debug.LogError);
    }

    private void HandleStoreResult(List<PlayFab.ClientModels.StoreItem> items)
    {
        foreach (var item in items)
        {
            Debug.Log(item.ItemId);
            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
            {
                ItemId = item.ItemId,
                VirtualCurrency = VIRTUAL_CURRENCY_KEY,
                Price = (int)item.VirtualCurrencyPrices[VIRTUAL_CURRENCY_KEY]
            }, result =>
            {
                Debug.Log($"Item {result.Items[0].ItemId} was purchased.");
                CreateCharacterWithItemId(result.Items[0].ItemId);
            }, Debug.LogError);
        }
    }

    public void CreateCharacterWithItemId(string itemId)
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = _characterName,
            ItemId = itemId
        }, result =>
        {
            UpdateCharacterStatistics(result.CharacterId);
        }, Debug.LogError);
    }

    private void UpdateCharacterStatistics(string characterId)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
            {
                CharacterId = characterId,
                CharacterStatistics = new Dictionary<string, int>
                {
                    {"Level", 1},
                    {"XP", 0},
                    {"Gold", 100},
                    {"DmgLow", 2},
                    {"DmgHi", 4},
                    {"HP", 25}
                }
            }, result =>
            {
                Debug.Log($"Initial stats set, telling client to update character list");
                CloseCreateNewCharacterPrompt();
            },
            Debug.LogError);
    }

    public void CharacterSelected(int slot)
    {
        PlayerPrefs.SetString(PLAYER_PREFS_CHARACTER_KEY, _characters[slot].CharacterId);
        inventoryPanel.SetActive(true);
        lobbyPanel.SetActive(true);
        charactersPanel.SetActive(false);
    }
}