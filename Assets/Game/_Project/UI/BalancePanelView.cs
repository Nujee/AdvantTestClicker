using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

public sealed class BalancePanelView : MonoBehaviour
{
    private ReactiveProperty<float> _balance;

    [field: SerializeField] public TMP_Text BalanceText { get; private set; }

    public void Init(EcsWorld world, int playerEntity)
    {
        ref var c_balance = ref world.GetPool<c_Balance>().Get(playerEntity);

        _balance = c_balance.Amount;
        _balance.OnValueSet += RenewBalanceText;
    }

    private void OnDestroy()
    {
        if (_balance != null)
            _balance.OnValueSet -= RenewBalanceText;
    }

    private void RenewBalanceText(float newBalance)
    {
        BalanceText.text = "Баланс: " + newBalance.ToString("0.##") + "$";
    }
}