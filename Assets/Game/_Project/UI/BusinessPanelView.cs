using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class BusinessPanelView : MonoBehaviour
{
    [field: SerializeField] public TMP_Text TitleText { get; private set; }
    [field: SerializeField] public TMP_Text LevelText { get; private set; }
    [field: SerializeField] public TMP_Text IncomeText { get; private set; }
    [field: SerializeField] public TMP_Text LevelUpText { get; private set; }
    [field: SerializeField] public UpgradeView Upgrade1View { get; private set; }
    [field: SerializeField] public UpgradeView Upgrade2View { get; private set; }
    [field: SerializeField] public Slider IncomeDelaySlider { get; private set; }

    [System.Serializable]
    public sealed class UpgradeView
    {
        [field: SerializeField] public TMP_Text TitleText { get; private set; }
        [field: SerializeField] public TMP_Text MultiplierText { get; private set; }
        [field: SerializeField] public TMP_Text PurchasedStatusText { get; private set; }
    }
}