using UnityEngine;
using Leopotam.EcsLite;

public sealed class EntryPoint : MonoBehaviour
{
    [field: SerializeField] public BalancePanelView BalancePanelView { get; private set; }
    [field: SerializeField] public RectTransform BusinessViewsParent { get; private set; }
    [field: SerializeField] public BusinessPanelView BusinessPanelPrefab { get; private set; }

    [field: SerializeField] public GameSettingsConfig Settings { get; private set; }
    [field: SerializeField] public SaveService SaveService { get; private set; }

    private EcsWorld _world;
    private IEcsSystems _systems;

    private void Start()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        //var (playerBalance, businessStates) = SaveService.LoadProgress(Settings.Businesses.Length);

        var playerEntity = _world.NewEntity();

        InitPlayer(playerEntity);

        InitBusinesses(playerEntity);

        InitSystems();
    }

    private void Update()
    {
        _systems?.Run();
    }

    private void OnDestroy()
    {
        _systems?.Destroy();
        _world?.Destroy();
    }

    private void OnApplicationQuit()
    {
        //saveService.SaveProgress();
    }

    private void InitPlayer(int playerEntity)
    {
        ref var c_playerBalance = ref _world.GetPool<c_Balance>().Add(playerEntity);
        BalancePanelView.Init(_world, playerEntity);
        c_playerBalance.Amount.Value = Settings.InitialBalance;
    }

    private void InitBusinesses(int playerEntity)
    {
        var businessViewPool = new GenericPool<BusinessPanelView>(BusinessPanelPrefab, 5, BusinessViewsParent);

        foreach (var businessConfigById in Settings.BusinessConfigsById)
        {
            var businessEntity = _world.NewEntity();

            ref var c_data = ref _world.GetPool<c_BusinessData>().Add(businessEntity);
            c_data.Id = businessConfigById.Key;
            c_data.PlayerEntityPacked = _world.PackEntity(playerEntity);

            ref var c_state = ref _world.GetPool<c_BusinessState>().Add(businessEntity);
            var businessView = businessViewPool.Get();
            businessView.Init(Settings, _world, businessEntity);
            SetUpBusinessState(ref c_state, ref c_data);

            ref var c_upgrades = ref _world.GetPool<c_BusinessUpgrades>().Add(businessEntity);
            foreach (var upgradeConfigById in businessConfigById.Value.Upgrades)
            {
                var upgradeId = upgradeConfigById.Key;
                var upgradeConfig = upgradeConfigById.Value;
                var upgradeInfo = c_upgrades.InfoById;

                upgradeInfo[upgradeId] = new UpgradeInfo
                (
                    purchaseData: default,
                    factor: upgradeConfig.PercentFactor * 0.01f
                );

                var upgradeView = businessView.UpgradeViews[upgradeId];
                upgradeView.Init(upgradeId, Settings, _world, businessEntity);

                var upgradePurchaseData = upgradeInfo[upgradeId].PurchaseData;
                upgradePurchaseData.Value = (upgradeConfig.Price, false);
            }

            if (c_state.Level.Value == 1)
                _world.GetPool<t_IsPurchased>().Add(businessEntity);
        }
    }

    private void SetUpBusinessState(ref c_BusinessState c_state, ref c_BusinessData c_data)
    {
        var config = Settings.BusinessConfigsById[c_data.Id];

        c_state.Level.Value = (c_data.Id == 1) ? 1 : 0;

        var levelUpPrice = (c_state.Level.Value + 1) * config.BasePrice;
        c_state.LevelUpPrice.Value = levelUpPrice;

        c_state.Income.Value = config.BaseIncome;

        c_state.IncomeDelayElapsed.Value = (0f, 0f);
    }

    private void InitSystems()
    {
        _systems
            .Add(new s_UpdateBalance())
            .Add(new s_UpdateBusinessIncome(Settings))
            .Add(new s_IncreaseBusinessLevel(Settings))
            .Add(new s_BuyBusinessUpgrade())
            .Add(new s_ElapseIncomeDelay(Settings))
            .Add(new s_CollectBusinessIncome(Settings))
            .Init();
    }
}