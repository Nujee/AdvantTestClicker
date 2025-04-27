using System;
using UnityEngine;

public class SaveService
{
    private const string PlayerBalanceKey = "PlayerBalance";
    private const string BusinessCurrentIncomeDelayKey = "BusinessCurrentIncomeDelay";
    private const string BusinessLevelKey = "BusinessLevel";
    private const string BusinessUpgradesKey = "BusinessUpgrades";

    private readonly GameSettingsConfig _settings;

    public SaveService(GameSettingsConfig settings) => _settings = settings;

    public (float playerBalance, BusinessState[] businessData) LoadProgress(int numberOfBusinesses)
    {
        var playerBalance = PlayerPrefs.GetFloat(PlayerBalanceKey, _settings.InitialBalance);

        var businessData = new BusinessState[numberOfBusinesses];
        for (int i = 0; i < numberOfBusinesses; i++)
        {
            //var incomeProgress = PlayerPrefs.GetFloat($"{BusinessIncomeProgressKey}_{i}", 0f);
            var defaultLevel = (i == 0) ? 1 : 0; 
            var level = PlayerPrefs.GetInt($"{BusinessLevelKey}_{i}", defaultLevel);
            var upgradesPurchased = PlayerPrefs.GetInt($"{BusinessUpgradesKey}_{i}", 0);

            //businessData[i] = new BusinessData(incomeProgress, level, upgradesPurchased);
        }

        return (playerBalance, businessData);
    }
}

[Serializable]
public struct BusinessState
{
    public int Level;
    public float IncomeProgress;
    public bool[] UpgradesPurchased;

    public BusinessState(int level, float incomeProgress, bool[] upgradesPurchased)
    {
        Level = level;
        IncomeProgress = incomeProgress;
        UpgradesPurchased = upgradesPurchased;
    }
}