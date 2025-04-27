using Leopotam.EcsLite;
using System.Collections.Generic;

public struct c_Upgrades : IEcsAutoReset<c_Upgrades>
{
    public List<Upgrade> List;

    public void AutoReset(ref c_Upgrades c) => c.List = new List<Upgrade>();
}

public struct Upgrade
{
    public bool IsPurchased;
    public float Price;
    public float Multiplier;
}

