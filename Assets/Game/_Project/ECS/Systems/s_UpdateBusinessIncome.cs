using Leopotam.EcsLite;

public sealed class s_UpdateBusinessIncome : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<c_BusinessState> _businessStatePool;
    private EcsPool<c_BusinessData> _businessDataPool;
    private EcsPool<c_BusinessUpgrades> _businessUpgradesPool;    
    private EcsPool<r_UpdateBusinessIncome> _updateBusinessIncomeRequestPool;

    private EcsFilter _ownedBusinessWithIncomeToUpdateFilter;

    private EcsWorld _world;

    private readonly GameSettingsConfig _settings;

    public s_UpdateBusinessIncome(GameSettingsConfig settings) => _settings = settings;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _ownedBusinessWithIncomeToUpdateFilter = _world
            .Filter<c_BusinessState>()
            .Inc<c_BusinessData>()
            .Inc<t_IsPurchased>()
            .Inc<r_UpdateBusinessIncome>()
            .End();

        _businessStatePool = _world.GetPool<c_BusinessState>();
        _businessDataPool = _world.GetPool<c_BusinessData>();
        _businessUpgradesPool = _world.GetPool<c_BusinessUpgrades>();
        _updateBusinessIncomeRequestPool = _world.GetPool<r_UpdateBusinessIncome>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var businessEntity in _ownedBusinessWithIncomeToUpdateFilter)
        {
            ref var c_state = ref _businessStatePool.Get(businessEntity);
            ref var c_data = ref _businessDataPool.Get(businessEntity);
            ref var c_upgrades = ref _businessUpgradesPool.Get(businessEntity);

            var config = _settings.BusinessConfigsById[c_data.Id];

            var totalUpgradesFactor = 0f;
            foreach (var upgradeInfo in c_upgrades.InfoById)
            {
                var isUpgradePurchased = upgradeInfo.Value.PurchaseData.Value.isPurchased;

                if (isUpgradePurchased)
                    totalUpgradesFactor += upgradeInfo.Value.Factor;
            }

            c_state.Income.Value = c_state.Level.Value * config.BaseIncome * (1 + totalUpgradesFactor);

            _updateBusinessIncomeRequestPool.Del(businessEntity);
        }
    }
}