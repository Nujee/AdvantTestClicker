using UnityEngine;
using Leopotam.EcsLite;
using UnityEngine.SceneManagement;

public class EntryInstaller : MonoBehaviour
{
    [field: SerializeField] private GameConfig gameConfig;
    [field: SerializeField] private SaveService saveService;

    private EcsWorld _world;
    private EcsSystems _systems;

    private void Awake()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        var (playerBalance, businessData) = saveService.LoadProgress(gameConfig.businesses.Length);

        InitBusinessData(businessData);

        SceneManager.LoadScene("MainScene");
    }

    private void InitBusinessData(BusinessData[] businessData)
    {
        var businessComponentPool = _world.GetPool<c_Business>();

        for (int i = 0; i < businessData.Length; i++)
        {
            ref var business = ref businessComponentPool.Add(_world.NewEntity());
            business.Level = businessData[i].Level;
            business.IncomeProgress = businessData[i].IncomeProgress;
            business.UpgradesPurchased = businessData[i].UpgradesPurchased;
        }
    }

    private void OnApplicationQuit()
    {
        var a = _world.GetPool<c_Business>();

        //saveService.SaveProgress();
    }
}