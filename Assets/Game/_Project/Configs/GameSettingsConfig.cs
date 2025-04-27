using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/GameSettings")]
public class GameSettingsConfig : ScriptableObject
{
    public BusinessConfig[] Businesses;
    public float InitialBalance;
    public GameObject BusinessPrefab;
}