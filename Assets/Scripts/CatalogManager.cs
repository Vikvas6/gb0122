using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;


public class CatalogManager : MonoBehaviour
{
    [SerializeField] private StoreItem _item;
    
    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();
    private int _gold;

    private void Start()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnFailure);
        UpdateUserInventory();
    }

    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"GetCatalogItems: Something went wrong: {errorMessage}");
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        HandleCatalog(result.Catalog);
        Debug.Log($"Catalog was loaded successfully!");
    }

    private void HandleCatalog(List<CatalogItem> catalog)
    {
        foreach (var item in catalog)
        {
            _catalog.Add(item.ItemId, item);
            Debug.Log($"Catalog item {item.ItemId} was added successfully!");
            var storeItem = Instantiate(_item, _item.transform.parent);
            storeItem.InitializeItem(this, item);
            storeItem.gameObject.SetActive(true);
        }
    }
    
    private void UpdateUserInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetInventorySuccess, OnGetInventoryFailure);
    }

    private void OnGetInventorySuccess(GetUserInventoryResult result)
    {
        _gold = result.VirtualCurrency["GD"];
    }
    
    private void OnGetInventoryFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"GetUserInventory: Something went wrong: {errorMessage}");
    }

    public int GetCurrentGold()
    {
        return _gold;
    }
}
