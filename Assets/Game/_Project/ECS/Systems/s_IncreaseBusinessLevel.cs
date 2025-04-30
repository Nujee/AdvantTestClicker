using Leopotam.EcsLite;

public sealed class s_IncreaseBusinessLevel : IEcsInitSystem, IEcsRunSystem
{
    private EcsPool<c_BusinessState> _businessStatePool = default;
    private EcsPool<c_BusinessData> _businessDataPool = default;
    private EcsPool<c_Balance> _balancePool = default;
    private EcsPool<r_UpdateBalance> _updateBalancePool = default;

    private EcsFilter _levelledUpBusinessfilter = default;

    private EcsWorld _world = default;

    private readonly GameSettingsConfig _settings;

    public s_IncreaseBusinessLevel(GameSettingsConfig settings) => _settings = settings;

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();
        _levelledUpBusinessfilter = _world
            .Filter<c_BusinessState>()
            .Inc<c_BusinessData>()
            .Inc<r_LevelUpClicked>()
            .End();

        _businessStatePool = _world.GetPool<c_BusinessState>();
        _businessDataPool = _world.GetPool<c_BusinessData>();
        _balancePool = _world.GetPool<c_Balance>();
        _updateBalancePool = _world.GetPool<r_UpdateBalance>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var businessEntity in _levelledUpBusinessfilter)
        {
            ref var c_data = ref _businessDataPool.Get(businessEntity);

            if (c_data.PlayerEntityPacked.Unpack(_world, out var playerEntityUnpacked))
            {
                ref var c_playerBalance = ref _balancePool.Get(playerEntityUnpacked);
                ref var c_state = ref _businessStatePool.Get(businessEntity);

                // Check if balance is enough to level up
                if (c_playerBalance.Amount.Value >= c_state.LevelUpPrice.Value)
                {
                    // Update player balance
                    ref var r_updateBalance = ref _updateBalancePool.AddOrGet(playerEntityUnpacked);
                    r_updateBalance.Amount -= c_state.LevelUpPrice.Value;

                    // Update business level
                    // If it was 0 -> 1 transition, then tag as purchased
                    c_state.Level.Value++;
                    if (c_state.Level.Value == 1)
                        _world.GetPool<t_IsPurchased>().Add(businessEntity);

                    // Update business level-up price according to formula
                    var config = _settings.BusinessConfigsById[c_data.Id];
                    c_state.LevelUpPrice.Value = (c_state.Level.Value + 1) * config.BasePrice;

                    // Update business income
                    _world.GetPool<r_UpdateBusinessIncome>().Add(businessEntity);
                }
            }

            _world.GetPool<r_LevelUpClicked>().Del(businessEntity);
        }
    }
}