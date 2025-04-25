using Leopotam.EcsLite;
using System.Collections.Generic;

public struct c_Business
{
    public int Id;
    public int Level;
    public float IncomeDelay;
    public float BasePrice;
    public float BaseIncome;
    public List<(bool isPurchased, float factor)> Upgrades;
    public EcsPackedEntity PlayerBalancePacked;
}