using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Business", menuName = "Configs/Business")]
public sealed class BusinessConfig : ScriptableObject
{
    [field: SerializeField] public float BasePrice { get; private set; }
    [field: SerializeField] public float BaseIncome { get; private set; }
    [field: SerializeField] public float BaseIncomeDelay { get; private set; }
    [field: SerializeField] public TitlesConfig Titles { get; private set; }
    [field: SerializeField] public IdMapSerializable<UpgradeConfig> Upgrades { get; private set; }
}