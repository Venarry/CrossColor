using UnityEngine;

public class DeathHandler
{
    private readonly HealthModel _healthModel;
    private readonly GameObject _losePanel;

    public DeathHandler(HealthModel healthModel, GameObject losePanel)
    {
        _healthModel = healthModel;
        _losePanel = losePanel;

        losePanel.SetActive(false);
    }

    public void Enable()
    {
        _healthModel.Over += OnHealthOver;
    }

    public void Disable()
    {
        _healthModel.Over -= OnHealthOver;
    }

    private void OnHealthOver()
    {
        _losePanel.SetActive(true);
    }
}
