using Leopotam.EcsLite;

public struct c_BusinessState : IEcsAutoReset<c_BusinessState>
{
    public ReactiveProperty<int> Level;
    public ReactiveProperty<float> Income;
    public ReactiveProperty<float> LevelUpPrice;
    public ReactiveProperty<(float raw, float normalized)> ElapsedIncomeDelay;

    public void AutoReset(ref c_BusinessState c)
    {
        c.Level = new ReactiveProperty<int>();
        c.Income = new ReactiveProperty<float>();
        c.LevelUpPrice = new ReactiveProperty<float>();
        c.ElapsedIncomeDelay = new ReactiveProperty<(float raw, float normalized)>();
    }
}