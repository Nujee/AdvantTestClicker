using Leopotam.EcsLite;

public sealed class s_CollectIncome : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _filter = default;

    private EcsWorld _world;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        _filter = _world.Filter<c_Business>().Inc<t_IsIncomeReady>();
    }

    public void Run(IEcsSystems systems)
    {
        throw new System.NotImplementedException();
    }
}