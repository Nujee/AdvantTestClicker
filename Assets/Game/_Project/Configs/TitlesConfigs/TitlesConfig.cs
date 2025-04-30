using UnityEngine;

[CreateAssetMenu(fileName = "Titles", menuName = "Configs/Titles")]
public sealed class TitlesConfig : ScriptableObject
{
    [field: SerializeField] public string BusinessTitle { get; private set; }
    [field: SerializeField] public IdMapSerializable<string> UpgradeTitlesById { get; private set; }
}