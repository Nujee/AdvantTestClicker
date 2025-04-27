using UnityEngine;
using Leopotam.EcsLite;
using UnityEngine.SceneManagement;

public class EntryPoint : MonoBehaviour
{
    [field: SerializeField] private GameSettingsConfig gameConfig;
    [field: SerializeField] private SaveService saveService;

    private EcsWorld _world;
    private EcsSystems _systems;

    // TODO: replace field with some other way of playerEntity storage
    private int _playerEntity;

    private void Awake()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        var (playerBalance, businessStates) = saveService.LoadProgress(gameConfig.Businesses.Length);

        var playerEntity = _world.NewEntity();

        var balancePool = _world.GetPool<c_Balance>();
        ref var c_playerBalance = ref balancePool.Add(playerEntity);
        c_playerBalance.Value = gameConfig.InitialBalance;

        for (int i = 0; i < gameConfig.Businesses.Length; i++)
        {
            var businessEntity = _world.NewEntity();

            var config = gameConfig.Businesses[i];
            config.Id = i;
            ref var ñ_data = ref _world.GetPool<c_BusinessData>().Add(businessEntity);
            ñ_data.Config = config;

            ref var c_state = ref _world.GetPool<c_BusinessState>().Add(businessEntity);
            c_state.Level = (ñ_data.Config.Id == 0) ? 1 : 0;
            c_state.IncomeDelayElapsed = 0;
            //c_state.UpgradeStates = new 
        }

        SceneManager.LoadScene("MainScene");
    }

    private void OnApplicationQuit()
    {
        //saveService.SaveProgress();
    }
}