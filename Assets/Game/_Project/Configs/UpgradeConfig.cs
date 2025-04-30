using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Configs/Upgrade")]
public sealed class UpgradeConfig : ScriptableObject
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
    [field: SerializeField] public int PercentFactor { get; private set; }
}