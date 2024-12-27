using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private LevelCellsSpawner _levelCellsSpawner;
    [SerializeField] private LevelsDataSource _levelsDataSource;
    [SerializeField] private ColorPicker _colorPicker;
    [SerializeField] private HealthView _healthView;

    private async void Awake()
    {
        StreaminAssetsReader streaminAssetsReader = new();
        ColorsDataSource colorsDataSource = new();
        LevelData level = await streaminAssetsReader.ReadAsync<LevelData>("level1.json");

        int health = 3;
        HealthModel healthModel = new(health);

        _levelCellsSpawner.Init(colorsDataSource);
        CellsClickHandler cellsClickHandler = new(_levelCellsSpawner, _colorPicker, healthModel);
        _colorPicker.Init(_levelCellsSpawner, colorsDataSource);
        _healthView.Init(healthModel);

        _levelCellsSpawner.SpawnLevel(level);
    }
}
