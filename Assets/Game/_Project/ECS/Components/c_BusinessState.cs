using Leopotam.EcsLite;
using System.Collections.Generic;

public struct c_BusinessState : IEcsAutoReset<c_BusinessState>
{
    public int Level;
    public float IncomeDelayElapsed;
    public List<UpgradeState> UpgradeStates;

    public void AutoReset(ref c_BusinessState c) => c.UpgradeStates = new List<UpgradeState>();

    public struct UpgradeState
    {
        public int Id;
        public bool IsPurchased;
    }
}