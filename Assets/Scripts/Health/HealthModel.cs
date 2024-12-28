using System;

public class HealthModel
{
    private int _maxValue;
    public HealthModel(int value)
    {
        Value = value;
        _maxValue = value;
    }

    public event Action DamageTaken;
    public event Action Restored;
    public event Action Over;

    public int Value { get; private set; }

    public void TakeDamage()
    {
        if(Value <= 0)
        {
            return;
        }

        Value--;
        DamageTaken?.Invoke();

        if (Value <= 0)
        {
            Over?.Invoke();
        }
    }

    public void Restore()
    {
        Value = _maxValue;
        Restored?.Invoke();
    }
}