using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using UnityEngine;
using UnityEngine.UI;


public class StoreItem : MonoBehaviour
{
    [SerializeField] private Text _name;
    [SerializeField] private Text _price;

    private CatalogItem _item;

    public void InitializeItem(CatalogItem item)
    {
        _item = item;
        
        _name.text = _item.DisplayName;
        if (_item.VirtualCurrencyPrices.ContainsKey("GD"))
        {
            _price.text = _item.VirtualCurrencyPrices["GD"].ToString();
        }
    }
    
    public void BuyItem()
    {
        if (_item.VirtualCurrencyPrices.ContainsKey("GD"))
        {
            if (CatalogManager.Instance.GetCurrentGold() < _item.VirtualCurrencyPrices["GD"])
            {
                Debug.Log($"You don't have enough gold - you have {CatalogManager.Instance.GetCurrentGold()}, but price is {_item.VirtualCurrencyPrices["GD"]}");
            }
            else
            {
                Debug.Log($"You purchased {_item.DisplayName} for {_item.VirtualCurrencyPrices["GD"]}, your gold is {CatalogManager.Instance.GetCurrentGold()}");
                MakePurchase();
            }
        }
        else
        {
            Debug.Log("Item is priceless!");
        }
    }
    
    private void MakePurchase() {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest {
            ItemId = _item.ItemId,
            Price = (int) _item.VirtualCurrencyPrices["GD"],
            VirtualCurrency = "GD"
        }, LogSuccess, LogFailure);
    }
    
    private void LogSuccess(PlayFabResultCommon result) {
        var requestName = result.Request.GetType().Name;
        Debug.Log(requestName + " successful");
    }

    private void LogFailure(PlayFabError error) {
        Debug.LogError(error.GenerateErrorReport());
    }
}
