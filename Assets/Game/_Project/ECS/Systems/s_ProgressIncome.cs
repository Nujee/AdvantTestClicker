using Leopotam.EcsLite;
using UnityEngine;

public sealed class s_ProgressIncome : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _filter = default;

    private EcsPool<c_IncomeDelay> _incomeDelayPool = default;
    private EcsPool<t_IsIncomeReady> _incomeReadyTagPool = default;

    private EcsWorld _world = default;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _filter = _world.Filter<c_IncomeDelay>()
            .Inc<t_IsPurchased>()
            .End();

        _incomeDelayPool = _world.GetPool<c_IncomeDelay>();
        _incomeReadyTagPool = _world.GetPool<t_IsIncomeReady>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entity in _filter)
        {
            ref var c_incomeDelay = ref _incomeDelayPool.Get(entity);

            c_incomeDelay.CurrentValue -= Time.deltaTime;

            if (c_incomeDelay.CurrentValue <= 0f)
            {
                if (!_incomeReadyTagPool.Has(entity))  
                    _incomeReadyTagPool.Add(entity);

                c_incomeDelay.CurrentValue = c_incomeDelay.BaseValue;
            }
        }
    }
}