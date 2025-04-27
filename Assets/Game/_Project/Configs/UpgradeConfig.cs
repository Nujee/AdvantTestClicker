using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Configs/Upgrade")]
public class UpgradeConfig : ScriptableObject
{
    [HideInInspector] public int Id;

    public float Price;
    public float Multiplier;
}