using System;

public sealed class ReactiveProperty<T>
{
    private T _value;
    public event Action<T> OnValueSet;

    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueSet?.Invoke(_value);
        }
    }
}