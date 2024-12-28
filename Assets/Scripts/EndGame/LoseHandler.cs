using System;
using UnityEngine;
using UnityEngine.UI;

public class LoseHandler : MonoBehaviour
{
    [SerializeField] private GameObject _restartPanel;
    [SerializeField] private Button _restartGameButton;
    [SerializeField] private LevelsSwitchHandler _levelsSwitchHandler;

    private HealthModel _healthModel;

    public void Init(HealthModel healthModel)
    {
        _healthModel = healthModel;
    }

    public void Enable()
    {
        _healthModel.Over += OnHealthOver;
        _restartGameButton.onClick.AddListener(RestartGame);
    }

    public void Disable()
    {
        _healthModel.Over -= OnHealthOver;
        _restartGameButton.onClick.RemoveListener(RestartGame);
    }

    private void RestartGame()
    {
        _levelsSwitchHandler.RestartLevel();
        _healthModel.Restore();
        _restartPanel.SetActive(false);
    }

    private void OnHealthOver()
    {
        _restartPanel.SetActive(true);
    }
}
