using UnityEngine;
using Leopotam.EcsLite;
using System.Threading;

public class EntryPoint : MonoBehaviour
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

        ref var c_playerBalance = ref _world.GetPool<c_Balance>().Add(playerEntity);
        BalancePanelView.Init(_world, playerEntity);
        c_playerBalance.Amount.Value = Settings.InitialBalance;

        var businessViewPool = new GenericPool<BusinessPanelView> (BusinessPanelPrefab, 5, BusinessViewsParent);

        foreach (var businessIdToConfig in Settings.BusinessConfigsById)
        {
            var businessEntity = _world.NewEntity();

            ref var c_data = ref _world.GetPool<c_BusinessData>().Add(businessEntity);
            c_data.Id = businessIdToConfig.Key;
            c_data.PlayerEntityPacked = _world.PackEntity(playerEntity);

            ref var c_state = ref _world.GetPool<c_BusinessState>().Add(businessEntity);
            var businessView = businessViewPool.Get();
            businessView.Init(Settings, _world, businessEntity);
            SetUpBusinessState(ref c_state, ref c_data);

            if (c_state.Level.Value == 1)
                _world.GetPool<t_IsPurchased>().Add(businessEntity);
        }

        _systems
            .Add(new s_UpdateBalance())
            .Add(new s_UpdateBusinessIncome(Settings))
            .Add(new s_IncreaseBusinessLevel(Settings))
            .Add(new s_ElapseIncomeDelay(Settings))
            .Add(new s_CollectBusinessIncome(Settings))
            .Init();
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

    private void SetUpBusinessState(ref c_BusinessState c_state, ref c_BusinessData c_data)
    {
        var config = Settings.BusinessConfigsById[c_data.Id];

        c_state.Level.Value = (c_data.Id == 1) ? 1 : 0;

        var levelUpPrice = (c_state.Level.Value + 1) * config.BasePrice;
        c_state.LevelUpPrice.Value = levelUpPrice;

        c_state.Income.Value = config.BaseIncome;

        c_state.IncomeDelayElapsed.Value = (0f, 0f);
    }
}