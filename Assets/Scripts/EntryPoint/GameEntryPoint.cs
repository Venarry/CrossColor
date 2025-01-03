using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameEntryPoint : MonoBehaviour
{
    private const string PathJsonEnding = ".json";

    [SerializeField] private LevelCellsSpawner _levelCellsSpawner;
    [SerializeField] private LevelsDataSource _levelsDataSource;
    [SerializeField] private ColorPicker _colorPicker;
    [SerializeField] private SwipeHandler _swipeHandler;
    [SerializeField] private TutorialShower _tutorialShower;
    [SerializeField] private LoseHandler _loseHandler;

    [SerializeField] private HealthView _healthView;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private Image _finalImage;
    [SerializeField] private WinPanelShower _winHandler;

    private CellsClickHandler _cellsClickHandler;
    private StreaminAssetsReader _streaminAssetsReader;
    private SideDataColorSwitcher _sideDataColorSwitcher;

    private async void Awake()
    {
        _streaminAssetsReader = new();
        ColorsDataSource colorsDataSource = new();
        CoroutineProvider coroutineProvider = new GameObject("CoroputineProvider").AddComponent<CoroutineProvider>();

        int health = 3;
        HealthModel healthModel = new(health);

        LevelData[] levelsData = _levelsDataSource.Levels;
        LevelLoadData[] levelsLoadData = await LoadLevels();
        _levelCellsSpawner.Init(colorsDataSource, levelsLoadData, levelsData);

        _winHandler.Enable();

        _cellsClickHandler = new(_levelCellsSpawner, _colorPicker, healthModel, _winHandler, _tutorialShower);
        _cellsClickHandler.Enable();

        _sideDataColorSwitcher = new(_levelCellsSpawner);
        _sideDataColorSwitcher.Enable();

        _swipeHandler.Init(_cellsClickHandler);

        _loseHandler.Init(healthModel);
        _loseHandler.Enable();

        _colorPicker.Init(_levelCellsSpawner, colorsDataSource);
        _healthView.Init(healthModel);
        _tutorialShower.Init(colorsDataSource);
        _tutorialShower.Enable();

        _levelCellsSpawner.TrySpawnLevel();

        _finalImage.GetComponent<RectTransform>().sizeDelta = _levelCellsSpawner.GetGridSize();
    }

    private void OnDestroy()
    {
        _cellsClickHandler.Disable();
        _tutorialShower.Disable();
        _sideDataColorSwitcher.Disable();
        _loseHandler.Disable();
    }

    private async Task<LevelLoadData[]> LoadLevels()
    {
        string[] levelNames = _levelsDataSource.LevelsName;
        TextAsset[] levelFiles = _levelsDataSource.LevelsFiles;
        List<LevelLoadData> levels = new();

        foreach (TextAsset levelAsset in levelFiles)
        {
            //LevelLoadData level = await _streaminAssetsReader.ReadAsync<LevelLoadData>(levelName + PathJsonEnding);
            LevelLoadData level = JsonUtility.FromJson<LevelLoadData>(levelAsset.text);
            levels.Add(level);
        }

        return levels.ToArray();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    _winHandler.HandleWinGame();
        //}
    }
}
