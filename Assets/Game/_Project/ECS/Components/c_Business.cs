using Leopotam.EcsLite;
using System.Collections.Generic;

public struct c_Business
{
    public int Id;
    public int Level;
    public float BasePrice;
    public List<(bool isPurchased, float multiplier)> Upgrades;
    public EcsPackedEntity PlayerBalancePacked;
}