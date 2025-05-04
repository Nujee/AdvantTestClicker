using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class SaveService
{
    private GameSettingsConfig _settings;
    private EcsWorld _world;

    private string SavePath => Path.Combine(Application.persistentDataPath, "game_save.json");

    public SaveService(GameSettingsConfig settings, EcsWorld world)
    {
        _settings = settings;
        _world = world;
    }

    public SaveData LoadProgress()
    {
        if (!File.Exists(SavePath))
            return CreateDefaultSave();

        var jsonData = new JsonSaveData();
        try
        {
            var json = File.ReadAllText(SavePath);
            jsonData = JsonUtility.FromJson<JsonSaveData>(json);
            return ConvertFromJson();
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failed: {e}");
            return CreateDefaultSave();
        }

        SaveData CreateDefaultSave()
        {
            var defaultBalance = _settings.InitialBalance;
            var businessesById = new Dictionary<int, BusinessData>();

            foreach (var pair in _settings.BusinessConfigsById)
            {
                var businessId = pair.Key;
                var config = pair.Value;

                var upgradeStatuses = new Dictionary<int, bool>();

                foreach (var upgradePair in config.Upgrades)
                    upgradeStatuses[upgradePair.Key] = false;

                // If dict is empty (count == 0), then newly created BusinessData is the 1st one in it -
                //  - meeting the condition of the 1st business being lvl_1 and others being lvl_0.
                var defaultLevel = (businessesById.Count == 0) ? 1 : 0;

                businessesById[businessId] = new BusinessData(
                    level: defaultLevel,
                    income: config.BaseIncome,
                    delay: 0f,
                    upgradeStatusesById: upgradeStatuses
                );
            }

            return new SaveData(defaultBalance, businessesById);
        }

        SaveData ConvertFromJson()
        {
            var businesses = new Dictionary<int, BusinessData>();
            foreach(var jsonBusiness in jsonData.Businesses)
            {
                var upgradeStatuses = new Dictionary<int, bool>();
                foreach (var jsonUpgrade in jsonBusiness.Upgrades)
                {
                    upgradeStatuses[jsonUpgrade.Id] = jsonUpgrade.IsPurchased;
                }

                businesses[jsonBusiness.Id] = new BusinessData(
                    level: jsonBusiness.Level,
                    income: jsonBusiness.Income,
                    delay: jsonBusiness.IncomeDelay,
                    upgradeStatusesById: upgradeStatuses
                );
            }

            return new SaveData(jsonData.PlayerBalance, businesses);
        }
    }

    public void SaveProgress()
    {
        var jsonData = new JsonSaveData();
        try
        {
            jsonData = ConvertToJson();
            var json = JsonUtility.ToJson(jsonData, prettyPrint: true);
            File.WriteAllText(SavePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e}");
        }

        JsonSaveData ConvertToJson()
        {
            foreach (var entity in _world.Filter<c_Balance>().End())
            {
                jsonData.PlayerBalance = _world.GetPool<c_Balance>().Get(entity).Amount.Value;
                break;
            }

            var businessEntities = _world
                .Filter<c_BusinessData>()
                .Inc<c_BusinessState>()
                .Inc<c_BusinessUpgrades>()
                .End();

            jsonData.Businesses = new JsonBusinessData[businessEntities.GetEntitiesCount()];

            var dataPool = _world.GetPool<c_BusinessData>();
            var statePool = _world.GetPool<c_BusinessState>();
            var upgradesPool = _world.GetPool<c_BusinessUpgrades>();

            var businessIndex = 0;
            foreach (var entity in businessEntities)
            {
                ref var data = ref dataPool.Get(entity);
                ref var state = ref statePool.Get(entity);
                ref var upgrades = ref upgradesPool.Get(entity);

                var upgradesData = new JsonUpgradeData[upgrades.InfoById.Count];

                var upgradeIndex = 0;
                foreach (var upgrade in upgrades.InfoById)
                {
                    upgradesData[upgradeIndex++] = new JsonUpgradeData
                    {
                        Id = upgrade.Key,
                        IsPurchased = upgrade.Value.PurchaseData.Value.isPurchased
                    };
                }

                jsonData.Businesses[businessIndex++] = new JsonBusinessData
                {
                    Id = data.Id,
                    Level = state.Level.Value,
                    Income = state.Income.Value,
                    IncomeDelay = state.ElapsedIncomeDelay.Value.raw,
                    Upgrades = upgradesData
                };
            }

            return jsonData;
        }
    }

    [Serializable]
    private class JsonSaveData
    {
        public float PlayerBalance;
        public JsonBusinessData[] Businesses;
    }

    [Serializable]
    private class JsonBusinessData
    {
        public int Id;
        public int Level;
        public float Income;
        public float IncomeDelay;
        public JsonUpgradeData[] Upgrades;
    }

    [Serializable]
    private class JsonUpgradeData
    {
        public int Id;
        public bool IsPurchased;
    }
}

public readonly struct SaveData
{
    public readonly float PlayerBalance;
    public readonly Dictionary<int, BusinessData> BusinessesById;

    public SaveData(float balance, Dictionary<int, BusinessData> businessesById)
    {
        PlayerBalance = balance;
        BusinessesById = businessesById;
    }
}

public readonly struct BusinessData
{
    public readonly int Level;
    public readonly float Income;
    public readonly float IncomeDelay;
    public readonly Dictionary<int, bool> UpgradeStatusesById;

    public BusinessData(int level, float income, float delay, Dictionary<int, bool> upgradeStatusesById)
    {
        Level = level;
        Income = income;
        IncomeDelay = delay;
        UpgradeStatusesById = upgradeStatusesById;
    }
}