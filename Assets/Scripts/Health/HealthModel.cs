using System;

public class HealthModel
{
    public HealthModel(int value)
    {
        Value = value;
    }

    public event Action DamageTaken;
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
}