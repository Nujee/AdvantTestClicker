using UnityEngine;
using Leopotam.EcsLite;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class EntryPoint : MonoBehaviour
{
    [field: SerializeField] public BalancePanelView BalancePanelView { get; private set; }
    [field: SerializeField] public RectTransform BusinessViewsParent { get; private set; }
    [field: SerializeField] public BusinessPanelView BusinessPanelPrefab { get; private set; }
    [field: SerializeField] public ScrollRect Scroll { get; private set; }

    [field: SerializeField] public GameSettingsConfig Settings { get; private set; }

    private EcsWorld _world;
    private IEcsSystems _systems;
    private SaveService _saveService;

    private void Start()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);
        _saveService = new SaveService(Settings, _world);

        SaveData saveData = _saveService.LoadProgress();

        var playerEntity = _world.NewEntity();

        // Player initialization is split to let BusinessView subscribe to reactive properties
        // before initial values are set, ensuring proper visual updates.
        InitPlayer();
        InitBusinesses();
        SetupPlayer();
        InitSystems();

        ResetScrollToTop();

        void InitPlayer()
        {
            ref var c_playerBalance = ref _world.GetPool<c_Balance>().Add(playerEntity);
        }

        void InitBusinesses()
        {
            var businesses = Settings.BusinessConfigsById.Map;
            var businessViewPool =
                new GenericPool<BusinessPanelView>(BusinessPanelPrefab, businesses.Count, BusinessViewsParent);

            foreach (var pair in businesses)
            {
                var businessView = businessViewPool.Get();

                var businessId = pair.Key;
                var businessConfig = pair.Value;

                var businessSaveData = saveData.BusinessesById[businessId];

                var businessEntity = _world.NewEntity();

                // State initialization is split to let BusinessView subscribe to reactive properties
                // before initial values are set, ensuring proper visual updates.
                InitData();
                InitState();
                InitView();
                InitUpgrades();
                SetupState();

                void InitData()
                {
                    ref var c_data = ref _world.GetPool<c_BusinessData>().Add(businessEntity);
                    c_data.Id = businessId;
                    c_data.PlayerEntityPacked = _world.PackEntity(playerEntity);
                }

                void InitState()
                {
                    ref var c_state = ref _world.GetPool<c_BusinessState>().Add(businessEntity);
                }

                void InitView()
                {
                    businessView.Init(Settings, _world, businessEntity, playerEntity);
                }

                void InitUpgrades()
                {
                    ref var c_data = ref _world.GetPool<c_BusinessData>().Get(businessEntity);
                    ref var c_upgrades = ref _world.GetPool<c_BusinessUpgrades>().Add(businessEntity);

                    foreach (var pair in pair.Value.Upgrades)
                    {
                        var upgradeId = pair.Key;
                        var upgradeConfig = pair.Value;
                        var upgradeInfo = c_upgrades.InfoById;

                        upgradeInfo[upgradeId] = new UpgradeInfo
                        (
                            purchaseData: default,
                            factor: upgradeConfig.PercentFactor * 0.01f
                        );

                        var upgradeView = businessView.UpgradeViews[upgradeId];
                        upgradeView.Init(upgradeId, Settings, _world, businessEntity, playerEntity);

                        var isUpgradePurchased = businessSaveData.UpgradeStatusesById[upgradeId];
                        var upgradePurchaseData = upgradeInfo[upgradeId].PurchaseData;

                        upgradePurchaseData.Value = (upgradeConfig.Price, isUpgradePurchased);
                    }
                }

                void SetupState()
                {
                    ref var c_state = ref _world.GetPool<c_BusinessState>().Get(businessEntity);

                    c_state.Level.Value = businessSaveData.Level;
                    c_state.LevelUpPrice.Value = (c_state.Level.Value + 1) * businessConfig.BasePrice;
                    c_state.Income.Value = businessSaveData.Income;

                    var rawDelay = businessSaveData.IncomeDelay;
                    var normalizedDelay = rawDelay / businessConfig.BaseIncomeDelay;
                    c_state.ElapsedIncomeDelay.Value = (rawDelay, normalizedDelay);

                    if (c_state.Level.Value >= 1)
                        _world.GetPool<t_IsPurchased>().Add(businessEntity);
                }
            }
        }

        void SetupPlayer()
        {
            ref var c_playerBalance = ref _world.GetPool<c_Balance>().Get(playerEntity);
            BalancePanelView.Init(_world, playerEntity);
            c_playerBalance.Amount.Value = saveData.PlayerBalance;
        }

        void InitSystems()
        {
            _systems
                .Add(new s_UpdateBalance())
                .Add(new s_UpdateBusinessIncome(Settings))
                .Add(new s_IncreaseBusinessLevel(Settings))
                .Add(new s_BuyBusinessUpgrade())
                .Add(new s_ElapseIncomeDelay(Settings))
                .Add(new s_CollectIncomeFromBusiness(Settings))
                .Init();
        }
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

    private void OnApplicationPause() => _saveService.SaveProgress();

    private void OnApplicationQuit() => _saveService.SaveProgress();

    private void ResetScrollToTop()
    {
        Scroll.normalizedPosition = new Vector2(Scroll.normalizedPosition.x, 1f);
    }
}