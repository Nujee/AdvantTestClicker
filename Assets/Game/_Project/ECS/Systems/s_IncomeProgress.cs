using Leopotam.EcsLite;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public sealed class IncomeProgressSystem :  IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _purchasedBusinessesFilter = default;

    private EcsPool<c_Business> _businessPool = default;
    private EcsPool<t_IsPurchased> _isPurchasedPool = default;
    private EcsPool<t_IsIncomeReady> _incomeReadyTagPool = default;
    private EcsPool<c_PlayerBalance> _playerBalancePool = default;

    private EcsWorld _world = default;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _purchasedBusinessesFilter = _world.Filter<c_Business>().Inc<t_IsPurchased>().End();

        _businessPool = _world.GetPool<c_Business>();
        _isPurchasedPool = _world.GetPool<t_IsPurchased>();
        _playerBalancePool = _world.GetPool<c_PlayerBalance>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entity in _purchasedBusinessesFilter)
        {
            ref var c_business = ref _businessPool.Get(entity);

            c_business.IncomeDelay -= Time.deltaTime;

            if (c_business.IncomeDelay > 0f)
                continue;

            if (!c_business.PlayerBalancePacked.Unpack(_world, out var playerBalanceUnpacked))
                continue;

            ref var c_playerBalance = ref _playerBalancePool.Get(playerBalanceUnpacked);

            var totFac = c_business.Upgrades
                .Where(upgrade => upgrade.isPurchased)
                .Sum(upgrade => upgrade.factor);

            var totalFactor = 0f;
            foreach (var upgrade in c_business.Upgrades)
            {
                if (upgrade.isPurchased)
                    totalFactor += upgrade.factor;
            }

            //var businessCurrentIncome = c_business.Level * c_business.BaseIncome * (1 + );
           // c_playerBalance += c_buisness.currentIncome;
            c_business.IncomeDelay = 0f;

        }
    }
}