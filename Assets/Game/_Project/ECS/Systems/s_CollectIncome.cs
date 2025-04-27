//using Leopotam.EcsLite;

//public sealed class s_CollectIncome : IEcsInitSystem, IEcsRunSystem
//{
//    private EcsPool<c_BusinessState> _businessStatePool = default;
//    private EcsPool<t_IsIncomeReady> _incomeReadyTagPool = default;
//    private EcsPool<c_Balance> _balancePool = default;

//    private EcsFilter _filter = default;

//    private EcsWorld _world;

//    private readonly GameSettingsConfig _settings;

//    public s_CollectIncome(GameSettingsConfig settings) => _settings = settings;

//    public void Init(IEcsSystems systems)
//    {
//        _world = systems.GetWorld();

//        _filter = _world.Filter<t_IsIncomeReady>().End();

//        _businessStatePool = _world.GetPool<c_BusinessState>();
//        _incomeReadyTagPool = _world.GetPool<t_IsIncomeReady>();
//        _balancePool = _world.GetPool<c_Balance>();
//    }

//    public void Run(IEcsSystems systems)
//    {
//        foreach (var entity in _filter)
//        {
//            ref var c_businessState = ref _businessStatePool.Get(entity);

//            if (!c_businessState.PlayerEntityPacked.Unpack(_world, out var playerEntityUnpacked))
//                continue;

//            ref var c_playerBalance = ref _balancePool.Get(playerEntityUnpacked);
//            c_playerBalance.Value += GetTotalBusinessIncome(ref c_businessState);

//            _incomeReadyTagPool.Del(entity);

//            float GetTotalBusinessIncome(ref c_BusinessState businessState)
//            {
//                foreach (var upgradeState in businessState.UpgradeStates)
//                {
//                    if (upgradeState.IsPurchased)
//                    {

//                    }
//                }

//                foreach(var upgradePurchasedState in businessState.UpgradePurchasedStates)
//                {
//                    if (upgradePurchasedState)
//                    {
//                        _settings.Businesses[businessState.Id].Upgrades
//                    }
//                }

//                var totalMultiplier = 0f;
//                foreach (var upgrade in  upgrades.List)
//                    if (upgrade.IsPurchased)
//                        totalMultiplier += upgrade.Multiplier;

//                var baseIncome = _settings.Businesses[businessState.Id].BaseIncome;
//                var totalBusinessIncome = businessState.Level * baseIncome * (1 + totalMultiplier);

//                return totalBusinessIncome;
//            }
//        }
//    }
//}