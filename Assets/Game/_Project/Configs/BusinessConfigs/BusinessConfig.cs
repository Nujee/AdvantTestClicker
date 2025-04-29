using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Business", menuName = "Configs/Business")]
public sealed class BusinessConfig : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field:SerializeField] public float BasePrice { get; private set; }
    [field: SerializeField] public float BaseIncome { get; private set; }
    [field: SerializeField] public float BaseIncomeDelay { get; private set; }
    [field: SerializeField] public TitlesConfig Titles { get; private set; }
    [field: SerializeField] public List<UpgradeConfig> Upgrades { get; private set; }
}