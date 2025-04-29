using Leopotam.EcsLite;

public sealed class s_UpdateBusinessIncome : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<c_BusinessState> _businessStatePool = default;
    private EcsPool<c_BusinessData> _businessDataPool = default;
    private EcsPool<r_UpdateBusinessIncome> _updateBusinessIncomeRequestPool = default;

    private EcsFilter _ownedBusinessWithIncomeToUpdateFilter = default;

    private EcsWorld _world = default;

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
        _updateBusinessIncomeRequestPool = _world.GetPool<r_UpdateBusinessIncome>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var businessEntity in _ownedBusinessWithIncomeToUpdateFilter)
        {
            ref var c_state = ref _businessStatePool.Get(businessEntity);
            ref var c_data = ref _businessDataPool.Get(businessEntity);

            var config = _settings.BusinessConfigsById[c_data.Id];
            c_state.Income.Value = c_state.Level.Value * config.BaseIncome; // * (1 + u1multip + u2multip)

            _updateBusinessIncomeRequestPool.Del(businessEntity);
        }
    }
}