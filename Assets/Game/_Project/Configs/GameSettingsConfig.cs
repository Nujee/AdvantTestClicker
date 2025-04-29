using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/GameSettings")]
public sealed class GameSettingsConfig : ScriptableObject
{
    [field: SerializeField] public List<BusinessEntry> BusinessEntries {  get; private set; }
    [field: SerializeField] public float InitialBalance { get; private set; }

    /// <summary>
    /// Code below is a workaround to both "serialize" a dictionary 
    /// and use it to search business config by its ID in O(1)
    /// </summary>

    public Dictionary<int, BusinessConfig> BusinessConfigsById { get; private set; }

    private void OnEnable()
    {
        BusinessConfigsById = new Dictionary<int, BusinessConfig>();
        foreach (var businessEntry in BusinessEntries)
        {
            BusinessConfigsById[businessEntry.Id] = businessEntry.Config;
        }
    }

    [System.Serializable]
    public sealed class BusinessEntry
    {
        public int Id;
        public BusinessConfig Config;
    }
}