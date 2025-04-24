using System;
using UnityEngine;

public class SaveService
{
    private const string PlayerBalanceKey = "PlayerBalance";
    private const string BusinessProgressKey = "BusinessProgress";
    private const string BusinessLevelKey = "BusinessLevel";
    private const string BusinessUpgradesKey = "BusinessUpgrades";

    public void SaveProgress(float playerBalance, BusinessData[] businessData)
    {
        PlayerPrefs.SetFloat(PlayerBalanceKey, playerBalance);

        for (int i = 0; i < businessData.Length; i++)
        {
            PlayerPrefs.SetFloat($"{BusinessProgressKey}_{i}", businessData[i].IncomeProgress);
            PlayerPrefs.SetFloat($"{BusinessLevelKey}_{i}", businessData[i].Level);
            PlayerPrefs.SetFloat($"{BusinessUpgradesKey}_{i}", businessData[i].UpgradesPurchased);
        }

        PlayerPrefs.Save();
    }

    public (float playerBalance, BusinessData[] businessData) LoadProgress(int numberOfBusinesses)
    {
        var playerBalance = PlayerPrefs.GetFloat(PlayerBalanceKey, 0f);

        var businessData = new BusinessData[numberOfBusinesses];
        for (int i = 0; i < numberOfBusinesses; i++)
        {
            var incomeProgress = PlayerPrefs.GetFloat($"{BusinessProgressKey}_{i}", 0f);
            var level = PlayerPrefs.GetInt($"{BusinessLevelKey}_{i}", 0);
            var upgradesPurchased = PlayerPrefs.GetInt($"{BusinessUpgradesKey}_{i}", 0);

            businessData[i] = new BusinessData(incomeProgress, level, upgradesPurchased);
        }

        return (playerBalance, businessData);
    }
}

[Serializable]
public struct BusinessData
{
    public float IncomeProgress;
    public int Level;
    public int UpgradesPurchased;

    public BusinessData(float incomeProgress, int level, int upgradesPurchased)
    {
        IncomeProgress = incomeProgress;
        Level = level;
        UpgradesPurchased = upgradesPurchased;
    }
}