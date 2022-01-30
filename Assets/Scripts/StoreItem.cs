using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;


public class StoreItem : MonoBehaviour
{
    [SerializeField] private Text _name;
    [SerializeField] private Text _price;

    private CatalogItem _item;
    private CatalogManager _manager;

    public void InitializeItem(CatalogManager manager, CatalogItem item)
    {
        _item = item;
        _manager = manager;
        
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
            if (_manager.GetCurrentGold() < _item.VirtualCurrencyPrices["GD"])
            {
                Debug.Log($"You don't have enough gold - you have {_manager.GetCurrentGold()}, but price is {_item.VirtualCurrencyPrices["GD"]}");
            }
            else
            {
                Debug.Log($"You purchased {_item.DisplayName} for {_item.VirtualCurrencyPrices["GD"]}, your gold is {_manager.GetCurrentGold()}");
            }
        }
        else
        {
            Debug.Log("Item is priceless!");
        }
    }
}
