using Leopotam.EcsLite;

public sealed class s_BuyBusinessUpgrade : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<c_BusinessData> _businessDataPool;
    private EcsPool<c_BusinessUpgrades> _businessUpgradesPool;
    private EcsPool<r_UpgradeClicked> _upgradeClickedRequestPool;
    private EcsPool<c_Balance> _balancePool;

    private EcsFilter _ownedBusinessToUpgradeFilter;

    private EcsWorld _world;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        _ownedBusinessToUpgradeFilter = _world
            .Filter<c_BusinessData>()
            .Inc<t_IsPurchased>()
            .Inc<r_UpgradeClicked>()
            .End();

        _businessDataPool = _world.GetPool<c_BusinessData>();
        _businessUpgradesPool = _world.GetPool<c_BusinessUpgrades>();
        _upgradeClickedRequestPool = _world.GetPool<r_UpgradeClicked>();
        _balancePool = _world.GetPool<c_Balance>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var businessEntity in _ownedBusinessToUpgradeFilter)
        {
            ref var c_data = ref _businessDataPool.Get(businessEntity);

            if (c_data.PlayerEntityPacked.Unpack(_world, out var playerEntityUnpacked))
            {
                ref var c_playerBalance = ref _balancePool.Get(playerEntityUnpacked);
                ref var c_upgrades = ref _businessUpgradesPool.Get(businessEntity);
                ref var r_upgradeClicked = ref _upgradeClickedRequestPool.Get(businessEntity);

                var upgradePurchaseData = c_upgrades.InfoById[r_upgradeClicked.UpgradeId].PurchaseData;
                var isUpgradePurchased = upgradePurchaseData.Value.isPurchased;
                var upgradePrice = upgradePurchaseData.Value.price;

                // Check if [upgrade is not purchased] and [balance is enough to buy it]
                if (!isUpgradePurchased && c_playerBalance.Amount.Value >= upgradePrice)
                {
                    // Update player balance
                    ref var r_updateBalance = ref _world.GetPool<r_UpdateBalance>().AddOrGet(playerEntityUnpacked);
                    r_updateBalance.ByAmount -= upgradePrice;

                    // Update upgrade purchase data (true is for isPurchased)
                    upgradePurchaseData.Value = (upgradePrice, true);

                    // Update business income
                    _world.GetPool<r_UpdateBusinessIncome>().Add(businessEntity);
                }
            }

            _world.GetPool<r_UpgradeClicked>().Del(businessEntity);
        }
    }
}