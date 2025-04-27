using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Business", menuName = "Configs/Business")]
public class BusinessConfig : ScriptableObject
{
    public float IncomeDelay;
    public float BasePrice;
    public float BaseIncome;
    public List<(float price, float multiplier)> Upgrades;
    public TitlesConfig Titles;
}