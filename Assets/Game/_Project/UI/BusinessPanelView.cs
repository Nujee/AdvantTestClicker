using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class BusinessPanelView : MonoBehaviour
{
    private ReactiveProperty<float> _income;
    private ReactiveProperty<int> _level;
    private ReactiveProperty<float> _levelUpPrice;
    private ReactiveProperty<(float raw, float normalized)> _incomeDelay;
    private ReactiveProperty<float> _balance;

    [field: SerializeField] public TMP_Text TitleText { get; private set; }
    [field: SerializeField] public TMP_Text LevelText { get; private set; }
    [field: SerializeField] public TMP_Text IncomeText { get; private set; }
    [field: SerializeField] public TMP_Text LevelUpText { get; private set; }
    [field: SerializeField] public Slider IncomeDelaySlider { get; private set; }
    [field: SerializeField] public Button LevelUpButton { get; private set; }
    [field: SerializeField] public IdMapSerializable<UpgradeView> UpgradeViews { get; private set; }

    public void Init(GameSettingsConfig settings, EcsWorld world, int businessEntity, int playerEntity)
    {
        SetupTitle();
        SubscribeToBusinessStateChanges();
        SubscribeToBalanceChanges();
        SetupLevelUpClickHandler();

        void SetupTitle()
        {
            ref var c_data = ref world.GetPool<c_BusinessData>().Get(businessEntity);

            var titleConfig = settings.BusinessConfigsById[c_data.Id].Titles;
            TitleText.text = titleConfig.BusinessTitle;
        }

        void SubscribeToBusinessStateChanges()
        {
            ref var c_state = ref world.GetPool<c_BusinessState>().Get(businessEntity);

            _income = c_state.Income;
            _income.OnValueSet += UpdateIncomeText;

            _level = c_state.Level;
            _level.OnValueSet += UpdateLevelText;

            _levelUpPrice = c_state.LevelUpPrice;
            _levelUpPrice.OnValueSet += UpdateLevelUpPriceText;

            _incomeDelay = c_state.ElapsedIncomeDelay;
            _incomeDelay.OnValueSet += UpdateIncomeDelaySlider;
        }

        void SubscribeToBalanceChanges()
        {
            ref var c_balance = ref world.GetPool<c_Balance>().Get(playerEntity);

            _balance = c_balance.Amount;
            _balance.OnValueSet += UpdateLevelUpButtonState;
        }

        void SetupLevelUpClickHandler()
        {
            LevelUpButton.onClick.AddListener(() =>
            {
                world.GetPool<r_LevelUpClicked>().Add(businessEntity);
            });
        }
    }

    private void OnDestroy()
    {
        if (_income != null)
            _income.OnValueSet -= UpdateIncomeText;

        if (_level != null)
            _level.OnValueSet -= UpdateLevelText;

        if (_levelUpPrice != null)
            _levelUpPrice.OnValueSet -= UpdateLevelUpPriceText;

        if (_incomeDelay != null)
            _incomeDelay.OnValueSet -= UpdateIncomeDelaySlider;

        LevelUpButton.onClick.RemoveAllListeners();
    }

    private void UpdateIncomeText(float newIncome)
    {
        IncomeText.text = $"{newIncome:0.##}$"; 
    }

    private void UpdateLevelText(int newLevel)
    {
        LevelText.text = newLevel.ToString();
    }

    private void UpdateLevelUpPriceText(float newLevelUpPrice)
    {
        LevelUpText.text = $"Цена: {newLevelUpPrice:0.##}$";
    }

    private void UpdateIncomeDelaySlider((float raw, float normalized) delayElapsed)
    {
        IncomeDelaySlider.value = delayElapsed.normalized;
    }

    private void UpdateLevelUpButtonState(float newBalance)
    {
        var isBalanceEnough = (newBalance >= _levelUpPrice.Value);
        LevelUpButton.interactable = isBalanceEnough;
    }
}