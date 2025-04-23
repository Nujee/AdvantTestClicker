using UnityEngine;

[CreateAssetMenu(fileName = "BusinessConfig", menuName = "Configs/BusinessConfig")]
public class BusinessConfig : ScriptableObject
{
    [System.Serializable]
    public struct Upgrade
    {
        public float price;
        public float profitMultiplier;
    }

    public string businessTitle;
    public float incomeDelay;
    public float baseCost;
    public float baseIncome;
    public Upgrade upgrade1;
    public Upgrade upgrade2;
}