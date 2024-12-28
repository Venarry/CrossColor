using UnityEngine;
using UnityEngine.UI;

public class LevelsSwitchHandler : MonoBehaviour
{
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _restartLevelButton;
    [SerializeField] private LevelCellsSpawner _levelCellsSpawner;
    [SerializeField] private WinPanelShower _winPanelShower;

    private void OnEnable()
    {
        _nextLevelButton.onClick.AddListener(StartAvailableLevel);
        _restartLevelButton.onClick.AddListener(RestartLevels);
    }

    private void OnDisable()
    {
        _nextLevelButton.onClick.RemoveListener(StartAvailableLevel);
        _restartLevelButton.onClick.RemoveListener(RestartLevels);
    }

    private void StartAvailableLevel()
    {
        _levelCellsSpawner.TrySpawnLevel();
    }

    private void RestartLevels()
    {
        _levelCellsSpawner.ResetLevels();
        _levelCellsSpawner.TrySpawnLevel();
    }
}
