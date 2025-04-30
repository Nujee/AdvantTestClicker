using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UpgradeView : MonoBehaviour
{
    private ReactiveProperty<(float price, bool isPurchased)> _purchaseData;

    [field: SerializeField] public Button BuyButton { get; private set; }
    [field: SerializeField] public TMP_Text TitleText { get; private set; }
    [field: SerializeField] public TMP_Text FactorText { get; private set; }
    [field: SerializeField] public TMP_Text PurchasedStatusText { get; private set; }

    public void Init(int upgradeId, GameSettingsConfig settings, EcsWorld world, int businessEntity)
    {
        ref var c_businessData = ref world.GetPool<c_BusinessData>().Get(businessEntity);
        ref var c_upgrades = ref world.GetPool<c_BusinessUpgrades>().Get(businessEntity);

        var businessConfig = settings.BusinessConfigsById[c_businessData.Id];
        TitleText.text = businessConfig.Titles.UpgradeTitlesById[upgradeId];
        FactorText.text = $"Доход: +{businessConfig.Upgrades[upgradeId].PercentFactor}%";

        var upgradeInfo = c_upgrades.InfoById[upgradeId];
        _purchaseData = upgradeInfo.PurchaseData;
        _purchaseData.OnValueSet += RenewUpgradeVisualData;

        BuyButton.onClick.AddListener(() =>
        {
            ref var r_upgradeClicked = ref world.GetPool<r_UpgradeClicked>().Add(businessEntity);
            r_upgradeClicked.UpgradeId = upgradeId;
        });
    }

    private void OnDestroy()
    {
        if (_purchaseData != null)
            _purchaseData.OnValueSet -= RenewUpgradeVisualData;
    }

    private void RenewUpgradeVisualData((float price, bool isPurchased) purchaseInfo)
    {
        BuyButton.interactable = !purchaseInfo.isPurchased;

        PurchasedStatusText.text = purchaseInfo.isPurchased
            ? "Куплено"
            : $"Цена: {purchaseInfo.price:0.##}$";
    }
}