using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UpgradeView : MonoBehaviour
{
    private ReactiveProperty<(float price, bool isPurchased)> _purchaseData;
    private ReactiveProperty<float> _balance;
    private ReactiveProperty<int> _level;

    [field: SerializeField] public Button BuyButton { get; private set; }
    [field: SerializeField] public TMP_Text TitleText { get; private set; }
    [field: SerializeField] public TMP_Text FactorText { get; private set; }
    [field: SerializeField] public TMP_Text PurchaseDataText { get; private set; }

    public void Init(int upgradeId, GameSettingsConfig settings, 
        EcsWorld world, int businessEntity, int playerEntity)
    {
        ref var c_businessData = ref world.GetPool<c_BusinessData>().Get(businessEntity);
        ref var c_businessState = ref world.GetPool<c_BusinessState>().Get(businessEntity);
        ref var c_businessUpgrades = ref world.GetPool<c_BusinessUpgrades>().Get(businessEntity);
        ref var c_balance = ref world.GetPool<c_Balance>().Get(playerEntity);

        var businessConfig = settings.BusinessConfigsById[c_businessData.Id];

        var upgradeTitle = businessConfig.Titles.UpgradeTitlesById[upgradeId];
        TitleText.text = upgradeTitle;

        var percentFactor = businessConfig.Upgrades[upgradeId].PercentFactor;
        FactorText.text = $"Доход: +{percentFactor}%";

        var upgradeInfo = c_businessUpgrades.InfoById[upgradeId];
        _purchaseData = upgradeInfo.PurchaseData;
        _purchaseData.OnValueSet += UpdateUpgradePurchaseDataText;
        _purchaseData.OnValueSet += UpdateButtonState;

        _balance = c_balance.Amount;
        _balance.OnValueSet += UpdateButtonState;

        _level = c_businessState.Level;
        _level.OnValueSet += UpdateButtonState;

        BuyButton.onClick.AddListener(() =>
        {
            ref var r_upgradeClicked = ref world.GetPool<r_UpgradeClicked>().Add(businessEntity);
            r_upgradeClicked.UpgradeId = upgradeId;
        });
    }

    private void OnDestroy()
    {
        if (_purchaseData != null)
        {
            _purchaseData.OnValueSet -= UpdateUpgradePurchaseDataText;
            _purchaseData.OnValueSet -= UpdateButtonState;
        }

        if (_balance != null)
            _balance.OnValueSet -= UpdateButtonState;

        if (_level != null)
            _level.OnValueSet -= UpdateButtonState;
    }

    private void UpdateUpgradePurchaseDataText((float price, bool isPurchased) purchaseData)
    {
        PurchaseDataText.text = purchaseData.isPurchased
            ? "Куплено"
            : $"Цена: {purchaseData.price:0.##}$";
    }

    private void UpdateButtonState()
    {
        var isBusinessPurchased = _level.Value > 0;
        var isUpgradeNotPurchased = !_purchaseData.Value.isPurchased;
        var isBalanceEnough = _balance.Value >= _purchaseData.Value.price;

        bool isInteractable = isBusinessPurchased &&
                             isUpgradeNotPurchased &&
                             isBalanceEnough;

        BuyButton.interactable = isInteractable;
    }

    private void UpdateButtonState(float _) => UpdateButtonState();
    private void UpdateButtonState((float, bool) _) => UpdateButtonState();
    private void UpdateButtonState(int _) => UpdateButtonState();
}