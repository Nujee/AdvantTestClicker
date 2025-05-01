using Leopotam.EcsLite;
using UnityEngine;

public sealed class s_ElapseIncomeDelay : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<c_BusinessState> _businessStatePool;
    private EcsPool<c_BusinessData> _businessDataPool;
    private EcsPool<r_CollectIncome> _collectIncomeRequestPool;

    private EcsFilter _ownedBusinessFilter;

    private EcsWorld _world;

    private readonly GameSettingsConfig _settings;

    public s_ElapseIncomeDelay(GameSettingsConfig settings) => _settings = settings;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _ownedBusinessFilter = _world
            .Filter<c_BusinessState>()
            .Inc<c_BusinessData>()
            .Inc<t_IsPurchased>()
            .End();

        _businessStatePool = _world.GetPool<c_BusinessState>();
        _businessDataPool = _world.GetPool<c_BusinessData>();
        _collectIncomeRequestPool = _world.GetPool<r_CollectIncome>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var businessEntity in _ownedBusinessFilter)
        {
            ref var c_state = ref _businessStatePool.Get(businessEntity);
            ref var c_data = ref _businessDataPool.Get(businessEntity);

            var config = _settings.BusinessConfigsById[c_data.Id];

            var raw = c_state.IncomeDelayElapsed.Value.raw + Time.deltaTime;
            var normalized = raw / config.BaseIncomeDelay;
            c_state.IncomeDelayElapsed.Value = (raw, normalized);

            if (c_state.IncomeDelayElapsed.Value.raw >= config.BaseIncomeDelay)
            {
                _collectIncomeRequestPool.Add(businessEntity);

                c_state.IncomeDelayElapsed.Value = (0f, 0f);
            }
        }
    }
}