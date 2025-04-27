using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Business", menuName = "Configs/Business")]
public class BusinessConfig : ScriptableObject
{
    [HideInInspector] public int Id;

    public float BaseIncomeDelay;
    public float BasePrice;
    public float BaseIncome;
    public TitlesConfig Titles;
    public List<Upgrade> Upgrades;

    [System.Serializable]
    public class Upgrade
    {
        [HideInInspector] public int Id;

        public float price;
        public float multiplier;
    }
}