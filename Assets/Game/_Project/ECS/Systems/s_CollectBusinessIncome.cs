using Leopotam.EcsLite;

public sealed class s_CollectBusinessIncome : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<c_BusinessState> _businessStatePool;
    private EcsPool<c_BusinessData> _businessDataPool;
    private EcsPool<r_CollectIncome> _collectIncomeRequestPool;
    private EcsPool<r_UpdateBalance> _updateBalanceRequestPool;

    private EcsFilter _ownedBusinessWithReadyIncomeFilter;

    private EcsWorld _world;

    private readonly GameSettingsConfig _settings;

    public s_CollectBusinessIncome(GameSettingsConfig settings) => _settings = settings;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _ownedBusinessWithReadyIncomeFilter = _world
            .Filter<c_BusinessState>()
            .Inc<c_BusinessData>()
            .Inc<t_IsPurchased>()
            .Inc<r_CollectIncome>()
            .End();

        _businessStatePool = _world.GetPool<c_BusinessState>();
        _businessDataPool = _world.GetPool<c_BusinessData>();
        _collectIncomeRequestPool = _world.GetPool<r_CollectIncome>();
        _updateBalanceRequestPool = _world.GetPool<r_UpdateBalance>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var businessEntity in _ownedBusinessWithReadyIncomeFilter)
        {
            ref var c_state = ref _businessStatePool.Get(businessEntity);
            ref var c_data = ref _businessDataPool.Get(businessEntity);

            if (c_data.PlayerEntityPacked.Unpack(_world, out var playerEntityUnpacked))
            {
                ref var r_updateBalance = ref _updateBalanceRequestPool.Add(playerEntityUnpacked);

                var config = _settings.BusinessConfigsById[c_data.Id];
                var totalBusinessIncome = c_state.Level.Value * config.BaseIncome;
                r_updateBalance.Amount = totalBusinessIncome;
            }

            _collectIncomeRequestPool.Del(businessEntity);
        }
    }
}