using Leopotam.EcsLite;
using UnityEngine;

public sealed class s_ProgressIncome : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<c_BusinessState> _businessStatePool = default;
    private EcsPool<c_BusinessData> _businessDataPool = default;
    private EcsPool<t_IsIncomeReady> _incomeReadyTagPool = default;

    private EcsFilter _filter = default;

    private EcsWorld _world = default;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _filter = _world
            .Filter<c_BusinessState>()
            .Inc<c_BusinessData>()
            .Inc<t_IsPurchased>()
            .End();

        _businessStatePool = _world.GetPool<c_BusinessState>();
        _businessDataPool = _world.GetPool<c_BusinessData>();
        _incomeReadyTagPool = _world.GetPool<t_IsIncomeReady>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entity in _filter)
        {
            ref var c_businessState = ref _businessStatePool.Get(entity);
            ref var c_businesData = ref _businessDataPool.Get(entity);

            c_businessState.IncomeDelayElapsed -= Time.deltaTime;

            if (c_businessState.IncomeDelayElapsed <= 0f)
            {
                if (!_incomeReadyTagPool.Has(entity))  
                    _incomeReadyTagPool.Add(entity);

                c_businessState.IncomeDelayElapsed = c_businesData.Config.BaseIncomeDelay;
            }
        }
    }
}