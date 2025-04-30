using UnityEngine;


[CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/GameSettings")]
public sealed class GameSettingsConfig : ScriptableObject
{
    [field: SerializeField] public float InitialBalance { get; private set; }
    [field: SerializeField] public IdMapSerializable<BusinessConfig> BusinessConfigsById { get; private set; } = new();
}