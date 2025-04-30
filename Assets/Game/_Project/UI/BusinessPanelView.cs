using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class BusinessPanelView : MonoBehaviour
{
    private ReactiveProperty<float> _income;
    private ReactiveProperty<int> _level;
    private ReactiveProperty<float> _levelUpPrice;
    private ReactiveProperty<(float raw, float normalized)> _incomeDelay;

    [field: SerializeField] public TMP_Text TitleText { get; private set; }
    [field: SerializeField] public TMP_Text LevelText { get; private set; }
    [field: SerializeField] public TMP_Text IncomeText { get; private set; }
    [field: SerializeField] public TMP_Text LevelUpText { get; private set; }
    [field: SerializeField] public Slider IncomeDelaySlider { get; private set; }
    [field: SerializeField] public Button LevelUpButton { get; private set; }
    [field: SerializeField] public IdMapSerializable<UpgradeView> UpgradeViews { get; private set; }

    public void Init(GameSettingsConfig settings, EcsWorld world, int businessEntity)
    {
        ref var c_state = ref world.GetPool<c_BusinessState>().Get(businessEntity);
        ref var c_data = ref world.GetPool<c_BusinessData>().Get(businessEntity);

        var titleConfig = settings.BusinessConfigsById[c_data.Id].Titles;
        TitleText.text = titleConfig.BusinessTitle;

        _income = c_state.Income;
        _income.OnValueSet += RenewIncomeText;

        _level = c_state.Level;
        _level.OnValueSet += RenewLevelText;

        _levelUpPrice = c_state.LevelUpPrice;
        _levelUpPrice.OnValueSet += RenewLevelUpPriceText;

        _incomeDelay = c_state.IncomeDelayElapsed;
        _incomeDelay.OnValueSet += MoveIncomeDelaySlider;

        LevelUpButton.onClick.AddListener(() =>
        {
            world.GetPool<r_LevelUpClicked>().Add(businessEntity);
        });
    }

    private void OnDestroy()
    {
        if (_income != null)
            _income.OnValueSet -= RenewIncomeText;

        if (_level != null)
            _level.OnValueSet -= RenewLevelText;

        if (_levelUpPrice != null)
            _levelUpPrice.OnValueSet -= RenewLevelUpPriceText;

        if (_incomeDelay != null)
            _incomeDelay.OnValueSet -= MoveIncomeDelaySlider;
    }

    private void RenewIncomeText(float newIncome)
    {
        IncomeText.text = $"{newIncome:0.##}$"; 
    }

    private void RenewLevelText(int newLevel)
    {
        LevelText.text = newLevel.ToString();
    }

    private void RenewLevelUpPriceText(float newLevelUpPrice)
    {
        LevelUpText.text = $"����: {newLevelUpPrice:0.##}$";
    }

    private void MoveIncomeDelaySlider((float raw, float normalized) delayElapsed)
    {
        IncomeDelaySlider.value = delayElapsed.normalized;
    }
}