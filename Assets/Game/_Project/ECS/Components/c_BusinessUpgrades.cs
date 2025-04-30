using Leopotam.EcsLite;
using System.Collections.Generic;

public struct c_BusinessUpgrades : IEcsAutoReset<c_BusinessUpgrades>
{
    public Dictionary<int, UpgradeInfo> InfoById;

    public void AutoReset(ref c_BusinessUpgrades c)
    {
        c.InfoById = new Dictionary<int, UpgradeInfo>();
    }
}

public sealed class UpgradeInfo
{
    public ReactiveProperty<(float price, bool isPurchased)> PurchaseData = new();
    public float Factor;

    public UpgradeInfo((float, bool) purchaseData, float factor)
    {
        PurchaseData.Value = purchaseData;
        Factor = factor;
    }
}