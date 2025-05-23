using Leopotam.EcsLite;

public sealed class s_UpdateBalance : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<c_Balance> _balancePool;
    private EcsPool<r_UpdateBalance> _updateBalanceRequestPool;

    private EcsFilter _balanceToUpdateFilter;

    private EcsWorld _world;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        _balanceToUpdateFilter = _world
            .Filter<c_Balance>()
            .Inc<r_UpdateBalance>()
            .End();

        _balancePool = _world.GetPool<c_Balance>();
        _updateBalanceRequestPool = _world.GetPool<r_UpdateBalance>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var balanceEntity in _balanceToUpdateFilter)
        {
            ref var c_balance = ref _balancePool.Get(balanceEntity);
            ref var r_updateBalance = ref _updateBalanceRequestPool.Get(balanceEntity);

            c_balance.Amount.Value += r_updateBalance.ByAmount;

            _updateBalanceRequestPool.Del(balanceEntity);
        }
    }
}