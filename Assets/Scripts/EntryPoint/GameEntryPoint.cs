using System;
using UnityEngine;
using UnityEngine.UI;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private LevelCellsSpawner _levelCellsSpawner;
    [SerializeField] private LevelsDataSource _levelsDataSource;
    [SerializeField] private ColorPicker _colorPicker;
    [SerializeField] private HealthView _healthView;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private Image _finalImage;
    [SerializeField] private WinHandler _winHandler;

    private CellsClickHandler _cellsClickHandler;
    private DeathHandler _deathHandler;

    private async void Awake()
    {
        StreaminAssetsReader streaminAssetsReader = new();
        ColorsDataSource colorsDataSource = new();
        LevelData level = await streaminAssetsReader.ReadAsync<LevelData>("level1.json");
        CoroutineProvider coroutineProvider = new GameObject("CoroputineProvider").AddComponent<CoroutineProvider>();

        int health = 3;
        HealthModel healthModel = new(health);

        _levelCellsSpawner.Init(colorsDataSource);

        _winHandler.Enable();

        _cellsClickHandler = new(_levelCellsSpawner, _colorPicker, healthModel, _winHandler);
        _cellsClickHandler.Enable();

        _deathHandler = new(healthModel, _losePanel);
        _deathHandler.Enable();

        _colorPicker.Init(_levelCellsSpawner, colorsDataSource);
        _healthView.Init(healthModel);

        await _levelCellsSpawner.SpawnLevel(level);

        _finalImage.GetComponent<RectTransform>().sizeDelta = _levelCellsSpawner.GetGridSize();
    }

    private void OnDestroy()
    {
        _cellsClickHandler.Disable();
        _deathHandler.Disable();
    }
}
