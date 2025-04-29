using Leopotam.EcsLite;

public struct c_Balance : IEcsAutoReset<c_Balance>
{
    public ReactiveProperty<float> Amount;

    public void AutoReset(ref c_Balance c)
    {
        c.Amount = new ReactiveProperty<float>();
    }
}